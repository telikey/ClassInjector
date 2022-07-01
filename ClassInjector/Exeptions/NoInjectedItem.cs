using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassInjector.Exeptions
{
    internal class NoInjectedItem : ClassInjectorExeptions
    {
        public NoInjectedItem(Type type) : base("Нет определения для " + type.Name)
        {

        }
    }
}
