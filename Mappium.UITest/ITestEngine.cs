using Mappium.UITest.Enums;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mappium.UITest
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface ITestEngine : IDisposable
    {
        /// <summary>
        /// TODO
        /// </summary>
        Platform Platform { get; }

        /// <summary>
        /// Returns Settings provided by your configuration to help with testing
        /// </summary>
        /// <remarks>
        /// You may use this to provide values such as user credentials
        /// </remarks>
        IReadOnlyDictionary<string, string> Settings { get; }

        /// <summary>
        /// TODO
        /// </summary>
        void StopApp();

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="primarySelectorMode"></param>
        /// <param name="primarySelector"></param>
        /// <param name="secondarySelectorMode"></param>
        /// <param name="secondarySelector"></param>
        /// <returns></returns>
        bool ElementExists(PrimarySelectorMode primarySelectorMode, string primarySelector, SecondarySelectorMode secondarySelectorMode = SecondarySelectorMode.None, string secondarySelector = null);

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="primarySelectorMode"></param>
        /// <param name="primarySelector"></param>
        /// <param name="secondarySelectorMode"></param>
        /// <param name="secondarySelector"></param>
        /// <returns></returns>
        IWebElement FindElement(PrimarySelectorMode primarySelectorMode, string primarySelector, SecondarySelectorMode secondarySelectorMode = SecondarySelectorMode.None, string secondarySelector = null);

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="element">The element to interact with</param>
        void Tap(IWebElement element);

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="element">The element to interact with</param>
        /// <param name="text">The text to be entered</param>
        void EnterText(IWebElement element, string text);

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="text"></param>
        void SendKeysTokeyboard(string text);

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="primarySelectorMode"></param>
        /// <param name="primarySelector"></param>
        /// <param name="secondarySelectorMode"></param>
        /// <param name="secondarySelector"></param>
        /// <param name="timeout"></param>
        IUIElement WaitForElement(PrimarySelectorMode primarySelectorMode, string primarySelector, SecondarySelectorMode secondarySelectorMode = SecondarySelectorMode.None, string secondarySelector = null, TimeSpan? timeout = null);

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="primarySelectorMode"></param>
        /// <param name="primarySelector"></param>
        /// <param name="secondarySelectorMode"></param>
        /// <param name="secondarySelector"></param>
        /// <param name="timeout"></param>
        void WaitForNoElement(PrimarySelectorMode primarySelectorMode, string primarySelector, SecondarySelectorMode secondarySelectorMode = SecondarySelectorMode.None, string secondarySelector = null, TimeSpan? timeout = null);
        
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="direction"></param>
        void Swipe(SwipeDirection direction);

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="title"></param>
        /// <param name="methodName"></param>
        void Screenshot(string title, [CallerMemberName] string methodName = null);

        /// <summary>
        /// TODO
        /// </summary>
        void DismissKeyboard();

        /// <summary>
        /// TODO
        /// </summary>
        void BackButton();

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        List<string> GetDriverContexts();

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="context"></param>
        void SetDriverContext(string context);
    }
}
