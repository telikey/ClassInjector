﻿using ClassInjector.Exeptions;
using System.Reflection;

namespace ClassInjector
{

    public interface IObjectFactory
    {
        void Add<TInterface, TClass>(bool IsSingleton = false) where TClass : class, TInterface;
        dynamic GetObject(Type TInterface);
        dynamic GetObject<TInterface>();
    }

    internal class ObjectFactory:IObjectFactory
    {
        private static Dictionary<Type, IInjectedItemFactory> ObjectsDict=new Dictionary<Type, IInjectedItemFactory>();

        public void Add<TInterface,TClass>(bool IsSingleton=false) where TClass : class, TInterface
        {
            TClass FuncToCreateObject(){ 
                var constructor= GetConstructor(typeof(TClass));
                var args = GetArgs(constructor);

                return constructor.Invoke(args) as TClass;
            }
            var item = InjectedItemFactory.Create<TInterface,TClass>(FuncToCreateObject, IsSingleton);
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

        private ConstructorInfo GetConstructor(Type type)
        {
            var constructors = type.GetConstructors();

            ConstructorInfo selectedConstructor = null;
            var maxArgsCount = -1;
            foreach(var constructor in constructors)
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
                return selectedConstructor;
            }
            else
            {
                throw new NoConstructorException(type);
            }
        }
        private object[] GetArgs(ConstructorInfo constructor)
        {
            var parametersInfo= constructor.GetParameters();

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

            return resList.ToArray();
        }
    }
}