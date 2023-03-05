using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandBoxCore.Interfaces
{
    //[Serializable]
    internal interface GenericInfoInterface
    {
        string FullName { get; set; }
        string ShortName { get; set; }
    }
}
