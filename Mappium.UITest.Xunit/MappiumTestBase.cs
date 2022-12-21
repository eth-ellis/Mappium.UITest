using System;
using Xunit;

namespace Mappium.UITest
{
    [Collection(nameof(MappiumTest))]
    public class MappiumTestBase : IDisposable
    {
        protected ITestEngine Engine { get; }

        public MappiumTestBase()
        {
            AppManager.StartApp();
            Engine = AppManager.Engine;
        }

        ~MappiumTestBase ()
        {
            Dispose(false);
        }

        public virtual void Dispose()
        {
            Dispose(true);
        }

        bool disposed;
        void Dispose (bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                    DisposeManagedResources();
                DisposeUnmanagedResources();
                disposed = false;
            }
        }

        protected virtual void DisposeManagedResources ()
        {
            Engine.StopApp();
        }

        protected virtual void DisposeUnmanagedResources ()
        {

        }
    }
}
