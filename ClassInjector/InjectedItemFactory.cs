using ClassInjector.Exeptions;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private Func<object> ObjectCreateFunc = null;

        private Type ClassType = null;

        private InjectedItemFactory(){}

        public static InjectedItemFactory Create<TInterface, TClass>(Func<object> ObjectCreateFunc,bool IsSingleton = false) where TClass : class, TInterface
        {
            var factory=new InjectedItemFactory();
            factory._isSingleton = IsSingleton;
            factory.ObjectCreateFunc= ObjectCreateFunc;
            factory.ClassType = typeof(TClass);

            return factory;
        }

        public dynamic CreateObject()
        {
            if (IsSingleton)
            {
                if (_staticObject != null)
                {
                    return Convert.ChangeType(_staticObject,ClassType);
                }
                else
                {
                    _staticObject = Convert.ChangeType(ObjectCreateFunc.Invoke(), ClassType);
                    return Convert.ChangeType(_staticObject, ClassType);
                }
            }
            return Convert.ChangeType(ObjectCreateFunc.Invoke(), ClassType);
        }
    }
}
