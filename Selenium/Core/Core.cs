using OpenQA.Selenium;
using System.Configuration;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Firefox;
using System.Threading;
using OpenQA.Selenium.Interactions;
using Selenium.Properties;
using System;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Appium.Android;
using System.IO;
using System.Linq;

namespace Selenium
{
    public class Core
    {
        private static IWebDriver webDriver;
        private static string baseURL = ConfigurationManager.AppSettings["url"];
        private static string browser = ConfigurationManager.AppSettings["browser"];
        public static Dictionary<string, string> dictProfiles = new Dictionary<string, string>();
        public static Dictionary<string, string> OR = new Dictionary<string, string>();
        public const string MC_SERVER = "http://127.0.0.1:8080";
    
        
 public static void PopulateDictionary(string path)
        {
            string[] txtFileLines = File.ReadAllLines(path);
            foreach (var line in txtFileLines)
            {
                string[] str = line.Split(',');
                if (txtFileLines.Contains(Core.dictProfiles[str[0]] = str[1]))
                {
                    dictProfiles.Add(str[0], str[1]);

                }
                else
                {
                    Core.dictProfiles[str[0]] = str[1];
                    continue;
                }
            }

        }
        public static void PopulateOR(string path)
        {
            string[] txtFileLines = File.ReadAllLines(path);
            foreach (var line in txtFileLines)
            {
                string[] str = line.Split(',');
                if (txtFileLines.Contains(Core.OR[str[0]] = str[1]))
                {
                    OR.Add(str[0], str[1]);

                }
                else
                {
                    Core.OR[str[0]] = str[1];
                    continue;
                }
            }

        }
        public static void StartUp()
        {
            if (Device.TestPhone.Equals("Sony"))
            {
                PopulateDictionary(@"AndroidDeviceApps/Sony.txt");
              //  PopulateOR(@"EnvtestOR/LIVE PROD OR.txt");
                //PopulateOR(@"Envtestdata/OR.txt");
            }
            else 
            {
                PopulateDictionary(@"AndroidDeviceApps/Other.txt");
                // PopulateOR(@"EnvtestOR/LIVE PROD OR.txt");
                //PopulateOR(@"EnvtestOR/PRODA External OR.txt");
            }


           
        }
        public static void Init(string url)
        {
            switch (browser)
            {
                case "Chrome":
                    webDriver = new ChromeDriver();
                    break;
                case "IE":
                    webDriver = new InternetExplorerDriver();
                    break;
                case "Firefox":
                    webDriver = new FirefoxDriver();
                    break;
            }

            webDriver.Manage().Window.Maximize();
            Goto(url);
        }

        #region Getters and Setters
        public static void Goto(string url) { webDriver.Url = url; }

        public static string Title { get { return webDriver.Title; } }
        public static IWebDriver getDriver { get { return webDriver; } }
        public static string getBrowser { get { return browser; } }
        public static string getURL { get { return baseURL; } }
        #endregion

        public static void Close()
        {
            Reports.closeReport();
            webDriver.Quit();
        }
        

        public static void Click(string Element)
        {
            var element = By.XPath(Element);
            webDriver.FindElement(element).Click();
            webDriver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);
        }

        public static void EnterValue(string Element, string value)
        {
            var element = By.XPath(Element);

            webDriver.FindElement(element).Clear();
            webDriver.FindElement(element).SendKeys(value);
          
        }

        public static void MoveToElement(string Element)
        {
            Actions actions = new Actions(webDriver);

            actions.MoveToElement(webDriver.FindElement(By.XPath(Element)));
            actions.Perform();
        }

