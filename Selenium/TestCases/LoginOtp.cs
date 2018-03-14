using OpenQA.Selenium;
using NUnit.Framework;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using System;
using System.Reflection;
using Selenium.Properties;
using OpenQA.Selenium.Interactions;

//Web Drivers
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System.Configuration;

namespace Selenium
{
    [TestFixture]
    public class LoginOtp
    {
        IWebDriver driver;
        ReportsManager reports;
        string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
        WebDriverWait wait;

        [SetUp]
        public void startTest()
        {
            Core.Init(ConfigurationManager.AppSettings["url"]);
            driver = Core.getDriver;
        }

        [TearDown]
        public void endTest()
        {
        }

        [Test]
        public void VerifyBalanceTest()
        {
            String testname = MethodBase.GetCurrentMethod().Name;
            Actions actions = new Actions(driver);
            wait = new WebDriverWait(driver, timeout: TimeSpan.FromSeconds(30));
            reports = new ReportsManager(Core.getBrowser, Core.getURL, className, testname);
            reports.verifyURL(Core.getURL);

            //Retrieve opt from cellphone
            Core.PerformloginOtp(Envtestdata.Tester, Envtestdata.GlobalPassword, testname, reports);

            //verifyBalance(testname);
            Core.Close();
          

        }

        public void verifyBalance(string testname)
        {
            reports.verifyElement(EnvtestOR.ManageMyAccount);
            reports.Screenshot(driver,"Account Overview", testname);
            Core.Click(EnvtestOR.ManageMyAccount);

            Thread.Sleep(10000);
            wait.Until(x => ((IJavaScriptExecutor)x).ExecuteScript("return document.readyState").Equals("complete"));

            try
            {
                Thread.Sleep(3000);
                //Core.MoveToElement(EnvtestOR.Airtime);
                reports.verifyElement(EnvtestOR.Airtime);
                reports.Screenshot(driver,"Verify Airtime", testname);
            }
            catch
            {
                Thread.Sleep(15000);
                Core.MoveToElement(EnvtestOR.Billsofar);
                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(EnvtestOR.Billsofar)));
                reports.verifyElement(EnvtestOR.Billsofar);
                reports.Screenshot(driver,"Verify Bill so far", testname);
            }
        }
    }
}
