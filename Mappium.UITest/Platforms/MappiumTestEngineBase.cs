using Mappium.UITest.Configuration;
using Mappium.UITest.Enums;
using Mappium.UITest.Providers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.MultiTouch;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

namespace Mappium.UITest.Platforms
{
    internal abstract class MappiumTestEngineBase<T, E> : ITestEngine
        where T : AppiumDriver<E>
        where E : IWebElement
    {
        public const int SHORT_TIMEOUT = 5;
        public const int LONG_TIMEOUT = 30;

        private UITestConfiguration _config { get; }

        protected MappiumTestEngineBase(UITestConfiguration config)
        {
            _config = config;
            SetupDriver();
        }

        ~MappiumTestEngineBase()
        {
            Dispose(false);
        }

        private void SetupDriver()
        {
            var options = new AppiumOptions();
            ConfigureCapabilities(options);

            Driver = CreateDriver(options, _config);

            // Setup timeouts
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(SHORT_TIMEOUT);
        }

        protected abstract T CreateDriver(AppiumOptions options, UITestConfiguration config);

        protected virtual IUIElement CreateUIElement(E nativeElement) => new UIElementBase<E>(nativeElement);

        protected WebDriverWait Wait(TimeSpan? timeout = null)
        {
            return new WebDriverWait(Driver, timeout ?? TimeSpan.FromSeconds(LONG_TIMEOUT));
        }

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, string> Settings => _config.Settings;

        public void Dispose()
        {
            Dispose(true);
        }

        bool disposed;
        void Dispose (bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
                DisposeManagedResources();
            DisposeUnmanagedResources();
            disposed = true;
        }

        protected virtual void DisposeManagedResources ()
        {
            StopApp();
        }

        protected virtual void DisposeUnmanagedResources ()
        {

        }

        protected T Driver { get; private set; }

        public Platform Platform => _config.Platform;

        protected void AddAdditionalCapability(AppiumOptions options, string key, object value)
        {
            if (!_config.Capabilities.ContainsKey(key))
            {
                options.AddAdditionalCapability(key, value);
            }
        }

        private void ConfigureCapabilities(AppiumOptions options)
        {
            foreach ((string key, string value) in _config.Capabilities)
            {
                options.AddAdditionalCapability(key, value);
            }
        }

        /// <inheritdoc/>
        public virtual void StopApp()
        {
            if (Driver != null)
            {
                Driver.Quit();
                Driver = null;
                AppManager.AppStopped();
            }
        }

        /// <inheritdoc/>
        public virtual bool ElementExists(PrimarySelectorMode primarySelectorMode, string primarySelector, SecondarySelectorMode secondarySelectorMode = SecondarySelectorMode.None, string secondarySelector = null)
        {
            if (string.IsNullOrEmpty(primarySelector))
            {
                return false;
            }

            if (secondarySelectorMode != SecondarySelectorMode.None && string.IsNullOrEmpty(secondarySelector))
            {
                return false;
            }

            if (secondarySelectorMode == SecondarySelectorMode.Index && !int.TryParse(secondarySelector, out _))
            {
                return false;
            }

            IReadOnlyCollection<E> elements = new List<E>();

            switch (primarySelectorMode)
            {
                case PrimarySelectorMode.AccessibilityId:
                    elements = Driver.FindElementsByAccessibilityId(primarySelector);
                    break;

                case PrimarySelectorMode.Id:
                    elements = Driver.FindElementsById(primarySelector);
                    break;

                case PrimarySelectorMode.XPath:
                    elements = Driver.FindElementsByXPath(primarySelector);
                    break;

                case PrimarySelectorMode.ClassName:
                    elements = Driver.FindElementsByClassName(primarySelector);
                    break;

                case PrimarySelectorMode.Name:
                    elements = Driver.FindElementsByName(primarySelector);
                    break;
            }

            switch (secondarySelectorMode)
            {
                case SecondarySelectorMode.Text:
                    return elements.Any(element => element.Text == secondarySelector);

                case SecondarySelectorMode.Index:
                    return elements.ElementAtOrDefault(int.Parse(secondarySelector)) != null;

                case SecondarySelectorMode.None:
                    return elements.Any();
            }

            return false;
        }

        /// <inheritdoc/>
        public virtual IWebElement FindElement(PrimarySelectorMode primarySelectorMode, string primarySelector, SecondarySelectorMode secondarySelectorMode = SecondarySelectorMode.None, string secondarySelector = null)
        {
            if (string.IsNullOrEmpty(primarySelector))
            {
                return null;
            }

            if (secondarySelectorMode != SecondarySelectorMode.None && string.IsNullOrEmpty(secondarySelector))
            {
                return null;
            }

            if (secondarySelectorMode == SecondarySelectorMode.Index && !int.TryParse(secondarySelector, out _))
            {
                return null;
            }

            IReadOnlyCollection<E> elements = new List<E>();

            switch (primarySelectorMode)
            {
                case PrimarySelectorMode.AccessibilityId:
                    elements = Driver.FindElementsByAccessibilityId(primarySelector);
                    break;

                case PrimarySelectorMode.Id:
                    elements = Driver.FindElementsById(primarySelector);
                    break;

                case PrimarySelectorMode.XPath:
                    elements = Driver.FindElementsByXPath(primarySelector);
                    break;

                case PrimarySelectorMode.ClassName:
                    elements = Driver.FindElementsByClassName(primarySelector);
                    break;

                case PrimarySelectorMode.Name:
                    elements = Driver.FindElementsByName(primarySelector);
                    break;
            }

            switch (secondarySelectorMode)
            {
                case SecondarySelectorMode.Text:
                    return elements
                        .Where(element => element.Text == secondarySelector)
                        .FirstOrDefault();

                case SecondarySelectorMode.Index:
                    return elements
                        .ElementAtOrDefault(int.Parse(secondarySelector));

                case SecondarySelectorMode.None:
                    return elements
                        .FirstOrDefault();
            }

            return null;
        }

        /// <inheritdoc/>
        public virtual void Tap(IWebElement element)
        {         
            if (element is null)
            {
                return;
            }

            element.Click();
        }

        /// <inheritdoc/>
        public virtual void EnterText(IWebElement element, string text)
        {
            if (element is null)
            {
                return;
            }

            element.SendKeys(text);
        }

        /// <inheritdoc/>
        public virtual void SendKeysTokeyboard(string text)
        {
            Actions builder = new Actions(Driver);
            builder.SendKeys(text);
            builder.Perform();
        }

        /// <inheritdoc/>
        public virtual IUIElement WaitForElement(PrimarySelectorMode primarySelectorMode, string primarySelector, SecondarySelectorMode secondarySelectorMode = SecondarySelectorMode.None, string secondarySelector = null, TimeSpan? timeout = null)
        {
            if (string.IsNullOrEmpty(primarySelector))
            {
                return null;
            }

            if (secondarySelectorMode != SecondarySelectorMode.None && string.IsNullOrEmpty(secondarySelector))
            {
                return null;
            }

            if (secondarySelectorMode == SecondarySelectorMode.Index && !int.TryParse(secondarySelector, out _))
            {
                return null;
            }

            By query = null;

            switch (primarySelectorMode)
            {
                case PrimarySelectorMode.AccessibilityId:
                    query = MobileBy.AccessibilityId(primarySelector);
                    break;

                case PrimarySelectorMode.Id:
                    query = By.Id(primarySelector);
                    break;

                case PrimarySelectorMode.XPath:
                    query = By.XPath(primarySelector);
                    break;

                case PrimarySelectorMode.ClassName:
                    query = By.ClassName(primarySelector);
                    break;

                case PrimarySelectorMode.Name:
                    query = By.Name(primarySelector);
                    break;
            }

            Wait(timeout).Until(w =>
            {
                switch (secondarySelectorMode)
                {
                    case SecondarySelectorMode.Text:
                        return w.FindElements(query)
                            .Where(element => element.Text == secondarySelector)
                            .FirstOrDefault();

                    case SecondarySelectorMode.Index:
                        return w.FindElements(query)
                            .ElementAtOrDefault(int.Parse(secondarySelector));
                    
                    case SecondarySelectorMode.None:
                    default:
                        return w.FindElements(query)
                            .FirstOrDefault();

                }
            });

            switch (secondarySelectorMode)
            {
                case SecondarySelectorMode.Text:
                    return CreateUIElement(Driver.FindElements(query)
                        .Where(element => element.Text == secondarySelector)
                        .FirstOrDefault());

                case SecondarySelectorMode.Index:
                    return CreateUIElement(Driver.FindElements(query)
                        .ElementAtOrDefault(int.Parse(secondarySelector)));

                case SecondarySelectorMode.None:
                default:
                    return CreateUIElement(Driver.FindElements(query)
                        .FirstOrDefault());

            }
        }

        /// <inheritdoc/>
        public virtual void WaitForNoElement(PrimarySelectorMode primarySelectorMode, string primarySelector, SecondarySelectorMode secondarySelectorMode = SecondarySelectorMode.None, string secondarySelector = null, TimeSpan? timeout = null)
        {
            if (string.IsNullOrEmpty(primarySelector))
            {
                return;
            }

            if (secondarySelectorMode != SecondarySelectorMode.None && string.IsNullOrEmpty(secondarySelector))
            {
                return;
            }

            if (secondarySelectorMode == SecondarySelectorMode.Index && !int.TryParse(secondarySelector, out _))
            {
                return;
            }

            By query = null;

            switch (primarySelectorMode)
            {
                case PrimarySelectorMode.AccessibilityId:
                    query = MobileBy.AccessibilityId(primarySelector);
                    break;

                case PrimarySelectorMode.XPath:
                    query = By.XPath(primarySelector);
                    break;
            }

            switch (primarySelectorMode)
            {
                case PrimarySelectorMode.AccessibilityId:
                    query = MobileBy.AccessibilityId(primarySelector);
                    break;

                case PrimarySelectorMode.Id:
                    query = By.Id(primarySelector);
                    break;

                case PrimarySelectorMode.XPath:
                    query = By.XPath(primarySelector);
                    break;

                case PrimarySelectorMode.ClassName:
                    query = By.ClassName(primarySelector);
                    break;

                case PrimarySelectorMode.Name:
                    query = By.Name(primarySelector);
                    break;
            }

            Wait(timeout).Until(w =>
            {
                var elements = new List<IWebElement>();

                switch (secondarySelectorMode)
                {
                    case SecondarySelectorMode.Text:
                        elements = w.FindElements(query)
                            .Where(element => element.Text == secondarySelector)
                            .ToList();
                        break;

                    case SecondarySelectorMode.Index:
                        elements = w.FindElements(query).ToList();
                        break;

                    case SecondarySelectorMode.None:
                    default:
                        elements = w.FindElements(query).ToList();
                        break;

                }

                return elements.Count <= 0;
            });
        }

        /// <inheritdoc/>
        public virtual void Swipe(SwipeDirection direction = SwipeDirection.Down)
        {
            var action = new TouchAction(Driver);

            var screenWidth = Driver.Manage().Window.Size.Width;
            var screenHeight = Driver.Manage().Window.Size.Height;

            switch (direction)
            {
                case SwipeDirection.Up:
                    action
                        .Press(screenWidth / 2 - 50, screenHeight * 0.8)
                        .Wait(2000)
                        .MoveTo(screenWidth / 2 + 50, screenHeight * 0.2)
                        .Release()
                        .Perform();
                    break;
                case SwipeDirection.Down:
                    action
                        .Press(screenWidth / 2 - 50, screenHeight * 0.2)
                        .Wait(2000)
                        .MoveTo(screenWidth / 2 + 50, screenHeight * 0.8)
                        .Release()
                        .Perform();
                    break;
                case SwipeDirection.Left:
                    action
                        .Press(screenWidth * 0.8, screenHeight/ 2 - 50)
                        .Wait(2000)
                        .MoveTo(screenWidth * 0.2, screenHeight/ 2 + 50)
                        .Release()
                        .Perform();
                    break;
                case SwipeDirection.Right:
                    action
                        .Press(screenWidth * 0.2, screenHeight / 2 - 50)
                        .Wait(2000)
                        .MoveTo(screenWidth * 0.8, screenHeight / 2 + 50)
                        .Release()
                        .Perform();
                    break;
            }
        }

        /// <inheritdoc/>
        public virtual void Screenshot(string title, [CallerMemberName] string methodName = null)
        {
            var st = new StackTrace();
            var i = 1;
            MethodBase method = null;
            do
            {
                method = st.GetFrame(i++).GetMethod();
                if (method.ReflectedType.Assembly == GetType().Assembly)
                    method = null;
            } while (method is null);

            var className = method.ReflectedType.Name;
            var namespaceName = method.ReflectedType.Namespace;

            var baseDir = _config.ScreenshotsPath;

            var newFile = string.IsNullOrEmpty(title) ? $"{methodName}.png" : $"{methodName}-{title}.png";
            var newDir = Path.Combine(baseDir, Driver.SessionId.ToString(), namespaceName, className);
            var fullPath = Path.Combine(newDir, newFile);
            Directory.CreateDirectory(newDir);

            var s = Driver.GetScreenshot();
            s.SaveAsFile(fullPath, ScreenshotImageFormat.Png);
            TestFrameworkProvider.AttachFile(fullPath, Path.GetFileNameWithoutExtension(newFile));
        }

        /// <inheritdoc/>
        public virtual void DismissKeyboard() =>
            Driver.HideKeyboard();

        /// <inheritdoc/>
        public virtual void BackButton() =>
            Driver.Navigate().Back();

        public virtual List<string> GetDriverContexts()
        {
            List<string> AllContexts = new List<string>();
            
            foreach (var context in (Driver.Contexts))
            {
                AllContexts.Add(context);
            }

            return AllContexts;
        }

        public virtual void SetDriverContext(string context)
        {
            Driver.Context = context;
        }
    }
}
