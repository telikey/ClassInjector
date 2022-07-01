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

    public class ObjectFactory : IObjectFactory
    {
        private static Dictionary<Type, IInjectedItemFactory> ObjectsDict = new Dictionary<Type, IInjectedItemFactory>();

        public void Add<TInterface, TClass>(object[] defaultArgs = null, bool IsSingleton = false) where TClass : class, TInterface
        {
            defaultArgs = defaultArgs ?? new object[0];
            if (!ObjectsDict.ContainsKey(typeof(TInterface)))
            {
                var constructor = GetConstructor(typeof(TClass), defaultArgs);
                var parameterValues = GetParameterValues(constructor, defaultArgs);
                var item = InjectedItemFactory.Create<TInterface, TClass>(constructor, parameterValues, IsSingleton);
                ObjectsDict.Add(typeof(TInterface), item);
            }
            else
            {
                throw new InjectedItemIsAlreadyContains(typeof(TInterface));
            }
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

        private bool CheckConstructor(ConstructorInfo constructor, object[] defaultArgs)
        {
            var parametersTypes = constructor
                .GetParameters()
                .Select(x => x.ParameterType).ToArray();

            bool reFlag = true;

            var defaultArgsCopy = defaultArgs;

            foreach (var parameterType in parametersTypes)
            {
                var flag = false;
                foreach (var defaultArg in defaultArgsCopy)
                {
                    var defaultType = defaultArg.GetType();
                    if (defaultType.IsAssignableTo(parameterType))
                    {
                        flag = true;
                        defaultArgsCopy = defaultArgsCopy.Where(x => x != defaultArg).ToArray();
                        break;
                    }
                }
                if (flag)
                {
                    continue;
                }

                foreach (var containType in ObjectsDict)
                {
                    var type = containType.Key;
                    if (type.IsAssignableTo(parameterType))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    reFlag = false;
                    break;
                }
            }
            return reFlag;
        }

        private ConstructorInfo GetConstructor(Type type, object[] defaultArgs)
        {
            var constructors = type.GetConstructors().OrderByDescending(x => x.GetParameters().Length);

            ConstructorInfo selectedConstructor = null;

            foreach (var constructor in constructors)
            {
                if (CheckConstructor(constructor, defaultArgs))
                {
                    selectedConstructor = constructor;
                    break;
                }
            }


            if (selectedConstructor != null)
            {
                return selectedConstructor;
            }
            else
            {
                throw new NoConstructorException(type);
            }
        }

        private object[] GetParameters(ParameterInfo[] parameterInfo, object[] defaultArgs)
        {
            var defaultArgsCopy = defaultArgs;
            var parametersTypes = parameterInfo.Select(x => x.ParameterType).ToArray();
            List<object> values = new List<object>();
            foreach (var parameterType in parametersTypes)
            {
                object value = null;
                foreach (var defaultArg in defaultArgsCopy)
                {
                    var defaultType = defaultArg.GetType();
                    if (defaultType.IsAssignableTo(parameterType))
                    {
                        value=defaultArg;
                        defaultArgsCopy = defaultArgsCopy.Where(x => x != defaultArg).ToArray();
                        break;
                    }
                }

                if (value == null)
                {
                    foreach (var containType in ObjectsDict)
                    {
                        var type = containType.Key;
                        if (type.IsAssignableTo(parameterType))
                        {
                            value = ObjectsDict[type].CreateObject();
                        }
                    }
                }
                values.Add(value);
            }

            return values.ToArray();
        }

        private object[] GetParameterValues(ConstructorInfo constructor, object[] defaultArgs)
        {
            var parametersInfo = constructor.GetParameters();

            return GetParameters(parametersInfo, defaultArgs);
        }
    }
}