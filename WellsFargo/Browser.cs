using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using System.Configuration;
using System.IO;

namespace WellsFargo
{
    public class Browser
    {
       public enum browserType { chrome, Firefox, IE }
        private static IWebDriver driver;
        public Browser(browserType type)
        {
            switch (type)
            {
                case browserType.chrome:
                    driver = new ChromeDriver();
                    break;
                case browserType.Firefox:
                    //FirefoxDriverService service = FirefoxDriverService.CreateDefaultService("C:\\Selenium", "geckodriver.exe");
                    //service.FirefoxBinaryPath = "C:\\Program Files (x86)\\Mozilla Firefox\\firefox.exe";
                    driver = new FirefoxDriver();
                    break;
                case browserType.IE:
                    driver = new InternetExplorerDriver();
                    break;
                default:
                    driver = new ChromeDriver();
                    break;
            }
        }

        public static string BrowserName()
        {
            return ((RemoteWebDriver)driver).Capabilities.BrowserName;
        }

        public class browserWait
        {
            public browserWait()
            {
                new WebDriverWait(driver, TimeSpan.FromSeconds(config.TIMEOUT));
            }
        }

        public browserWait wait()
        {
            return new browserWait();
        }

        public void Quit()
        {
            driver.Quit();
        }
        public static void TakeScreenshot(string callingMethod)
        {
            TakeScreenshot(Validate.driver, Validate.GetCurrentDateTime("HHmmssfff"), callingMethod);
        }
        public static void TakeScreenshot(IWebDriver driver, string fileappend, string callingMethod)
        {
            if (ConfigurationManager.AppSettings.Get("capture_screenshots") == "true")
            {
                try
                {
                    string pathString = @"C:\Selenium\Screenshots\" + Validate.GetCurrentDateTime("yyyyMMdd") + @"\" + config.Validatename;
                    if (!File.Exists(pathString))
                    {
                        Directory.CreateDirectory(pathString);
                    }
                    Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
                    ss.SaveAsFile(pathString + @"\" + callingMethod + fileappend + ".jpg", ScreenshotImageFormat.Jpeg);
                    report.Action("Screenshot saved", pathString);
                }

                catch (Exception e)
                {
                    Console.WriteLine("Unable to capture screenshot: " + e.Message);
                }
            }
        }
    }
}
