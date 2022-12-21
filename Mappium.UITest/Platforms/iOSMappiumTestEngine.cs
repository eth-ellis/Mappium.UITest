using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.iOS;
using Mappium.UITest.Configuration;

namespace Mappium.UITest.Platforms
{
    internal class iOSMappiumTestEngine : MappiumTestEngineBase<IOSDriver<IOSElement>, IOSElement>
    {
        public iOSMappiumTestEngine(UITestConfiguration config)
            : base(config)
        {
        }

        protected override IOSDriver<IOSElement> CreateDriver(AppiumOptions options, UITestConfiguration config)
        {
            // The value of DEVICE_NAME is only used for running on the iOS simulator,
            // but must also have some (any) value for iOS and Android physical devices
            AddAdditionalCapability(options, MobileCapabilityType.DeviceName, config.DeviceName);
            if (!string.IsNullOrWhiteSpace(config.AppPath))
                AddAdditionalCapability(options, MobileCapabilityType.App, config.AppPath);
            AddAdditionalCapability(options, MobileCapabilityType.PlatformName, "iOS");
            AddAdditionalCapability(options, MobileCapabilityType.PlatformVersion, config.OSVersion);
            AddAdditionalCapability(options, MobileCapabilityType.AutomationName, "XCUITest");
            AddAdditionalCapability(options, "autoAcceptAlerts", true);
            AddAdditionalCapability(options, "isHeadless", true);
            AddAdditionalCapability(options, IOSMobileCapabilityType.LaunchTimeout, 90000);
            AddAdditionalCapability(options, "appium:screenshotQuality", 2);
            AddAdditionalCapability(options, "appium:showIOSLog", true);
            AddAdditionalCapability(options, "showXcodeLog", true);
            AddAdditionalCapability(options, "deviceReadyTimeout", 30);

            // https://github.com/appium/appium/issues/12094#issuecomment-482593020
            AddAdditionalCapability(options, "wdaStartupRetries", "8");
            AddAdditionalCapability(options, "iosInstallPause","15000" );
            AddAdditionalCapability(options, "wdaStartupRetryInterval", "45000");

            return new IOSDriver<IOSElement>(config.AppiumServer, options);
        }
    }
}
