using System;
using System.Collections.Generic;
using System.Linq;

namespace Mappium.UITest.Providers
{
    internal static class TestFrameworkProvider
    {
        #region Private Definitions

        private static readonly Dictionary<string, ITestFramework> Frameworks = new Dictionary<string, ITestFramework>(StringComparer.OrdinalIgnoreCase)
        {
            ["mspec"] = new MSpecFramework(),
            ["nspec3"] = new NSpecFramework(),
            ["nunit"] = new NUnitTestFramework(),
            ["mstestv2"] = new MSTestFrameworkV2(),
            ["xunit2"] = new XUnit2TestFramework()
        };

        private static ITestFramework testFramework;

        #endregion

        public static void AssertDoesNotThrow(Action action, string message = null)
        {
            try
            {
                action?.Invoke();
            }
            catch(Exception ex)
            {
                Throw(message ?? $"Expected condition would not throw exception. Got: {ex.GetType().Name}");
            }
        }

        public static void AssertDoesNotThrowAndIsNotNull<T>(Func<T> func, string message = null)
            where T : class
        {
            T returnItem = null;
            AssertDoesNotThrow(() =>
            {
                returnItem = func.Invoke();
            }, message);
            AssertNotNull(returnItem, message);
        }

        public static void AssertDoesNotThrowAndIsNull<T>(Func<T> func, string message = null)
            where T : class
        {
            T returnItem = null;
            AssertDoesNotThrow(() =>
            {
                returnItem = func.Invoke();
            }, message);
            AssertIsNull(returnItem, message);
        }

        public static void AssertNotNull(object element, string message = null)
        {
            if (element is null)
                Throw(message ?? "Expected object would not be null.");
        }

        public static void AssertIsNull(object element, string message = null)
        {
            if (element != null)
                Throw(message ?? "Expected object would be null.");
        }

        public static void Throw(string message)
        {
            if (testFramework is null)
            {
                testFramework = DetectFramework();
            }

            testFramework.Throw(message);
        }

        public static void AttachFile(string filePath, string description)
        {
            if (testFramework is null)
            {
                testFramework = DetectFramework();
            }

            testFramework.AttachFile(filePath, description);
        }

        private static ITestFramework DetectFramework()
        {
            return AttemptToDetectUsingDynamicScanning() ?? new FallbackTestFramework();
        }

        private static ITestFramework AttemptToDetectUsingDynamicScanning()
        {
            return Frameworks.Values.FirstOrDefault(framework => framework.IsAvailable);
        }
    }
}