        public static void Performlogin(string userName, string passwrd, string testname, ReportsManager reports)
        {
            Thread.Sleep(5000);

            Click(EnvtestOR.Login);

           
            reports.verifyElement(EnvtestOR.UsernameInput);
            reports.verifyElement(EnvtestOR.PasswordInput);
            reports.verifyElement(EnvtestOR.LoginButton);
            reports.Screenshot(webDriver, "Verified Log In", testname);

            EnterValue(EnvtestOR.UsernameInput, userName);
            EnterValue(EnvtestOR.PasswordInput, passwrd);

            Click(EnvtestOR.LoginButton);

            Thread.Sleep(5000);
            reports.verifyElement(EnvtestOR.Logout);
        }
        public static void PerformloginOtp(string userName, string passwrd, string testname, ReportsManager reports)
        {
            Thread.Sleep(5000);
            StartUp();
            Click(EnvtestOR.Login);

            reports.verifyElement(EnvtestOR.UsernameInput);
            reports.verifyElement(EnvtestOR.LoginButton);
            reports.Screenshot(webDriver, "LogIn page", testname);

            EnterValue(EnvtestOR.UsernameInput, userName);
            webDriver.FindElement(By.XPath("//a[contains(.,'Use OTP')]")).Click();
            Thread.Sleep(3000);
            reports.Screenshot(webDriver, "Requesting OTP", testname);

            var value = Core.ReadMessege(reports, testname);

            Thread.Sleep(3000);
            //Move to OTPLogin button
            MoveToElement("//a[@class='btn btnPurple login-otp']");
            webDriver.FindElement(By.XPath("//input[@name='otp']")).SendKeys(value);

            reports.Screenshot(webDriver, "OTP Entered", testname);      
            //loginOTP button
            Click("//a[@class='btn btnPurple login-otp']");

            Thread.Sleep(5000);
            reports.Screenshot(webDriver, "Succesfully logged-in", testname);

            
        }
        public static string ReadMessege(ReportsManager reports,string testname)
        {
            // Set the capabilities instance
            DesiredCapabilities capabilities = new DesiredCapabilities();
            capabilities.SetCapability("deviceName", "3a2bad9");
          
            // Set device capabilities
            capabilities.SetCapability("PlatformName", "Android");
            // Set application capabilities
            //SMS     
            capabilities.SetCapability("appPackage", dictProfiles["MessegingAppPckg"]);
            capabilities.SetCapability("appActivity", dictProfiles["MessegingAppActivty"]);
                         
            // Create new Appium driver pointing to the MC server and based on the capabilities set above.
            AndroidDriver<AppiumWebElement> Phone = new AndroidDriver<AppiumWebElement>(new Uri(MC_SERVER + "/wd/hub"), capabilities, TimeSpan.FromMinutes(10));
       
            // Create a wait object instance
            WebDriverWait waitController = new WebDriverWait(Phone, TimeSpan.FromSeconds(30));
            MobileCore mobile = new MobileCore();

            try
            {
                mobile.MobileFindElement(waitController, "TextView", "text", "Write new message");
                mobile.MobileClicks(waitController, "ImageButton", "content-desc", "Navigate up");
            }
            catch { }

            mobile.MobileFindElement(waitController,"TextView","text","Messaging");

            var i = Phone.FindElement(By.XPath("//android.widget.TextView[@resource-id='"+Sony.sms_resID+"']")).Text.Substring(47);
               
            //com.android.mms:id / subject

            Phone.FindElement(By.XPath("//android.widget.TextView[@resource-id='" + Sony.sms_resID + "']")).Click();
            Thread.Sleep(3000);

           //getBrowser the sms sent
            reports.Screenshot(Phone,"Sms sent to phone", testname);
            Console.WriteLine(i);
            Console.WriteLine(dictProfiles["MessegingAppPckg"]);
            Console.WriteLine(dictProfiles["MessegingAppActivty"]);

            return i;
        }
   
        public static void Logout(ReportsManager reports, string testname)
        {
            MoveToElement(EnvtestOR.Logout);

            reports.verifyElement(EnvtestOR.Logout);
            Click(EnvtestOR.Logout);

            reports.verifyElement(EnvtestOR.Login);
            reports.verifyElement(Message.LogoutMsg);
           // reports.Screenshot("Logout Successful", testname);
        }
    }
}
