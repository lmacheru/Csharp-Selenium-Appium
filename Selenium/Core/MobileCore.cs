using OpenQA.Selenium;
using NUnit.Framework;
using System.Threading;
using System.IO;
using OpenQA.Selenium.Support.UI;
using System;
using System.Reflection;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium;

namespace Selenium
{
    public class MobileCore
    {
        static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
        static String testname = MethodBase.GetCurrentMethod().Name;

        public static IWebDriver driver;
        public const string MC_SERVER = "http://127.0.0.1:8080";
        public ReportsManager reports = new ReportsManager(Core.getBrowser, Core.getURL, className, testname);
        int nbr = 1;

       public void SwipeVertical(AndroidDriver<AppiumWebElement> Phone,string direction)
        {
         
            int size = Phone.Manage().Window.Size.Height;
            Console.WriteLine(size);

            //Find swipe start and end point from screen's with and height.
            //Find starty point which is at bottom side of screen.
            int starty = (int)(size * 0.80);
            //Find endy point which is at top side of screen.
            int endy = (int)(size * 0.20);
            //Find horizontal point where you wants to swipe. It is in middle of screen width.
            int startx = size / 2;
            Console.WriteLine("starty = " + starty + " ,endy = " + endy + " , startx = " + startx);

            if (direction.Equals("Down"))
            {
                Thread.Sleep(2000);

                //Swipe from Bottom to Top.
                Phone.Swipe(startx, starty, startx, endy, 3000);
                reports.addLog("Swipping " + direction, "PASS", "Swipped " + direction);

                Thread.Sleep(3000);

            }
            else
                if (direction.Equals("Top"))
            {
                //Swipe from Top to Bottom.
                Thread.Sleep(2000);

                Phone.Swipe(startx, endy, startx, starty, 3000);
                reports.addLog("Swipping " + direction, "PASS", "Swipped " + direction);

                Thread.Sleep(2000);
            }

        }
       public void SwipeHorizontal(AndroidDriver<AppiumWebElement> Phone, string direction)
        {
            int size = Phone.Manage().Window.Size.Width;

            //Find swipe start and end point from screen's with and height.
            //Find startx point which is at right side of screen.
            int startx = (int)(size * 0.70);
            //Find endx point which is at left side of screen.
            int endx = (int)(size * 0.30);
            //Find vertical point where you wants to swipe. It is in middle of screen height.
            int starty = size / 2;
            if (direction.Equals("Right"))
            {
                //Swipe from Right to Left.
                Phone.Swipe(startx, starty, endx, starty, 3000);
                reports.addLog("Swipping " + direction, "PASS", "Swipped " +direction );

                Thread.Sleep(2000);

            }
            else
               if (direction.Equals("Left"))
            {
                //Swipe from Left to Right.
                Phone.Swipe(endx, starty, startx, starty, 3000);
                reports.addLog("Swipping " + direction, "PASS", "Swipped " + direction);

                Thread.Sleep(2000);
            }
        }

        public void MobileFindElement(WebDriverWait Controller, string tag, string style, string ElementName)
        {
            try
            {
                
                    Controller.Until(x => x.FindElement(By.XPath("//android.widget." + tag + "[@" + style + "='" + ElementName + "']")));
                    reports.addLog("Looking For " + tag + " " + ElementName + "", "PASS", ElementName + " is found");           
            }
            catch
            {
                string message = "Element does not exist!";
                reports.addLog("Looking For " + tag + " " + ElementName + "", "FAIL", ElementName + " not found");

                Assert.Fail(message);

            }
        }
     
        public void MobileClicks(WebDriverWait Controller, string tag,string style, string ElementName)
        {
            Thread.Sleep(4000);
            try
            {
                    Controller.Until(x => x.FindElement(By.XPath("//android.widget." + tag + "[@" + style + "='" + ElementName + "']"))).Click();
                    reports.addLog("Looking For " + tag + " " + ElementName + "", "PASS", ElementName + " is found and clicked");

            }
            catch
            {
                string message = "Element does not exist!";
                reports.addLog("Looking For " + tag + " " + ElementName + "", "FAIL", ElementName + " not found");

                Assert.Fail(message);

            }
        }
        public void MobileSendKey(WebDriverWait Controller, string ElementName, string value)
        {
            try
            {
                Controller.Until(x => x.FindElement(By.XPath("//android.widget.EditText" + "[@content-desc='" + ElementName + "']"))).SendKeys(value);
                reports.addLog("Looking For EditText" + " " + ElementName + "", "PASS", ElementName + " is found");
                reports.addLog("Sending "+value+ " to " + ElementName + "", "PASS", value + " Sent");
            }
            catch
            {
                string message = "Element does not exist!";
                reports.addLog("Looking For EditText "+ ElementName + "", "FAIL", ElementName + " not found");
                Assert.Fail(message);

            }
        }
        public void MobilePopUp(WebDriverWait Controller, string tag, string style, string ElementName)
        {
            try
            {
                if (tag == "Button" || tag == "ImageButton")
                {
                    Controller.Until(x => x.FindElement(By.XPath("//android.widget." + tag + "[@" + style + "='" + ElementName + "']"))).Click();
                    reports.addLog("Looking For " + tag + " " + ElementName + "", "PASS", ElementName + " is found");
                }
                else
                if (tag == "TextView" || tag == "Switch")
                {

                    Controller.Until(x => x.FindElement(By.XPath("//android.widget." + tag + "[@" + style + "='" + ElementName + "']"))).Click();
                    reports.addLog("Looking For " + tag + " " + ElementName + "", "PASS", ElementName + " is found");


                }
            }
            catch
            {
                string message = "Element did not Appear!";
                reports.addLog("Looking For " + tag + " " + ElementName + "", "Skipped", ElementName + " "+message);

            }
        }

        #region Screenshots
        public void Screenshot(IWebDriver type, string StepName, string testname)
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


    }
}
