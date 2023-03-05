using SandBoxCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace SandBoxCore.DataTransferObjects
{
    [Serializable]
    public class ClassEntity : GenericInfoInterface
    {
        private string fullname;
        private string shortname;

        public List<MethodEntity> Methods { get; set; }
        public string FullName { get => fullname; set => fullname=value; }
        public string ShortName { get => shortname; set => shortname = value; }
    }
}
