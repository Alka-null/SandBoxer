using SandBoxCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SandBoxCore
{
    public class HelperClass : HelperInterface
    {
        private List<Type> allowedtypes = new List<Type>() {
            typeof(Boolean), typeof(Byte), typeof(SByte), typeof(Int16), typeof(UInt16), typeof(Int32), 
            typeof(UInt32), typeof(Int64),typeof(UInt64), typeof(Char), typeof(Double), typeof(Single),
            typeof(Decimal), typeof(String), typeof(DateTime)   
        };

        public Object[] ObjectParameters(List<InvokeParameterInterface> invokeparams)
        {
            Object[] objparams = new Object[invokeparams.Count];
            int i = 0;
            foreach (var item in invokeparams)
            {
                switch (item.ParameterName)
                {
                    case "Int16":
                        objparams[i] = Convert.ToInt16(item.ParameterValue); 
                        i++;
                        break;
                    case "Int32":
                        objparams[i] = Convert.ToInt32(item.ParameterValue);
                        i++;
                        break;
                    case "Int64":
                        objparams[i] = Convert.ToInt64(item.ParameterValue);
                        i++;
                        break;
                    case "UInt16":
                        objparams[i] = Convert.ToUInt16(item.ParameterValue);
                        i++;
                        break;
                    case "UInt32":
                        objparams[i] = Convert.ToUInt32(item.ParameterValue);
                        i++;
                        break;
                    case "UInt64":
                        objparams[i] = Convert.ToUInt64(item.ParameterValue);
                        i++;
                        break;
                    case "Single":
                        objparams[i] = Convert.ToSingle(item.ParameterValue);
                        i++;
                        break;
                    case "Decimal":
                        objparams[i] = Convert.ToDecimal(item.ParameterValue);
                        i++;
                        break;
                    case "Double":
                        objparams[i] = Convert.ToDouble(item.ParameterValue);
                        i++;
                        break;
                    case "Boolean":
                        objparams[i] = Convert.ToBoolean(item.ParameterValue);
                        i++;
                        break;
                    case "String":
                        objparams[i] = Convert.ToString(item.ParameterValue);
                        i++;
                        break;
                    case "Char":
                        objparams[i] = Convert.ToChar(item.ParameterValue);
                        i++;
                        break;
                    case "DateTime":
                        objparams[i] = Convert.ToDateTime(item.ParameterValue);
                        i++;
                        break;
                    default:
                        break;
                }
            }

            return objparams;
        }


        public Type[] TypeParameters(List<InvokeParameterInterface> invokeparams)
        {
            Type[] objparams = new Type[invokeparams.Count];
            int i = 0;
            foreach (var item in invokeparams)
            {
                switch (item.ParameterName)
                {
                    case "Int16":
                        objparams[i] = typeof(System.Int16);
                        i++;
                        break;
                    case "Int32":
                        objparams[i] = typeof(System.Int32);
                i++;
                        break;
                    case "Int64":
                        objparams[i] = typeof(System.Int64);
                i++;
                        break;
                    case "UInt16":
                        objparams[i] = typeof(System.UInt16);
                        i++;
                        break;
                    case "UInt32":
                        objparams[i] = typeof(System.UInt32); 
                        i++;
                        break;
                    case "UInt64":
                        objparams[i] = typeof(System.UInt64);
                        i++;
                        break;
                    case "Single":
                        objparams[i] = typeof(System.Single);
                        i++;
                        break;
                    case "Decimal":
                        objparams[i] = typeof(System.Decimal);
                        i++;
                        break;
                    case "Double":
                        objparams[i] = typeof(System.Double);
                        i++;
                        break;
                    case "Boolean":
                        objparams[i] = typeof(System.Boolean);
                        i++;
                        break;
                    case "String":
                        objparams[i] = typeof(System.String);
                        i++;
                        break;
                    case "Char":
                        objparams[i] = typeof(System.Char);
                        i++;
                        break;
                    case "DateTime":
                        objparams[i] = typeof(System.DateTime);
                        i++;
                        break;
                    default:
                        break;
                }
            }

            return objparams;
        }

        public bool IsMethodInBuilt(MethodInfo method)
        {
            if (method.DeclaringType.Namespace.StartsWith("System")) return false;
            return true;
        }

        public bool IsMethodAllowed(List<ParameterInfo> paraminfos)
        {
            bool ismethodallowed = true;
            foreach (var paraminfo in paraminfos)
            {
                Type param = paraminfo.ParameterType;
                if (!(allowedtypes.Contains(param))) ismethodallowed = false;
            }
            return ismethodallowed;
        }
        public bool IsParameterAllowed(ParameterInfo paraminfo)
        {
            Type param = paraminfo.ParameterType;
            if (allowedtypes.Contains(param)) return true;
            return false;
        }
    }
}
