using ClassInjector.Exeptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClassInjector
{
    internal interface IInjectedItemFactory
    {
        dynamic CreateObject();
    }

    internal class InjectedItemFactory: IInjectedItemFactory
    {
        private bool _isSingleton = false;
        public bool IsSingleton { get => _isSingleton; }

        private object _staticObject = null;
        private object[] _parametersValues = null;
        private ConstructorInfo _constructor = null;

        private Type _classType = null;

        private InjectedItemFactory(){}

        public static InjectedItemFactory Create<TInterface, TClass>(ConstructorInfo constructor,object[] parametersValues,bool IsSingleton = false) where TClass : class, TInterface
        {
            var factory=new InjectedItemFactory();
            factory._isSingleton = IsSingleton;
            factory._constructor= constructor;
            factory._parametersValues= parametersValues;
            factory._classType = typeof(TClass);

            return factory;
        }

        public dynamic CreateObject()
        {
            if (IsSingleton)
            {
                if (_staticObject != null)
                {
                    return _staticObject;
                }
                else
                {
                    _staticObject = _constructor.Invoke(_parametersValues);
                    return Convert.ChangeType(_staticObject, _classType);
                }
            }
            return _constructor.Invoke(_parametersValues);
        }
    }
}
