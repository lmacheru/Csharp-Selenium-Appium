using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using NUnit.Framework;
using System.Threading;
using System.IO;

namespace Selenium
{
    public class ReportsManager
    {
        Reports report;
        string browser;
        string url;
        IWebDriver driver = Core.getDriver;
        public static double nbr;
        public ReportsManager(string browser, string url, string className, string testName)
        {
            this.browser = browser;
            this.url = url;
            report = new Reports(browser, url, className, testName);
            nbr = 1;
        }

        #region Screenshots
        public void Screenshot(IWebDriver type,string StepName, string testname)
        {
            string str = nbr + ") " + StepName + ".png";

            Screenshot ss = ((ITakesScreenshot)type).GetScreenshot();
            ss.SaveAsFile(str, ScreenshotImageFormat.Png);

            nbr += 1;
            ScreenMove(testname, str);
        }
       

        public static void MoveFiles(string fromPath, string toPath, string className, String str)
        {
            string[] files = System.IO.Directory.GetFiles(fromPath, str);//"*.png"
            string currentDateFolder = DateTime.Now.ToLongDateString();

            string tmStamp = DateTime.Now.ToString("hh-mm-ss tt");//For whole Date and Time, change to DateTime.Now.ToString("yyyyMMddHHmmssfff")
                                                                  // string fname = "Scr"
                                                                  // string toPath1 = Directory.CreateDirectory(toPath + "\\" + fname);
            DirectoryInfo dirInfo = null;
            if (!Directory.Exists(toPath + "\\" + currentDateFolder))
            {
                dirInfo = Directory.CreateDirectory(toPath + "\\" + currentDateFolder);
                // You can use this info if you need it  
            }
            else
            {
                dirInfo = new DirectoryInfo(toPath + "\\" + currentDateFolder);
            }

            if (dirInfo != null)
            {
                dirInfo = dirInfo.CreateSubdirectory(className);//+ " " + tmStamp
            }

            var newPath = dirInfo.FullName;
            foreach (var file in files)
            {
                File.Move(file, newPath + "\\" + Path.GetFileName(file));
            }
        }

        public static void ScreenMove(String testname, String str)
        {
            String appScreen = "Screenshot ";
            String machineName = System.Environment.MachineName;
            String networkShareLocation = @"\\10.100.6.111\d$\Screen\";

            try
            {
                MoveFiles(".", networkShareLocation + appScreen + machineName, testname, str);
            }
            catch
            {
                MoveFiles(".", ".\\" + appScreen, testname, str);
            }
        }

        #endregion Screenshots

        public void verifyURL(string url)
        {
            string PageURL = driver.Url;
            string message = "The Current Url and Expected Url are not equal";

            if (PageURL.Equals(url))
                report.addLine("Verify url", "PASS", "Url are Equals");
            else
                report.addLine("Verify url", "FAIL", message);

            Assert.AreEqual(PageURL, url, message);
        }

        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void verifyElement(string Element)
        {
            var element = By.XPath(Element);
            string elementHtml;
      

            Thread.Sleep(7000);
            if (!IsElementPresent(element))
            {
                string message = "Element does not exist!";
                addLog("Looking For Element " + element.ToString().Trim() + "", "FAIL", element.ToString().Trim() + " Should Be On The Page");
                Assert.Fail(message);
            }
            else
            {
                try
                {
                    elementHtml = driver.FindElement(By.XPath(Element)).GetAttribute("name").Trim().ToString();
                    if (elementHtml == "")
                        Assert.Fail();
                }
                catch
                {
                    elementHtml = driver.FindElement(element).GetAttribute("innerHTML").Trim().ToString();
                }
             
                    addLog("Looking For Element " + elementHtml + "", "PASS", elementHtml + " Found");
               
            }
        }

        public void addLog(string description, string result, string exception)
        {
            report.addLine(description, result, exception);
        }
    }
}
