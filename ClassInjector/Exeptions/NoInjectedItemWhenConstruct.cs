using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassInjector.Exeptions
{
    internal class NoInjectedItemWhenConstruct : ClassInjectorExeptions
    {
        public NoInjectedItemWhenConstruct(Type noType, Type inType) : base("Нет определения для " + noType.Name +" при конструировании "+inType.Name)
        {

        }
    }
}
