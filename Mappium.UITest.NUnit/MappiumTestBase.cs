using NUnit.Framework;

namespace Mappium.UITest.NUnit
{
    [NonParallelizable]
    public class MappiumTestBase
    {
        protected ITestEngine Engine { get; private set; }

        [SetUp]
        public virtual void StartApp()
        {
            AppManager.StartApp();
            Engine = AppManager.Engine;
        }

        [TearDown]
        public virtual void CloseApp()
        {
            Engine.StopApp();
        }
    }
}
