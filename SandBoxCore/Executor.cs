using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security.Permissions;
using System.Security.Policy;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SandBoxCore.DataTransferObjects;
using System.Security.Cryptography;
using SandBoxCore.Interfaces;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;

[assembly: AssemblyKeyFileAttribute("mysandboxapp.snk")]
namespace SandBoxCore
{
    public class Executor
    {

    }


    //The Sandboxer class needs to derive from MarshalByRefObject so that we can create it in another
    // AppDomain and refer to it from the default AppDomain.
    public class Sandboxer : MarshalByRefObject
    {
        static void Main()
        {
        }

        HelperClass helper= new HelperClass();

        public List<AssemblyEntity> GetAllAssembliesFromPath(string filepath)
        {
            string sandboxedassemblyname = Path.GetFileNameWithoutExtension(filepath);
            string sandboxedassemblydirectory = Path.GetDirectoryName(filepath);

            AppDomainSetup adSetup = new AppDomainSetup();
            adSetup.ApplicationBase = sandboxedassemblydirectory;
            PermissionSet permSet = new PermissionSet(PermissionState.None);
            permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            ReflectionPermission restrictedMemberAccessPerm = new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess);
            permSet.AddPermission(restrictedMemberAccessPerm);

            //We want the sandboxer assembly's strong name, so that we can add it to the full trust list.
            StrongName fullTrustAssembly = typeof(Sandboxer).Assembly.Evidence.GetHostEvidence<StrongName>();
            var fullTrustAssemblytest = typeof(Sandboxer).Assembly.Evidence;

            //Now we have everything we need to create the AppDomain, so let's create it.
            AppDomain newDomain = AppDomain.CreateDomain("Sandbox", null, adSetup, permSet, fullTrustAssembly);

            ObjectHandle handle = Activator.CreateInstanceFrom(
            newDomain, typeof(Sandboxer).Assembly.ManifestModule.FullyQualifiedName,
            typeof(Sandboxer).FullName
            );
            //Unwrap the new domain instance into a reference in this domain and use it to execute the
            //untrusted code.
            Sandboxer newDomainInstance = (Sandboxer)handle.Unwrap();
            List<AssemblyEntity> assemblyDTOs = newDomainInstance.ExecuteUntrustedCode(sandboxedassemblyname);
            return assemblyDTOs;
        }



        public List<AssemblyEntity> ExecuteUntrustedCode(string assemblyName)
        {
            List<AssemblyEntity> allassemblies = new List<AssemblyEntity>(); 
            //Load the MethodInfo for a method in the new Assembly. This might be a method you know, or
            //you can use Assembly.EntryPoint to get to the main function in an executable.
            var assembly = Assembly.Load(assemblyName);
            var assemblyDTO = new AssemblyEntity();
            assemblyDTO.FullName = assembly.FullName;
            assemblyDTO.ShortName = assembly.FullName.Split(',')[0];
            assemblyDTO.Classes = new List<ClassEntity>();
            foreach (var classitem in assembly.GetTypes())
            {
                var classDTO = new ClassEntity()
                {
                    FullName = classitem.FullName,
                    ShortName = classitem.Name
                };
                assemblyDTO.Classes.Add(classDTO);
                classDTO.Methods = new List<MethodEntity>();


                foreach (var methoditem in classitem.GetMethods())
                {
                    if (!(helper.IsMethodInBuilt(methoditem))) continue;
                    if (!(helper.IsMethodAllowed(methoditem.GetParameters().ToList()))) continue;
                    var methodDTO= new MethodEntity()
                    {
                        FullName = methoditem.Name,
                        ShortName = methoditem.Name
                    };
                    classDTO.Methods.Add(methodDTO);
                    methodDTO.Parameters = new List<ParameterEntity>();

                    foreach (var parameteritem in methoditem.GetParameters())
                    {
                        //bool paramisallowed = HelperClass.IsParameterAllowed(parameteritem);
                        //if (!paramisallowed)
                        //{
                        //    break;
                        //}
                        var parameterDTO = new ParameterEntity()
                        {
                            FullName = parameteritem.ParameterType.Name,
                            ShortName = parameteritem.Name
                        };
                        methodDTO.Parameters.Add(parameterDTO);
                    }
                }
            }

            allassemblies.Add(assemblyDTO);
            return allassemblies;
        }


        public void SetupSandBox(List<string> permissionboxes, string filepath, string assemblyName, string typeName, string entryPoint, Object[] parameters)
        {
            string sandboxedassemblyname = Path.GetFileNameWithoutExtension(filepath);
            string sandboxedassemblydirectory = Path.GetDirectoryName(filepath);

            AppDomainSetup adSetup = new AppDomainSetup();
            adSetup.ApplicationBase = sandboxedassemblydirectory;
            //Regex myRegex = new Regex(@"http://www\.contoso\.com/.*");
            Regex myRegex = new Regex(@"https://www\..*\.com/.*");
            PermissionSet permSet = new PermissionSet(PermissionState.None);
            foreach (var item in permissionboxes)
            {
                switch (item)
                {
                    case "Network":
                        permSet.AddPermission(new WebPermission(NetworkAccess.Connect, myRegex));
                        break;
                    case "ReadFile":
                        permSet.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read,  @"c:\"));
                        break;
                    case "WriteToFile":
                        permSet.AddPermission(new FileIOPermission(FileIOPermissionAccess.Write, @"c:\"));
                        break;
                    default:
                        break;
                }
            }
            
            permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            ReflectionPermission restrictedMemberAccessPerm = new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess);
            permSet.AddPermission(restrictedMemberAccessPerm);

            //We want the sandboxer assembly's strong name, so that we can add it to the full trust list.
            StrongName fullTrustAssembly = typeof(Sandboxer).Assembly.Evidence.GetHostEvidence<StrongName>();
            var fullTrustAssemblytest = typeof(Sandboxer).Assembly.Evidence;

            //Now we have everything we need to create the AppDomain, so let's create it.
            AppDomain newDomain = AppDomain.CreateDomain("SandboxEecute", null, adSetup, permSet, fullTrustAssembly);

            ObjectHandle handle = Activator.CreateInstanceFrom(
            newDomain, typeof(Sandboxer).Assembly.ManifestModule.FullyQualifiedName,
            typeof(Sandboxer).FullName
            );
            //Unwrap the new domain instance into a reference in this domain and use it to execute the
            //untrusted code.
            Sandboxer newDomainInstance = (Sandboxer)handle.Unwrap();
            newDomainInstance.ExecuteUntrustedCodeBasic(sandboxedassemblyname, typeName, entryPoint, parameters);
        }
        public void ExecuteUntrustedCodeBasic(string assemblyName, string typeName, string entryPoint, Object[] parameters)
        {
            MethodInfo target = Assembly.Load(assemblyName).GetType(typeName).GetMethod(entryPoint);
            try
            {
                //Now invoke the method.
                //bool retVal = (bool)target.Invoke(null, objparam);
                var retVal = target.Invoke(null, parameters);
            }
            catch (Exception error)
            {
                // When we print informations from a SecurityException extra information can be printed if we are
                //calling it with a full-trust stack.
                new PermissionSet(PermissionState.Unrestricted).Assert();
                Console.WriteLine("SecurityException caught:\n{0}", error.ToString());
                if (error.InnerException is System.Security.SecurityException)
                {
                    throw new System.Security.SecurityException("The Sandboxed program does not have the necessary permission to execute");
                }
                else
                {
                }

                CodeAccessPermission.RevertAssert();
                Console.ReadLine();
            }
        }




    }
}