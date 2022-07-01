using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassInjector.Exeptions
{
    internal class ClassInjectorExeptions: Exception
    {
        public ClassInjectorExeptions(string message) : base("ClassInjector: " + message)
        {

        }
    }
}
