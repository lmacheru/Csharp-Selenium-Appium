using OpenQA.Selenium;
using NUnit.Framework;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using System;
using System.Reflection;
using Selenium.Properties;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Appium.Android;

namespace Selenium
{
    [TestFixture]
    public class USSD_Dialer
    {
        string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
        public const string MC_SERVER = "http://0.0.0.0:4723";
        ReportsManager reports;

        [Test]
        public void USSD_Test()
        {
            try
            {
                String testname = MethodBase.GetCurrentMethod().Name;
                reports = new ReportsManager(Core.getBrowser, Core.getURL, className, testname);
                //   reports.verifyURL(Core.getURL);

                // Set the capabilities instance
                DesiredCapabilities capabilities = new DesiredCapabilities();
                capabilities.SetCapability("deviceName", Device.Emulator);

                // Set device capabilities
                capabilities.SetCapability("PlatformName", "Android");

                //voda app
                capabilities.SetCapability("appPackage","com.android.dialer");
                capabilities.SetCapability("appActivity","com.android.dialer.DialtactsActivity");

                // Create new Appium driver pointing to the MC server and based on the capabilities set above.
                AndroidDriver<AppiumWebElement> Phone = new AndroidDriver<AppiumWebElement>(new Uri(MC_SERVER + "/wd/hub"), capabilities, TimeSpan.FromMinutes(10));

                // Create a wait object instance
                WebDriverWait waitController = new WebDriverWait(Phone, TimeSpan.FromSeconds(30));

                Thread.Sleep(3000);

                //Dialing *111# on a mobile device
                reports.Screenshot(Phone,"Dialer Opened", testname);
                Phone.FindElement(By.Id("star")).Click();
                Phone.FindElement(By.Id("one")).Click();
                Phone.FindElement(By.Id("one")).Click();
                Phone.FindElement(By.Id("one")).Click();
                Phone.FindElement(By.Id("pound")).Click();

                //Press Call button
                reports.Screenshot(Phone,"Dialed *111#", testname);
                Phone.FindElement(By.Id("dialButton")).Click();
                //Dual Sim choose profile sim
                try
                {
                    Phone.FindElement(By.Id("select_account_list_item_view")).Click();
                }
                catch
                {
                   //if you dont have non Dual-sim it will skip this part
                }
                reports.Screenshot(Phone, "Dialer opened", testname);
            }
            finally
            {
                // Core.Close();
            }
        }

    }
}
