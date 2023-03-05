using SandBoxCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandBoxCore.DataTransferObjects
{
    public class InvokeParameter : InvokeParameterInterface
    {
        public string ParameterName { get ; set; }
        public string ParameterValue { get; set; }
    }
}
