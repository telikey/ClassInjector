using ClassInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassInjector
{
    public class Injector
    {
        private Injector() { }

        private static IObjectFactory _factory = new ObjectFactory();

        public static void Add<TInterface, TClass>(bool IsSingleton = false) where TClass : class, TInterface
        {
            var factory = _factory;
            factory.Add<TInterface, TClass>(IsSingleton);
        }

        public static dynamic GetObject(Type TInterface)
        {
            var factory = _factory;
            return factory.GetObject(TInterface);
        }

        public static dynamic GetObject<TInterface>()
        {
            var factory = _factory;
            return factory.GetObject<TInterface>();
        }
    }
}
