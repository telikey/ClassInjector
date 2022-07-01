using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassInjector.Exeptions
{
    internal class NoConstructorException : ClassInjectorExeptions
    {
        public NoConstructorException(Type type) : base("Нет конструктора объекта для " + type.Name)
        {

        }
    }
}
