using static Mappium.UITest.Providers.TestFrameworkProvider;

namespace Mappium.UITest.Pages
{
    /// <summary>
    /// TODO
    /// </summary>
    public class BasePage
    {
        /// <summary>
        /// TODO
        /// </summary>
        protected ITestEngine Engine => AppManager.Engine;

        /// <summary>
        /// Gets whether the current platform is Android
        /// </summary>
        protected bool OnAndroid => AppManager.Platform == Platform.Android;

        /// <summary>
        /// Gets whether the current platform is iOS
        /// </summary>
        protected bool OniOS => AppManager.Platform == Platform.iOS;

        /// <summary>
        /// The element used to determine whether on a page
        /// </summary>
        protected virtual string Trait { get; }

        /// <summary>
        /// The page name
        /// </summary>
        protected virtual string PageName { get; }

        /// <summary>
        /// TODO
        /// </summary>
        protected BasePage()
        {
            PageName = GetType().Name;

            AssertOnPage();
        }

        /// <summary>
        /// Verifies that the trait is present.
        /// </summary>
        protected virtual void AssertOnPage()
        {
            var message = "Unable to verify on page: " + PageName;

            AssertDoesNotThrowAndIsNotNull(() => Engine.WaitForElement(Enums.PrimarySelectorMode.AccessibilityId, Trait), message);
        }
    }
}
