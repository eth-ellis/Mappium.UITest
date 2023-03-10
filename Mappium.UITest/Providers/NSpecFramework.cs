using System;
using System.Reflection;

namespace Mappium.UITest.Providers
{
    internal class NSpecFramework : ITestFramework
    {
        private Assembly assembly;

        public bool IsAvailable
        {
            get
            {
                try
                {
                    assembly = Assembly.Load(new AssemblyName("nspec"));

                    if (assembly is null)
                    {
                        return false;
                    }

                    int majorVersion = assembly.GetName().Version.Major;

                    return majorVersion >= 2;
                }
                catch
                {
                    return false;
                }
            }
        }

        public void Throw(string message)
        {
            Type exceptionType = assembly.GetType("NSpec.Domain.AssertionException");
            if (exceptionType is null)
            {
                throw new Exception("Failed to create the NSpec assertion type");
            }

            throw (Exception)Activator.CreateInstance(exceptionType, message);
        }

        public virtual void AttachFile(string filePath, string description)
        {
        }

        public virtual void WriteLine(string message)
        {
        }
    }
}
