using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassInjector.Exeptions
{
    internal class InjectedItemIsAlreadyContains : ClassInjectorExeptions
    {
        public InjectedItemIsAlreadyContains(Type type) : base("Инжектор уже содержит определение для " + type.Name)
        {

        }
    }
}
