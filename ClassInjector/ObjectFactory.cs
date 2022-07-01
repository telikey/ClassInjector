using ClassInjector.Exeptions;
using System.Reflection;

namespace ClassInjector
{

    public interface IObjectFactory
    {
        void Add<TInterface, TClass>(object[] defaultArgs = null, bool IsSingleton = false) where TClass : class, TInterface;
        dynamic GetObject(Type TInterface);
        dynamic GetObject<TInterface>();
    }

    public class ObjectFactory:IObjectFactory
    {
        private static Dictionary<Type, IInjectedItemFactory> ObjectsDict=new Dictionary<Type, IInjectedItemFactory>();

        public void Add<TInterface,TClass>(object[] defaultArgs=null,bool IsSingleton=false) where TClass : class, TInterface
        {
            TClass FuncToCreateObject(object[] defaultArgs)
            { 
                if (GetConstructor(typeof(TClass), defaultArgs, out ConstructorInfo outConstructor))
                {
                    var args = GetArgs(outConstructor,defaultArgs);
                    return outConstructor.Invoke(args) as TClass;
                }
                else
                {
                    return outConstructor.Invoke(defaultArgs) as TClass;
                }
            }
            var item = InjectedItemFactory.Create<TInterface,TClass>(FuncToCreateObject, defaultArgs, IsSingleton);
            ObjectsDict.Add(typeof(TInterface), item);
        }

        public dynamic GetObject(Type TInterface)
        {
            if (ObjectsDict.ContainsKey(TInterface))
            {
                return ObjectsDict[TInterface].CreateObject();
            }
            else
            {
                throw new NoInjectedItem(TInterface);
            }
        }

        public dynamic GetObject<TInterface>()
        {
            if (ObjectsDict.ContainsKey(typeof(TInterface)))
            {
                return ObjectsDict[typeof(TInterface)].CreateObject();
            }
            else
            {
                throw new NoInjectedItem(typeof(TInterface));
            }
        }

        private bool GetConstructor(Type type, object[] defaultArgs,out ConstructorInfo outConstructor)
        {
            if (defaultArgs == null)
            {
                return GetConstructorWithoutDefaultArgs(type, out outConstructor);
            }
            else
            {
                return GetConstructorByDefaultArgs(type, defaultArgs, out outConstructor);
            }
        }

        private bool GetConstructorWithoutDefaultArgs(Type type, out ConstructorInfo outConstructor)
        {
            var constructors = type.GetConstructors();

            ConstructorInfo selectedConstructor = null;
            var maxArgsCount = -1;
            foreach (var constructor in constructors)
            {
                var count = constructor.GetParameters().Count();
                if (maxArgsCount < count)
                {
                    selectedConstructor = constructor;
                    maxArgsCount = count;
                }
            }


            if (selectedConstructor != null)
            {
                outConstructor= selectedConstructor;
                return true;
            }
            else
            {
                throw new NoConstructorException(type);
            }
        }

        private bool GetConstructorByDefaultArgs(Type type,object[] defaultArgs, out ConstructorInfo outConstructor)
        {
            var types = defaultArgs.Select(x => x.GetType()).ToArray();

            ConstructorInfo[] constructors = type.GetConstructors();
            ConstructorInfo selectedConstructor = null;

            List<Type[]> listOfTypes = new List<Type[]>();

            foreach(var constructor in constructors)
            {
                listOfTypes.Add(constructor.GetParameters().Select(x=>x.ParameterType).ToArray());
            }

            var listWithDefaults = listOfTypes.Where(x=>types.Except(x).Count()==0).ToList();
            var maxLengthList = listWithDefaults.MaxBy(x=>x.Length);
            var typesToLoad = maxLengthList.Except(types);

            bool BigOrDefault = true;
            foreach(var tp in typesToLoad)
            {
                if (!ObjectsDict.ContainsKey(tp))
                {
                    BigOrDefault = false;
                }
            }

            if (BigOrDefault)
            {
                selectedConstructor = type.GetConstructor(maxLengthList);
            }
            else
            {
                selectedConstructor = type.GetConstructor(types);
            }

            if (selectedConstructor != null)
            {

                outConstructor = selectedConstructor;
                return BigOrDefault;
            }
            else
            {
                throw new NoConstructorException(type);
            }
        }

        private object[] GetArgs(ConstructorInfo constructor, object[] defaultArgs)
        {
            var types = defaultArgs!=null?defaultArgs.Select(x => x.GetType()).ToArray():new object[0];
            var parametersInfo = constructor.GetParameters().Where(x=>!types.Contains(x.ParameterType)).ToArray();

            List<object> resList= new List<object>();
            foreach(var parameterInfo in parametersInfo)
            {
                if (ObjectsDict.ContainsKey(parameterInfo.ParameterType))
                {
                    var factory = ObjectsDict[parameterInfo.ParameterType];
                    resList.Add(factory.CreateObject());
                }
                else
                {
                    throw new NoInjectedItemWhenConstruct(parameterInfo.ParameterType,constructor.DeclaringType);
                }
            }

            resList.AddRange(defaultArgs != null ? defaultArgs : new object[0]);

            return resList.ToArray();
        }
    }
}