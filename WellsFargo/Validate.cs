using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net.Mail;
using System.Reflection;
//using SingleUserPerformance.Data;
using System.Diagnostics;
using System.IO;

namespace WellsFargo
{
    public class Validate
    {
        public static IWebDriver driver;
        public static testVars vars;
        public static IWebDriver altdriver;

        public static string env = "QA";
        public static string url = ConfigurationManager.AppSettings.Get("url");
        public static string username = ConfigurationManager.AppSettings.Get("username");
        public static string password = ConfigurationManager.AppSettings.Get("password");

        public static void ScrollDown(IWebDriver driver)
        {
            var js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
        }

        public static void ScrollUp(IWebDriver driver)
        {
            var js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("window.scrollTo(0, 0);");
        }

        public static void ScrollTo(IWebDriver driver, By by)
        {
            IWebElement element = driver.FindElement(by);
            ScrollTo(driver, element);
        }

        public static void ScrollTo(IWebDriver driver, IWebElement element)
        {
            var js = (IJavaScriptExecutor)driver;
            js.ExecuteScript(string.Format("window.scrollTo(0, {0});", element.Location.Y - 200));  //offset for banner

            //scroll md-content if it exists
            js.ExecuteScript("if(window.document.getElementsByTagName('md-content')[3]) {window.document.getElementsByTagName('md-content')[3].scrollTop=0;}");
            string y = string.Format("{0}", element.Location.Y);
            js.ExecuteScript("if(window.document.getElementsByTagName('md-content')[3]) {window.document.getElementsByTagName('md-content')[3].scrollTop=" + y + ";}");
        }

        public static void WaitForChrome(IWebDriver driver, int duration)
        {
            string browsername = ((RemoteWebDriver)driver).Capabilities.BrowserName;
            if (browsername.Equals("chrome")) { System.Threading.Thread.Sleep(duration); }
        }

        public static void WaitForFF(IWebDriver driver, int duration)
        {
            string browsername = ((RemoteWebDriver)driver).Capabilities.BrowserName;
            if (browsername.Equals("firefox")) { System.Threading.Thread.Sleep(duration); }
        }

        public static void WaitForIE(IWebDriver driver, int duration)
        {
            string browsername = ((RemoteWebDriver)driver).Capabilities.BrowserName;
            if (browsername.Equals("internet explorer")) { System.Threading.Thread.Sleep(duration); }
        }

        public static void WaitForElement(By by)
        {
            WaitForElement(driver, by);
        }

        public static void WaitForElement(IWebDriver driver, By by)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(config.TIMEOUT));

            wait.Until(d => (d.FindElements(by)).Count > 0);

        }
        public static void WaitForElement(By by, int timeout)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));

            wait.Until(d => (d.FindElements(by)).Count > 0);

        }


        public static void WaitForElementNotPresent(By by)
        {
            WaitForElementNotPresent(driver, by);
        }

        public static void WaitForElementNotPresent(IWebDriver driver, By by)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(config.TIMEOUT));

            wait.Until(d => (d.FindElements(by)).Count == 0);

        }

        public static void WaitForElementClickable(By by)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(config.TIMEOUT));

            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(by));

        }

        public static void WaitForTextInElement(By by)
        {
            WaitForTextInElement(by, config.TIMEOUT);
        }

        public static void WaitForTextInElement(By by, int seconds)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds));
            wait.Until(d => (d.FindElements(by)).Count > 0);
            wait.Until(d => (d.FindElement(by).GetAttribute("value").Length > 0));
        }

        public static void WaitForTextInElement(By by, string text)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(config.TIMEOUT));
            wait.Until(d => (d.FindElements(by)).Count > 0);
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElementValue(driver.FindElement(by), text));
        }

        public static int VerifyPageSourceText(IWebDriver driver, string text)
        {
            return VerifyPageSourceText(driver, text, false);
        }
        public static int VerifyPageSourceText(IWebDriver driver, string text, bool stopTest)
        {
            try
            {
                try
                {
                    WaitForPageToLoad(driver);
                    Assert.AreEqual(true, driver.PageSource.Contains(text));
                    report.Pass("Page source contains:  " + text);
                    return 0;
                }
                catch (Exception e)
                {
                    report.Fail("Page source does not contain:  " + text, e);
                    stopTestExecution(stopTest);
                    return 1;
                }
            }
            finally { }
        }

        public static int VerifyEditFieldText(IWebDriver driver, By by, string attribute, string text)
        {
            return VerifyEditFieldText(driver, by, attribute, text, false);
        }
        public static int VerifyEditFieldText(IWebDriver driver, By by, string attribute, string text, bool stopTest)
        {
            try
            {
                WaitForPageToLoad(driver);
                Assert.AreEqual(text, driver.FindElement(by).GetAttribute(attribute));
                report.Pass("Element (" + by + ") " + attribute + " attribute equals:  " + text);
                return 0;
            }
            catch (Exception e)
            {
                report.Fail("Element (" + by + ") " + attribute + " attribute does not equal:  " + text, e);
                stopTestExecution(stopTest);
                return 1;
            }
        }
        public static int VerifyText(By by, string text)
        {
            return VerifyText(driver, by, text, false);
        }

        public static int VerifyText(IWebDriver driver, By by, string text)
        {
            return VerifyText(driver, by, text, false);
        }
        public static int VerifyText(IWebDriver driver, By by, string text, bool stopTest)
        {
            try
            {
                WaitForPageToLoad(driver);
                Assert.AreEqual(text, driver.FindElement(by).Text, true);
                report.Pass("Element (" + by + ") equals:  " + text);
                return 0;
            }
            catch (Exception e)
            {
                report.Fail("Element (" + by + ") does not equal:  " + text, e);
                stopTestExecution(stopTest);
                return 1;
            }
        }
        public static int VerifyFieldPopulation(By by, bool expected)
        {
            try
            {
                WaitForPageToLoad(driver);
                var text = "";
                var type = driver.FindElement(by).TagName;
                switch (type)
                {
                    case "input":
                    case "textarea":
                        text = driver.FindElement(by).GetAttribute("value");
                        break;
                    case "select":
                        text = new SelectElement(driver.FindElement(by)).SelectedOption.Text;
                        break;
                    case "a":
                        text = driver.FindElement(by).Text;
                        break;
                }
                switch (expected)
                {
                    case true:
                        Assert.IsNotNull(text);
                        report.Pass(by + " is populated by " + text);
                        break;
                    case false:
                        Assert.IsNull(text);
                        report.Pass(by + " is empty");
                        break;
                }
                return 0;
            }
            catch (Exception e)
            {
                report.Fail(e.Message);
                return 1;
            }
        }
        public static int VerifyTextContains(By by, string text)
        {
            return VerifyTextContains(driver, by, text, false);
        }
        public static int VerifyTextContains(IWebDriver driver, By by, string text)
        {
            return VerifyTextContains(driver, by, text, false);
        }
        public static int VerifyTextContains(IWebDriver driver, By by, string text, bool stopTest)
        {
            try
            {
                WaitForPageToLoad(driver);
                StringAssert.Contains(driver.FindElement(by).Text.ToUpper().Trim(), text.ToUpper().Trim());
                report.Pass("Element (" + by + ") contains:  " + text);
                return 0;
            }
            catch (Exception e)
            {
                report.Fail("Element (" + by + ") does not contain:  " + text, e);
                stopTestExecution(stopTest);
                return 1;
            }
        }

        public static int VerifyTextContains(string textToSearch, string textToFind)
        {
            try
            {
                WaitForPageToLoad(driver);
                StringAssert.Contains(textToSearch.ToUpper().Trim(), textToFind.ToUpper().Trim());
                report.Pass("Text (" + textToSearch + ") contains:  " + textToFind);
                return 0;
            }
            catch (Exception e)
            {
                report.Fail("Text (" + textToSearch + ") does not contain:  " + textToFind, e);
                return 1;
            }
        }

        public static int VerifyTextStartsWith(string textToSearch, string textToFind)
        {
            try
            {
                WaitForPageToLoad(driver);
                StringAssert.StartsWith(textToSearch.ToUpper().Trim(), textToFind.ToUpper().Trim());
                report.Pass("Text (" + textToSearch + ") starts with:  " + textToFind);
                return 0;
            }
            catch (Exception e)
            {
                report.Fail("Text (" + textToSearch + ") does not start with:  " + textToFind, e);
                return 1;
            }
        }

        public static int VerifyEqual(string actual, string expected)
        {
            try
            {
                WaitForPageToLoad(driver);
                Assert.AreEqual(expected, actual, true);
                report.Pass(actual + " equals " + expected);
                return 0;
            }
            catch (Exception e)
            {
                report.Fail(actual + " does not equal " + expected, e);
                return 1;
            }
        }

        public static int VerifyTrue(bool actual, bool expected, string message)
        {
            try
            {
                WaitForPageToLoad(driver);
                Assert.IsTrue(expected == actual);
                report.Pass(message + ": " + actual + " equals " + expected);
                return 0;
            }
            catch
            {
                report.Fail(message + ": " + actual + " does not equal " + expected);
                return 1;
            }
        }

        public static int VerifyFieldByOperator(mdTable table, string columnname, string operand, string text)
        {
            return VerifyFieldByOperator(driver, null, operand, text, false, table, columnname);
        }

        public static int VerifyFieldByOperator(IWebDriver driver, By by, string operand, string text)
        {
            return VerifyFieldByOperator(driver, by, operand, text, false);
        }

        public static int VerifyFieldByOperator(IWebDriver driver, By by, string operand, string text, bool stopTest, mdTable table = null, string columnname = null)
        {
            string result;

            if (table == null)
            {
                result = driver.FindElement(by).Text;
                if (string.IsNullOrEmpty(result))  //Some tag types don't have a text element (e.g. TextArea).
                {
                    result = string.IsNullOrEmpty(driver.FindElement(by).GetAttribute("value")) ? result : driver.FindElement(by).GetAttribute("value");
                }
            }
            else
            {
                result = table.getFirstColumnValue(columnname);
            }

            try
            {
                WaitForPageToLoad(driver);

                switch (operand)
                {
                    case "=":
                        Assert.AreEqual(string.IsNullOrEmpty(result) ? 0 : int.Parse(result), int.Parse(text));
                        report.Pass(by + ":  '" + result + "' " + operand + " " + text);
                        return 0;
                    case ">":
                        Assert.IsTrue((string.IsNullOrEmpty(result) ? 0 : int.Parse(result)) > int.Parse(text));
                        report.Pass(by + ":  '" + result + "' " + operand + " " + text);
                        return 0;
                    case "<":
                        Assert.IsTrue((string.IsNullOrEmpty(result) ? 0 : int.Parse(result)) < int.Parse(text));
                        report.Pass(by + ":  '" + result + "' " + operand + " " + text);
                        return 0;
                    case "<>":
                        Assert.AreNotEqual(string.IsNullOrEmpty(result) ? 0 : int.Parse(result), int.Parse(text));
                        report.Pass(by + ":  '" + result + "' " + operand + " " + text);
                        return 0;
                    case ">=":
                        Assert.IsTrue((string.IsNullOrEmpty(result) ? 0 : int.Parse(result)) >= int.Parse(text));
                        report.Pass(by + ":  '" + result + "' " + operand + " " + text);
                        return 0;
                    case "<=":
                        Assert.IsTrue((string.IsNullOrEmpty(result) ? 0 : int.Parse(result)) <= int.Parse(text));
                        report.Pass(by + ":  '" + result + "' " + operand + " " + text);
                        return 0;
                    case "Is null":
                        Assert.AreEqual("", result);
                        report.Pass(by + ":  '" + result + "' " + operand + " " + text);
                        return 0;
                    case "Is not null":
                        Assert.IsNotNull(result);
                        report.Pass(by + ":  '" + result + "' " + operand + " " + text);
                        return 0;
                    case "Equals":
                        Assert.AreEqual(text.ToUpper(), result.ToUpper());
                        report.Pass(by + ":  '" + result + "' " + operand + " " + text);
                        return 0;
                    case "Includes":
                        StringAssert.Contains(result.ToUpper(), text.ToUpper());
                        report.Pass(by + ":  '" + result + "' " + operand + " " + text);
                        return 0;
                    case "Does not include":
                        Assert.IsFalse(result.ToUpper().Contains(text.ToUpper()));
                        report.Pass(by + ":  '" + result + "' " + operand + " " + text);
                        return 0;
                    case "Begins with":
                        StringAssert.StartsWith(result.ToUpper(), text.ToUpper());
                        report.Pass(by + ":  '" + result + "' " + operand + " " + text);
                        return 0;
                    case "Does not equal":
                        Assert.AreNotEqual(text.ToUpper(), result.ToUpper());
                        report.Pass(by + ":  '" + result + "' " + operand + " " + text);
                        return 0;
                    case "Is empty":
                        Assert.AreEqual("", result);
                        report.Pass(by + ":  '" + result + "' " + operand + " " + text);
                        return 0;
                    case "Is not empty":
                        Assert.AreNotEqual("", result);
                        report.Pass(by + ":  '" + result + "' " + operand + " " + text);
                        return 0;
                    case "d=":
                        Assert.AreEqual(DateTime.Parse(text), DateTime.Parse(result));
                        report.Pass(by + ":  '" + result + "' " + operand + " " + text);
                        return 0;
                    case "d<":
                        Assert.IsTrue(DateTime.Parse(result) < DateTime.Parse(text));
                        report.Pass(by + ":  '" + result + "' " + operand + " " + text);
                        return 0;
                    case "d>":
                        Assert.IsTrue(DateTime.Parse(result) > DateTime.Parse(text));
                        report.Pass(by + ":  '" + result + "' " + operand + " " + text);
                        return 0;
                    case "d<=":
                        Assert.IsTrue(DateTime.Parse(result) <= DateTime.Parse(text));
                        report.Pass(by + ":  '" + result + "' " + operand + " " + text);
                        return 0;
                    case "d>=":
                        Assert.IsTrue(DateTime.Parse(result) >= DateTime.Parse(text));
                        report.Pass(by + ":  '" + result + "' " + operand + " " + text);
                        return 0;
                    default:
                        report.Fail("Invalid operator was passed: " + operand);
                        stopTestExecution(stopTest);
                        return 1;
                }
            }
            catch (Exception e)
            {
                report.Fail(by + ":  " + result + " " + operand + " " + text, e);
                browser.TakeScreenshot(MethodBase.GetCurrentMethod().Name);
                stopTestExecution(stopTest);
                return 1;
            }
        }
        public static int VerifyDateFieldByOperator(IWebDriver driver, By by, string operand, string text)
        {
            return VerifyDateFieldByOperator(driver, by, operand, text, false);
        }
        public static int VerifyDateFieldByOperator(IWebDriver driver, By by, string operand, string text, bool stopTest)
        {
            WaitForPageToLoad(driver);
            string field = driver.FindElement(by).GetAttribute("value");
            DateTime? date = string.IsNullOrEmpty(text) ? (DateTime?)null : DateTime.Parse(text);
            DateTime? result = string.IsNullOrEmpty(field) ? (DateTime?)null : DateTime.Parse(field);

            try
            {
                switch (operand)
                {
                    case "=":
                        Assert.AreEqual(date, result);
                        report.Pass(by + ":  " + result + " " + operand + " " + date);
                        return 0;
                    case ">":
                        Assert.IsTrue(result > date);
                        report.Pass(by + ":  " + result + " " + operand + " " + date);
                        return 0;
                    case "<":
                        Assert.IsTrue(result < date);
                        report.Pass(by + ":  " + result + " " + operand + " " + date);
                        return 0;
                    case "<>":
                        Assert.AreNotEqual(date, result);
                        report.Pass(by + ":  " + result + " " + operand + " " + date);
                        return 0;
                    case ">=":
                        Assert.IsTrue(result >= date);
                        report.Pass(by + ":  " + result + " " + operand + " " + date);
                        return 0;
                    case "<=":
                        Assert.IsTrue(result <= date);
                        report.Pass(by + ":  " + result + " " + operand + " " + date);
                        return 0;
                    case "Is null":
                        Assert.IsNull(result);
                        report.Pass(by + ":  " + result + " " + operand + " " + date);
                        return 0;
                    case "Is not null":
                        Assert.IsNotNull(result);
                        report.Pass(by + ":  " + result + " " + operand + " " + date);
                        return 0;
                    default:
                        report.Fail("Invalid operator was passed: " + operand);
                        stopTestExecution(stopTest);
                        return 1;
                }
            }
            catch (Exception e)
            {
                report.Fail(by + ":  " + result + " " + operand + " " + date, e);
                stopTestExecution(stopTest);
                return 1;
            }
        }
        public static int VerifyTextDoesNotEqual(IWebDriver driver, By by, string text)
        {
            return VerifyTextDoesNotEqual(driver, by, text, false);
        }

        public static int VerifyTextDoesNotEqual(IWebDriver driver, By by, string text, bool stopTest)
        {
            try
            {
                WaitForPageToLoad(driver);
                Assert.AreNotEqual(text, driver.FindElement(by).Text);
                report.Pass("Element (" + by + ") does not equal:  " + text);
                return 0;
            }
            catch (Exception e)
            {
                report.Fail("Element (" + by + ") equals:  " + text, e);
                stopTestExecution(stopTest);
                return 1;
            }
        }

        public static int VerifyWarningToast(IWebDriver driver, string text)
        {
            return VerifyWarningToast(driver, text, false);
        }
        public static int VerifyWarningToast(IWebDriver driver, string text, bool stopTest)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            By by = By.XPath("//div[@class='toast toast-warning']//div[@class='toast-message'] | //div[@class='epiq-toast epiq-toast-warning']//span[@ng-bind='vm.message']");

            try
            {
                wait.Until(d => (d.FindElements(by).Count != 0));
                Assert.AreEqual(text, driver.FindElement(by).Text);
                report.Pass("Warning Toast equals:  " + text);
                driver.FindElement(by).Click();
                return 0;
            }
            catch (Exception e)
            {
                report.Fail("Warning Toast does not equal:  " + text, e);
                stopTestExecution(stopTest);
                return 1;
            }
        }

        public static int VerifySuccessToast()
        {
            return VerifySuccessToast(driver, null, false);
        }

        public static int VerifySuccessToast(IWebDriver driver, string text)
        {
            return VerifySuccessToast(driver, text, false);
        }
        public static int VerifySuccessToast(IWebDriver driver, string text, bool stopTest)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            By by = By.XPath("//div[@class='toast toast-success']//div[@class='toast-message'] | //div[@class='epiq-toast epiq-toast-success']//span[@ng-bind='vm.message']");

            try
            {
                wait.Until(d => (d.FindElements(by).Count != 0));

                if (text == null)
                {
                    report.Pass("Success Toast exists");
                    driver.FindElements(by)[0].Click();
                    return 0;
                }

                foreach (IWebElement ele in driver.FindElements(by))
                {
                    if (ele.Text == text)
                    {
                        report.Pass("Success Toast exists:  " + text);
                        ele.Click();
                        return 0;
                    }
                }
                report.Fail("Success Toast does not exist:  " + text);
                stopTestExecution(stopTest);
                return 1;
            }
            catch (Exception e)
            {
                report.Fail("Success Toast does not exist:  " + text, e);
                stopTestExecution(stopTest);
                return 1;
            }
        }

        public static int VerifyInfoToast(IWebDriver driver, string text)
        {
            return VerifyInfoToast(driver, text, false);
        }
        public static int VerifyInfoToast(IWebDriver driver, string text, bool stopTest)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            By by = By.XPath("//div[@class='toast toast-info']//div[@class='toast-message'] | //div[@class='epiq-toast epiq-toast-info']//span[@ng-bind='vm.message']");

            try
            {
                wait.Until(d => (d.FindElements(by).Count != 0));
                Assert.AreEqual(text, driver.FindElement(by).Text);
                report.Pass("Info Toast equals:  " + text);
                driver.FindElement(by).Click();
                return 0;
            }
            catch (Exception e)
            {
                report.Fail("Info Toast does not equal:  " + text, e);
                stopTestExecution(stopTest);
                return 1;
            }
        }

        public static int VerifyLogin(string message)
        {
            return VerifyLogin(message, "Success");
        }

        public static int VerifyLogin(string message, string expectedMessage)
        {
            if (message == expectedMessage)
            {
                report.Pass("Login message was: " + message);
                return 0;
            }
            else
            {
                report.Fail("Login message: " + message + " does not equal expected message: " + expectedMessage);
                return 1;
            }
        }

        public static int VerifyNoErrorToasts()
        {
            return VerifyNoErrorToasts(driver, false);
        }

        public static int VerifyNoErrorToasts(IWebDriver driver)
        {
            return VerifyNoErrorToasts(driver, false);
        }

        public static int VerifyNoErrorToasts(IWebDriver driver, bool stopTest)
        {
            string errortext = ErrorToastReturned();

            if (errortext != null)
            {
                report.Fail("Error toasts returned: " + errortext);
                stopTestExecution(stopTest);
                return 1;
            }
            else
            {
                report.Pass("No Error toasts returned");
                return 0;
            }
        }

        public static int VerifyNoWarningToasts()
        {
            string warningtext = WarningToastReturned();

            if (warningtext != null)
            {
                report.Fail("Warning toasts returned: " + warningtext);
                return 1;
            }
            else
            {
                report.Pass("No Warning toasts returned");
                return 0;
            }
        }

        public static string ErrorToastReturned()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            By by = By.XPath("//div[@class='toast toast-error']//div[@class='toast-message'] | //div[@class='epiq-toast epiq-toast-error']//span[@ng-bind='vm.message']");
            string errortext = null;

            try
            {
                wait.Until(d => (d.FindElements(by).Count != 0));
                errortext = driver.FindElement(by).Text;
                driver.FindElement(by).Click();
                return errortext;
            }
            catch (Exception)
            {
                return errortext;
            }
        }

        public static string WarningToastReturned()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            By by = By.XPath("//div[@class='toast toast-warning']//div[@class='toast-message'] | //div[@class='epiq-toast epiq-toast-warning']//span[@ng-bind='vm.message']");
            string warningtext = null;

            try
            {
                wait.Until(d => (d.FindElements(by).Count != 0));
                warningtext = driver.FindElement(by).Text;
                driver.FindElement(by).Click();
                return warningtext;
            }
            catch (Exception)
            {
                return warningtext;
            }
        }

        public static int VerifyURL(string url)
        {
            return VerifyURL(driver, url, false);

        }
        public static int VerifyURL(IWebDriver driver, string url)
        {
            return VerifyURL(driver, url, false);

        }
        public static int VerifyURL(IWebDriver driver, string url, bool stopTest)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            try
            {
                wait.Until(d => (d.Url.Contains(url)));
                report.Pass("URL contains:  " + url);
                return 0;
            }
            catch (Exception e)
            {
                report.Fail("URL does not contain:  " + url + ";  Instead, url is " + driver.Url, e);
                stopTestExecution(stopTest);
                return 1;
            }
        }
        public static int VerifyElementExists(By by)
        {
            return VerifyElementExists(driver, by);
        }
        public static int VerifyElementExists(IWebDriver driver, By by)
        {
            return VerifyElementExists(driver, by, false);
        }
        public static int VerifyElementExists(IWebDriver driver, By by, bool stopTest)
        {
            try
            {
                WaitForPageToLoad(driver);
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(d => d.FindElement(by).Displayed);
                report.Pass("Element (" + by + ") exists");
                return 0;
            }
            catch (Exception e)
            {
                report.Fail("Element (" + by + ") does not exist", e);
                stopTestExecution(stopTest);
                return 1;
            }
        }

        public static int VerifyElementDoesNotExist(By by)
        {
            try
            {
                WaitForPageToLoad(driver);
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(d => d.FindElement(by).Displayed);
                report.Fail("Element (" + by + ") exists");
                return 1;
            }
            catch (WebDriverTimeoutException)
            {
                report.Pass("Element (" + by + ") does not exist");
                return 0;
            }
            catch (Exception e)
            {
                report.Fail("Error occurred while determining (" + by + ") element existence: ", e);
                return 1;
            }
        }


        public static bool doesElementExist(By by, int secondsToWait)
        {
            try
            {
                WaitForPageToLoad(driver);
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(secondsToWait));
                wait.Until(d => d.FindElement(by).Displayed);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool doesElementExist(By by)
        {
            return doesElementExist(by, 0);
        }

        public static void stopTestExecution(bool stopTest)
        {
            if (stopTest)
            {
                //Assert.Fail("Test execution stopped");
                System.Threading.Thread.CurrentThread.Abort();
            }
        }

        public static string GetCurrentDateTime(string format)
        {
            return DateTime.Now.ToString(format);
        }

        public static string GetCurrentDateTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
        }

        public static string RandomString(int Size)
        {
            string input = "abcdefghijklmnopqrstuvwxyz0123456789";
            var builder = new StringBuilder();
            char ch;
            var random = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < Size; i++)
            {
                ch = input[random.Next(0, input.Length)];
                builder.Append(ch);
            }
            return builder.ToString();
        }
        public static void EditField(By by, string text)
        {
            EditField(driver, by, text);
        }
        public static void EditField(IWebDriver driver, By by, string text)
        {

            report.Action("Edit", by.ToString(), text);
            WaitForPageToLoad(driver);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(config.TIMEOUT));
            wait.Until(d => d.FindElement(by).Enabled);

            IWebElement field = driver.FindElement(by);
            if (field.TagName == "md-datepicker")
            {
                field = field.FindElement(By.TagName("input"));
            }

            field.Clear();
            field.SendKeys(text + Keys.Tab);
            WaitForPageToLoad(driver);
        }

        public static void ClearField(By by)
        {
            report.Action("Clear", by.ToString());
            WaitForPageToLoad(driver);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(config.TIMEOUT));
            wait.Until(d => d.FindElement(by).Enabled);

            driver.FindElement(by).Clear();
            WaitForPageToLoad(driver);
        }
        public static void SelectField(By by, string text)
        {
            SelectField(driver, by, text);
        }
        public static void SelectField(IWebDriver driver, By by, string text)
        {
            report.Action("Select", by.ToString(), text);
            WaitForPageToLoad(driver);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(config.TIMEOUT));
            wait.Until(d => d.FindElement(by).Enabled);

            IWebElement field = driver.FindElement(by);
            if (field.TagName == "md-select")
            {
                new mdSelectElement(field).SelectByText(text);
            }
            else
            {
                new SelectElement(field).SelectByText(text);
            }

            WaitForPageToLoad(driver);
        }

        public static void SelectFieldByIndex(By by, int index)
        {
            SelectFieldByIndex(driver, by, index);
        }
        public static void SelectFieldByIndex(IWebDriver driver, By by, int index)
        {
            report.Action("Select", by.ToString(), index.ToString());
            WaitForPageToLoad(driver);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(config.TIMEOUT));
            wait.Until(d => d.FindElement(by).Enabled);

            IWebElement field = driver.FindElement(by);
            if (field.TagName == "md-select")
            {
                new mdSelectElement(field).SelectByIndex(index);
            }
            else
            {
                new SelectElement(field).SelectByIndex(index);
            }
            WaitForPageToLoad(driver);
        }

        public static void SelectFieldByValue(By by, string value)
        {
            SelectFieldByValue(driver, by, value);
        }
        public static void SelectFieldByValue(IWebDriver driver, By by, string value)
        {
            report.Action("Select", by.ToString(), value);
            WaitForPageToLoad(driver);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(config.TIMEOUT));
            wait.Until(d => d.FindElement(by).Enabled);

            IWebElement field = driver.FindElement(by);
            if (field.TagName == "md-select")
            {
                new mdSelectElement(field).SelectByValue(value);
            }
            else
            {
                new SelectElement(field).SelectByValue(value);
            }
            WaitForPageToLoad(driver);
        }

        public static void Click(By by)
        {
            Click(driver, by);
        }

        public static void Click(IWebElement element)
        {
            report.Action("Click", element.Text);
            Click(driver, element);
        }

        public static void SetCheckbox(By by, bool value)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(config.TIMEOUT));
            WaitForPageToLoad(driver);
            wait.Until(d => d.FindElement(by).Enabled);

            IWebElement element = driver.FindElement(by);

            if (element.TagName == "md-checkbox")
            {
                var checkbox = new mdCheckbox(element);

                if (value)
                {
                    if (!checkbox.IsChecked)
                    {
                        report.Action("Click", by + " is checked.");
                        element.Click();
                    }
                    else
                    {
                        report.Action("Skip", by + " is already checked.");
                    }
                }
                else
                {
                    if (checkbox.IsChecked)
                    {
                        report.Action("Click", by + " is unchecked.");
                        element.Click();
                    }
                    else
                    {
                        report.Action("Skip", by + " is already unchecked.");
                    }
                }
            }
            else
            {
                if (value)
                {
                    if (driver.FindElement(by).GetAttribute("class").Contains("ng-empty"))
                    {
                        Click(by);
                    }
                    else
                    {
                        report.Action("Skip", by + " is already checked.");
                    }
                }
                else
                {
                    if (driver.FindElement(by).GetAttribute("class").Contains("ng-not-empty"))
                    {
                        Click(by);
                    }
                    else
                    {
                        report.Action("Skip", by + " is already unchecked.");
                    }
                }
            }
            WaitForPageToLoad(driver);
        }

        public static void SetSwitch(By by, bool value)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(config.TIMEOUT));
            WaitForPageToLoad(driver);
            wait.Until(d => d.FindElement(by).Enabled);

            IWebElement element = driver.FindElement(by);


            var checkbox = new mdSwitch(element);

            if (value)
            {
                if (!checkbox.IsChecked)
                {
                    report.Action("Click", by + " is on");
                    element.Click();
                }
                else
                {
                    report.Action("Skip", by + " is already on.");
                }
            }
            else
            {
                if (checkbox.IsChecked)
                {
                    report.Action("Click", by + " is off.");
                    element.Click();
                }
                else
                {
                    report.Action("Skip", by + " is already off.");
                }
            }

            WaitForPageToLoad(driver);
        }

        public static void Click(IWebDriver driver, By by)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(config.TIMEOUT));

            WaitForPageToLoad(driver);
            wait.Until(d => d.FindElement(by).Enabled);
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(by));
            var element = driver.FindElement(by);
            report.Action("Click", string.IsNullOrEmpty(element.Text) ? by.ToString() : element.Text);
            Click(driver, element);
        }

        private static void Click(IWebDriver driver, IWebElement element)
        {
            ScrollTo(driver, element); //Chrome requires element onscreen to click
            dismissToasts(driver);
            element.Click();
            WaitForPageToLoad(driver);
        }

        public static void ClickandConfirm(By by)
        {
            ClickandConfirm(driver, by);
        }
        public static void ClickandConfirm(IWebDriver driver, By by)
        {
            //For alerts
            report.Action("Click and Confirm alert", by.ToString());
            WaitForPageToLoad(driver);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(config.TIMEOUT));
            wait.Until(d => d.FindElement(by).Enabled);
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(by));

            ScrollTo(driver, by); //Chrome requires element onscreen to click
            dismissToasts(driver);
            driver.FindElement(by).Click();
            WaitForChrome(driver, 500);
            driver.SwitchTo().Alert().Accept();
            WaitForPageToLoad(driver);
        }

        public static void ClickandCancel(By by)
        {
            //For alerts
            report.Action("Click and Cancel alert", by.ToString());
            WaitForPageToLoad(driver);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(config.TIMEOUT));
            wait.Until(d => d.FindElement(by).Enabled);
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(by));

            ScrollTo(driver, by); //Chrome requires element onscreen to click
            dismissToasts(driver);
            driver.FindElement(by).Click();
            WaitForChrome(driver, 500);
            driver.SwitchTo().Alert().Dismiss();
            WaitForPageToLoad(driver);
        }
        public static void ClickandConfirmHtml(By by)
        {
            ClickandConfirmHtml(driver, by);
        }

        public static void ClickandConfirmHtml(IWebDriver driver, By by)
        {
            //for html popups
            report.Action("Click", by.ToString());
            WaitForPageToLoad(driver);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(config.TIMEOUT));
            wait.Until(d => d.FindElement(by).Enabled);
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(by));

            ScrollTo(driver, by); //Chrome requires element onscreen to click
            dismissToasts(driver);
            driver.FindElement(by).Click();
            WaitForChrome(driver, 500);
            Click(driver, By.XPath("//button[@ng-click='yes()']"));
            WaitForPageToLoad(driver);
        }

        public static void ClickandConfirmDialog(By by)
        {
            //for md dialog
            report.Action("Click", by.ToString());
            WaitForPageToLoad(driver);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(config.TIMEOUT));
            wait.Until(d => d.FindElement(by).Enabled);
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(by));

            ScrollTo(driver, by); //Chrome requires element onscreen to click
            dismissToasts(driver);
            driver.FindElement(by).Click();
            WaitForChrome(driver, 500);
            Click(driver, By.XPath("//button[@ng-click='dialog.hide()']"));
            WaitForPageToLoad(driver);
        }

        public static int ClickAndVerifyAlert(By by, string expectedText, bool? accept = null)
        {
            try
            {
                report.Action("Click", by.ToString());
                WaitForPageToLoad(driver);
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(config.TIMEOUT));
                wait.Until(d => d.FindElement(by).Enabled);
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(by));

                ScrollTo(driver, by); //Chrome requires element onscreen to click
                dismissToasts(driver);
                driver.FindElement(by).Click();
                WaitForChrome(driver, 500);
                if (!driver.SwitchTo().Alert().Text.Contains(expectedText)) throw new Exception();
                report.Pass("Alert text contains " + expectedText);
                if (accept == true) driver.SwitchTo().Alert().Accept();
                if (accept == false) driver.SwitchTo().Alert().Dismiss();
                WaitForPageToLoad(driver);
                VerifyNoErrorToasts();
                return 0;
            }
            catch (Exception e)
            {
                report.Fail("Alert text: " + driver.SwitchTo().Alert().Text + " does not contain the expected text: " +
                            expectedText, e);
                return 1;
            }
        }

        public static void WaitForDynamicFormToLoad()
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(config.TIMEOUT_PAGELOAD));
                string msg = "Loading. Please wait...";
                By msg_ele = By.XPath("//span[contains(text(),'" + msg + "')]");
                WaitForPageToLoad(driver);

                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementWithText(msg_ele, msg));
                //System.Threading.Thread.Sleep(2000);
                WaitForPageToLoad(driver);
            }
            catch
            {
                Assert.Fail("Page failed to load in " + config.TIMEOUT_PAGELOAD.ToString() + " seconds.");
            }
        }

        public static void WaitForPageToLoad()
        {
            WaitForPageToLoad(driver);
        }
        public static void WaitForPageToLoad(IWebDriver driver)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(config.TIMEOUT_PAGELOAD));

                //wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("(//section|//div)[contains(@class,'content')]")));
                wait.Until(d => (d.FindElements(By.XPath("//body[contains(@class,'block-ui-active')]")).Count == 0));
                wait.Until(d => (d.FindElements(By.XPath("//*[@aria-busy='true']")).Count == 0));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//div[contains(@class,'block-ui-overlay')]")));

                if (!isPageReady(driver))
                    WaitForPageToLoad(driver);
            }
            catch
            {
                Assert.Fail("Page failed to load in " + config.TIMEOUT_PAGELOAD.ToString() + " seconds.");
            }
        }

        private static bool isPageReady(IWebDriver driver)
        {
            Boolean ready = false;
            System.Threading.Thread.Sleep(250);

            ready |= (driver.FindElements(By.XPath("//body[contains(@class,'block-ui-active')]")).Count == 0
                && driver.FindElements(By.XPath("//*[@aria-busy='true']")).Count == 0);

            return ready;
        }

        public static string GetFieldValue(By by)
        {
            WaitForPageToLoad(driver);
            WaitForElement(by);
            return driver.FindElement(by).GetAttribute("value");
        }

        public static int VerifyFieldText(By by, string expectedvalue)
        {
            try
            {
                WaitForPageToLoad(driver);
                StringAssert.Contains(driver.FindElement(by).Text, expectedvalue);
                report.Pass(by + " contains: " + expectedvalue);
                return 0;
            }

            catch (Exception e)
            {
                report.Fail(by + " does not contain " + expectedvalue + ": " + e.Message);
                return 1;
            }
        }
        public static int VerifyFieldValue(By by, string expectedvalue)
        {
            return VerifyFieldValue(driver, by, expectedvalue);
        }
        public static int VerifyFieldValue(IWebDriver driver, By by, string expectedvalue)
        {
            try
            {
                WaitForPageToLoad(driver);
                StringAssert.Contains(driver.FindElement(by).GetAttribute("value"), expectedvalue);
                report.Pass(by + " contains: " + expectedvalue);
                return 0;
            }

            catch (Exception e)
            {
                report.Fail(by + " does not contain " + expectedvalue + ": " + e.Message);
                return 1;
            }
        }

        public static int VerifyFieldValueById(string fieldname, string expectedvalue)
        {
            return VerifyFieldValueById(driver, fieldname, expectedvalue);
        }
        public static int VerifyFieldValueById(IWebDriver driver, string fieldname, string expectedvalue)
        {
            try
            {
                WaitForPageToLoad(driver);
                StringAssert.Contains(driver.FindElement(By.Id(fieldname)).GetAttribute("value"), expectedvalue);
                report.Pass(fieldname + " contains: " + expectedvalue);
                return 0;
            }

            catch (Exception e)
            {
                report.Fail(fieldname + " does not contain " + expectedvalue + ": " + e);
                return 1;
            }
        }
        public static int VerifyFieldValueByName(string fieldname, string expectedvalue)
        {
            return VerifyFieldValueByName(driver, fieldname, expectedvalue);
        }

        public static int VerifyFieldValueByName(IWebDriver driver, string fieldname, string expectedvalue)
        {
            try
            {
                WaitForPageToLoad(driver);

                IWebElement element = driver.FindElement(By.Name(fieldname));
                if (element.TagName == "md-datepicker")
                    element = element.FindElement(By.TagName("input"));

                StringAssert.Contains(element.GetAttribute("value"), expectedvalue);
                report.Pass(fieldname + " contains: " + expectedvalue);
                return 0;
            }

            catch (Exception e)
            {
                report.Fail(fieldname + " does not contain " + expectedvalue + ": " + e);
                return 1;
            }
        }


        public static int VerifyCheckBoxValue(By by, bool? expectedvalue)
        {
            WaitForElement(by);

            IWebElement element = driver.FindElement(by);

            if (element.TagName == "md-checkbox")
            {
                try
                {
                    var checkbox = new mdCheckbox(element);
                    if (checkbox.IsChecked == expectedvalue)
                    {
                        report.Pass(by + " is " + expectedvalue);
                        return 0;
                    }
                    else
                    {
                        report.Fail(by + " is not " + expectedvalue);
                        return 1;
                    }
                }
                catch (Exception e)
                {
                    report.Fail("Unable to verify: " + e);
                    return 1;
                }
            }
            else
            {
                string expectedclass = "";
                switch (expectedvalue)
                {
                    case true:
                        expectedclass = "ng-not-empty";
                        break;
                    default:
                        expectedclass = "ng-empty";
                        break;
                }

                try
                {
                    if (driver.FindElement(by).GetAttribute("class").Contains(expectedclass))
                    {
                        report.Pass(by + " contains: " + expectedvalue);
                        return 0;
                    }
                    else
                    {
                        report.Fail(by + " does not contain " + expectedvalue);
                        return 1;
                    }
                }
                catch (Exception e)
                {
                    report.Fail("Unable to verify: " + e);
                    return 1;
                }
            }
        }

        public static int VerifySwitchValue(By by, bool? expectedvalue)
        {
            WaitForElement(by);

            IWebElement element = driver.FindElement(by);

            try
            {
                var checkbox = new mdSwitch(element);
                if (checkbox.IsChecked == expectedvalue)
                {
                    report.Pass(by + " is " + expectedvalue);
                    return 0;
                }
                else
                {
                    report.Fail(by + " is not " + expectedvalue);
                    return 1;
                }
            }
            catch (Exception e)
            {
                report.Fail("Unable to verify: " + e);
                return 1;
            }

        }

        public static void ToggleSection(string sectionname)
        {
            ToggleSection(driver, sectionname);
        }

        public static void ToggleSection(IWebDriver driver, string sectionname)
        {
            By by = By.XPath("//span[text()='" + sectionname + "']");
            report.Action("Toggle", sectionname);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(config.TIMEOUT));
            wait.Until(d => d.FindElement(by).Enabled);
            Click(driver, by);
            WaitForPageToLoad(driver);
        }

        static DateTime _globalBegin;
        static string _globalRun;
        static string _globalTestName;
        static string _globalBrowserName;
        static string _globalApplicationName;
        static string _globalFailureDescription;

        public static string GlobalRun
        {
            get
            {
                return _globalRun;
            }
            set
            {
                _globalRun = value;
            }
        }

        public static string GlobalBrowser
        {
            get
            {
                return _globalBrowserName;
            }
            set
            {
                _globalBrowserName = value;
            }
        }

        public static string GlobalTestName
        {
            get
            {
                return _globalTestName;
            }
            set
            {
                _globalTestName = value;
            }
        }


        public static string GlobalApplicationName
        {
            get
            {
                return _globalApplicationName;
            }
            set
            {
                _globalApplicationName = value;
            }
        }

        public static string GlobalFailureDescription
        {
            get
            {
                return _globalFailureDescription;
            }
            set
            {
                _globalFailureDescription = value;
            }
        }


        public static void startTime()
        {
            if (ConfigurationManager.AppSettings.Get("performance_testing_enabled") == "true")
            {
                _globalBegin = DateTime.Now;
            }
        }

        public static void stopTime(string transactionName)
        {
            if (ConfigurationManager.AppSettings.Get("performance_testing_enabled") == "true" && _globalTestName != "Logout")
            {
                DateTime end = DateTime.Now;
                double responseTime = Math.Round(((end.Subtract(_globalBegin).TotalMilliseconds) / 1000), 3);
                double timeLimit = SingleUserPerformance.Data.Database.getTimeLimit(ConfigurationManager.AppSettings.Get("singleuserperformance_connectionstring"), transactionName, _globalBrowserName, _globalApplicationName);

                if (responseTime > timeLimit)
                {
                    _globalFailureDescription = "WARNING response time " + responseTime + " > " + timeLimit + " sec limit";
                    //SendMail(ConfigurationManager.AppSettings.Get("informative_email"), _globalApplicationName + " '" + transactionName + "' " + _globalBrowserName + " transaction time " + responseTime + " > " + timeLimit + " sec time limit", "The report is at: " + ConfigurationManager.AppSettings.Get("report_url") + "\n\nTest Name: " + _globalTestName + "\n\nApplication Name: " + _globalApplicationName);
                }
                SingleUserPerformance.Data.Database.insertPerformanceResult(ConfigurationManager.AppSettings.Get("singleuserperformance_connectionstring"), _globalRun, transactionName, _globalFailureDescription, _globalTestName, responseTime, _globalBrowserName, timeLimit, SiteVersion(), _globalApplicationName);

            }
        }

        public static void stopTime(string transactionName, double timelimit)
        {
            if (ConfigurationManager.AppSettings.Get("performance_testing_enabled") == "true" && _globalTestName != "Logout")
            {
                DateTime end = DateTime.Now;
                double responseTime = Math.Round(((end.Subtract(_globalBegin).TotalMilliseconds) / 1000), 4);

                if (responseTime > timelimit)
                {
                    _globalFailureDescription = "WARNING: response time " + responseTime + " > " + timelimit + " sec limit";
                    //SendMail(ConfigurationManager.AppSettings.Get("informative_email"), _globalApplicationName + " '" + transactionName + "' " + _globalBrowserName + " transaction time " + responseTime + " > " + timeLimit + " sec time limit", "The report is at: " + ConfigurationManager.AppSettings.Get("report_url") + "\n\nTest Name: " + _globalTestName + "\n\nApplication Name: " + _globalApplicationName);
                }

                SingleUserPerformance.Data.Database.insertPerformanceResult(ConfigurationManager.AppSettings.Get("singleuserperformance_connectionstring"), _globalRun, transactionName, _globalFailureDescription, _globalTestName, responseTime, _globalBrowserName, timelimit, SiteVersion(), _globalApplicationName);
            }
        }

        public static string readConsole(string name)
        {
            string output = "";
            string browsername = ((RemoteWebDriver)driver).Capabilities.BrowserName;
            if (browsername.Equals("firefox"))
            {
                var entries = driver.Manage().Logs.GetLog(LogType.Browser);
                foreach (var entry in entries)
                {
                    if (entry.ToString().Contains("ERROR"))
                    {
                        Console.WriteLine(name + ": " + entry.ToString());
                        output = name + ": " + entry.ToString();
                    }
                }
            }

            if (browsername.Equals("chrome"))
            {
                var entries2 = driver.Manage().Logs.GetLog(LogType.Browser);
                foreach (var entry2 in entries2)
                {
                    Console.WriteLine(name + ": " + entry2.ToString());
                    output = name + ": " + entry2.ToString();
                }
            }
            return output;
        }

        public static void assignFailureDescription(string error)
        {
            if (error.Length > 499) { GlobalFailureDescription = error.Substring(0, 499); }
            else { GlobalFailureDescription = error; }
        }


        public static void SendMail(string to, string subject, string message)
        {
            SmtpClient smtp = new SmtpClient();
            MailMessage email = new MailMessage("tautoperf1@epiqsystems.com", to, subject, message);
            smtp.Send(email);
        }

        public static void dismissToasts(IWebDriver driver)
        {
            while (driver.FindElements(By.XPath("//div[@id='toast-container'] | //div[contains(@class,'epiq-toast')]/button")).Count > 0)
            {
                try
                {
                    driver.FindElement(By.XPath("//div[@id='toast-container'] | //div[contains(@class,'epiq-toast')]/button")).Click();
                    System.Threading.Thread.Sleep(500);
                }
                catch
                {
                    System.Threading.Thread.Sleep(500);
                }
            }

        }

        public static int VerifyElementAvailability(By by, bool expected)
        {
            if (expected == true)
            {
                if (driver.FindElements(by).Count > 0)
                {
                    report.Pass(by + " exists as was expected");
                    return 0;
                }
                else
                {
                    report.Fail(by + " does not exist which is not expected");
                    return 1;
                }
            }
            else
            {
                if (driver.FindElements(by).Count > 0)
                {
                    report.Fail(by + " exists which is not expected");
                    return 1;
                }
                else
                {
                    report.Pass(by + " does not exist as was expected");
                    return 0;
                }
            }

        }
        public static int VerifyElementEnabled(By by, bool expected)
        {
            try
            {
                WaitForElement(by);
            }
            catch (Exception e)
            {
                report.Fail("Unable to find element " + by, e);
                return 1;
            }
            string result;

            try
            {
                driver.FindElement(by).GetAttribute("disabled").ToString();
                result = "disabled";
            }
            catch
            {
                result = "enabled";
            }


            if (expected == true)
            {
                if (result == "enabled")
                {
                    report.Pass(by + " is enabled as was expected");
                    return 0;
                }
                else
                {
                    report.Fail(by + " is not enabled which is not expected");
                    return 1;
                }
            }
            else
            {
                if (result == "disabled")
                {
                    report.Pass(by + " is not enabled as was expected");
                    return 0;
                }
                else
                {
                    report.Fail(by + " is enabled which is not expected");
                    return 1;
                }
            }

        }

        public static int VerifyElementEditability(By by, bool expected)
        {
            WaitForElement(by);

            if (driver.FindElement(by).GetAttribute("ReadOnly") == null)
            {
                switch (expected)
                {
                    case true:
                        report.Pass(by + " is editable as was expected");
                        return 0;
                    default:
                        report.Fail(by + " is editable which is not expected");
                        return 1;
                }
            }
            else
            {
                switch (expected)
                {
                    case false:
                        report.Pass(by + " is not editable as was expected");
                        return 0;
                    default:
                        report.Fail(by + " is not editable which is not expected");
                        return 1;
                }

            }
        }


        public static int VerifyLinkAvailability(string linktext, bool expected)
        {
            return VerifyElementAvailability(By.LinkText(linktext), expected);
        }

        public static int VerifyElementVisibility(By by, bool expected)
        {
            try
            {
                WaitForElement(by);

                if (!driver.FindElement(by).GetAttribute("class").Contains("ng-hide") == expected)
                {
                    switch (expected)
                    {
                        case true:
                            report.Pass(by + " is visible as was expected");
                            break;
                        case false:
                            report.Pass(by + " is hidden as was expected");
                            break;
                    }
                    return 0;
                }
                else
                {
                    switch (expected)
                    {
                        case true:
                            report.Fail(by + " is hidden which is not expected");
                            break;
                        case false:
                            report.Fail(by + " is visible which is not expected");
                            break;
                    }
                    return 1;
                }
            }
            catch (Exception e)
            {
                report.Fail("Error occurred while determining element visibility: " + e.Message);
                return 1;
            }

        }

        public static int VerifyElementIsDisplayed(By by, bool expected)
        {
            try
            {
                WaitForElement(by);

                if (driver.FindElement(by).Displayed == expected)
                {
                    switch (expected)
                    {
                        case true:
                            report.Pass(by + " is visible as was expected");
                            break;
                        case false:
                            report.Pass(by + " is hidden as was expected");
                            break;
                    }
                    return 0;
                }
                else
                {
                    switch (expected)
                    {
                        case true:
                            report.Fail(by + " is hidden which is not expected");
                            break;
                        case false:
                            report.Fail(by + " is visible which is not expected");
                            break;
                    }
                    return 1;
                }
            }
            catch (Exception e)
            {
                report.Fail("Error occurred while determining element visibility: " + e.Message);
                return 1;
            }

        }

        public static int VerifyElementExists(By by, bool expected)
        {
            WaitForPageToLoad(driver);
            try
            {
                driver.FindElement(by);

                if (expected == true)
                {
                    report.Pass(by + " exists as was expected");
                    return 0;
                }
                else
                {
                    report.Fail(by + " exists which is not expected");
                    return 1;
                }

            }
            catch
            {
                if (expected == false)
                {
                    report.Pass(by + " does not exist as was expected");
                    return 0;
                }
                else
                {
                    report.Fail(by + " does not exist which is not expected");
                    return 1;
                }
            }
        }
        public static int VerifySelectOptionsContain(By operation_location, string listitem, bool expected)
        {
            WaitForElement(operation_location);

            IWebElement element = driver.FindElement(operation_location);
            if (element.TagName == "md-select")
                return VerifyMdSelectOptionsContain(operation_location, listitem, expected);

            var select = new SelectElement(driver.FindElement(operation_location));
            IEnumerable<string> options = select.Options.Select(i => i.Text);

            if (options.Contains(listitem))
            {
                switch (expected)
                {
                    case true:
                        report.Pass(listitem + " is present in " + operation_location + " as expected");
                        return 0;
                    default:
                        report.Fail(listitem + " is present in " + operation_location + " which is unexpected");
                        return 1;
                }
            }
            else
            {
                switch (expected)
                {
                    case false:
                        report.Pass(listitem + " is not present in " + operation_location + " as expected");
                        return 0;
                    default:
                        report.Fail(listitem + " is not present in " + operation_location + " which is unexpected");
                        return 1;
                }
            }
        }

        private static int VerifyMdSelectOptionsContain(By operation_location, string listitem, bool expected)
        {
            WaitForElement(operation_location);

            var select = new mdSelectElement(driver.FindElement(operation_location));
            IEnumerable<string> options = select.Options.Select(i => i.Text);

            if (options.Contains(listitem))
            {
                switch (expected)
                {
                    case true:
                        report.Pass(listitem + " is present in " + operation_location + " as expected");
                        return 0;
                    default:
                        report.Fail(listitem + " is present in " + operation_location + " which is unexpected");
                        return 1;
                }
            }
            else
            {
                switch (expected)
                {
                    case false:
                        report.Pass(listitem + " is not present in " + operation_location + " as expected");
                        return 0;
                    default:
                        report.Fail(listitem + " is not present in " + operation_location + " which is unexpected");
                        return 1;
                }
            }
        }

        public static void VerifyElementListContainsText(IList<IWebElement> list, string listdescription, string itemtext, bool expected)
        {
            bool match = false;

            foreach (IWebElement item in list)
            {
                match = item.Text.Equals(itemtext);
            }

            if (match)
            {
                switch (expected)
                {
                    case true:
                        report.Pass(itemtext + " is present in " + listdescription + " as expected");
                        return;
                    default:
                        report.Fail(itemtext + " is present in " + listdescription + " which is unexpected");
                        return;
                }
            }
            else
            {
                switch (expected)
                {
                    case false:
                        report.Pass(itemtext + " is not present in " + listdescription + " as expected");
                        return;
                    default:
                        report.Fail(itemtext + " is not present in " + listdescription + " which is unexpected");
                        return;
                }
            }
        }

        public static int VerifyAccess(bool expected)
        {
            bool access;

            WaitForPageToLoad(driver);

            if (driver.FindElements(By.XPath("//div[text()='Access Denied']")).Count() > 0)
            {
                access = false;
            }
            else
            {
                access = true;
            }

            if (access == expected)
            {
                report.Pass("Access is " + access + " as expected");
                return 0;
            }
            else
            {
                report.Fail("Access is " + access + " which is unexpected");
                return 1;
            }
        }

        public static string SiteVersion()
        {
            try
            {
                IJavaScriptExecutor js = driver as IJavaScriptExecutor;
                return (string)js.ExecuteScript("return siteVersion");
            }
            catch (Exception)
            {
                report.Pass("Using the default version 1.0.0");
                return "1.0.0";
            }
        }

        public static int VerifySelectedValueById(string fieldname, string expectedvalue)
        {
            try
            {
                WaitForElement(By.Id(fieldname));
                var select = new SelectElement(driver.FindElement(By.Id(fieldname)));
                StringAssert.Contains(select.SelectedOption.Text, expectedvalue);
                report.Pass(expectedvalue + " is selected for: " + fieldname);
                return 0;
            }

            catch (Exception e)
            {
                report.Fail(expectedvalue + " is not selected for: " + fieldname + ": " + e);
                return 1;
            }
        }

        public static int VerifySelectedValueByName(string fieldname, string expectedvalue)
        {
            try
            {
                WaitForElement(By.Name(fieldname));

                IWebElement element = driver.FindElement(By.Name(fieldname));
                if (element.TagName == "md-select")
                    return VerifyMdSelectedValueByName(fieldname, expectedvalue);

                var select = new SelectElement(driver.FindElement(By.Name(fieldname)));
                StringAssert.Contains(select.SelectedOption.Text, expectedvalue);
                report.Pass(expectedvalue + " is selected for " + fieldname);
                return 0;
            }

            catch (Exception e)
            {
                report.Fail(expectedvalue + " is not selected for " + fieldname + ": " + e.Message);
                return 1;
            }
        }

        private static int VerifyMdSelectedValueByName(string fieldname, string expectedvalue)
        {
            try
            {
                WaitForElement(By.Name(fieldname));
                var select = new mdSelectElement(driver.FindElement(By.Name(fieldname)));

                try
                {
                    StringAssert.Contains(select.SelectedOption.Text, expectedvalue);
                    select.closeSelectContainer();
                    report.Pass(expectedvalue + " is selected for " + fieldname);
                    return 0;
                }

                catch (Exception e)
                {
                    report.Fail(expectedvalue + " is not selected for " + fieldname + ": " + e.Message);
                    select.closeSelectContainer();
                    return 1;
                }
            }
            catch (Exception e)
            {
                report.Fail(expectedvalue + " is not selected for: " + fieldname + ": " + e.Message);
                return 1;
            }
        }

        public static int VerifySelectedValue(By by, string expectedvalue)
        {
            try
            {
                WaitForElement(by);

                IWebElement element = driver.FindElement(by);
                if (element.TagName == "md-select")
                    return VerifyMdSelectedValue(by, expectedvalue);

                var select = new SelectElement(driver.FindElement(by));
                StringAssert.Contains(select.SelectedOption.Text, expectedvalue);
                report.Pass(expectedvalue + " is selected for: " + by);
                return 0;
            }

            catch (Exception e)
            {
                report.Fail(expectedvalue + " is not selected for: " + by + ": " + e.Message);
                return 1;
            }
        }

        private static int VerifyMdSelectedValue(By by, string expectedvalue)
        {
            try
            {
                WaitForElement(by);
                var select = new mdSelectElement(driver.FindElement(by));

                try
                {
                    StringAssert.Contains(select.SelectedOption.Text, expectedvalue);
                    select.closeSelectContainer();
                    report.Pass(expectedvalue + " is selected for: " + by);
                    return 0;
                }
                catch (Exception e)
                {
                    report.Fail(expectedvalue + " is not selected for: " + by + ": " + e.Message);
                    select.closeSelectContainer();
                    return 1;
                }
            }
            catch (Exception e)
            {
                report.Fail(expectedvalue + " is not selected for: " + by + ": " + e.Message);
                return 1;
            }

        }

        public static int VerifySelectedOptionByValue(By by, string expectedvalue)
        {
            try
            {
                WaitForElement(by);

                IWebElement element = driver.FindElement(by);
                if (element.TagName == "md-select")
                    return VerifyMdSelectedOptionByValue(by, expectedvalue);

                var select = new SelectElement(driver.FindElement(by));
                StringAssert.Contains(select.SelectedOption.GetAttribute("value"), expectedvalue);
                report.Pass(expectedvalue + " is selected for: " + by);
                return 0;
            }

            catch (Exception e)
            {
                report.Fail(expectedvalue + " is not selected for: " + by + ": " + e.Message);
                return 1;
            }
        }

        private static int VerifyMdSelectedOptionByValue(By by, string expectedvalue)
        {
            try
            {
                WaitForElement(by);
                var select = new mdSelectElement(driver.FindElement(by));

                try
                {
                    StringAssert.Contains(select.SelectedOption.GetAttribute("value"), expectedvalue);
                    report.Pass(expectedvalue + " is selected for: " + by);
                    select.closeSelectContainer();
                    return 0;
                }

                catch (Exception e)
                {
                    report.Fail(expectedvalue + " is not selected for: " + by + ": " + e.Message);
                    select.closeSelectContainer();
                    return 1;
                }
            }
            catch (Exception e)
            {
                report.Fail(expectedvalue + " is not selected for: " + by + ": " + e.Message);
                return 1;
            }
        }

        public static int VerifyColumnContains(mdTable table, string columnname, string columnvalue)
        {
            try
            {
                int index = table.getRowIndex(columnname, columnvalue);

                if (index > -1)
                {
                    report.Pass("Column '" + columnname + "' contains the value '" + columnvalue + "'");
                    return 0;
                }
                else
                {
                    report.Fail("Column '" + columnname + "' does not contain the value '" + columnvalue + "'");
                    return 1;
                }

            }
            catch (Exception e)
            {
                report.Fail("Error occurred while verifying column value: " + e.Message);
                return 1;
            }
        }

        public static int VerifyColumnDoesNotContain(mdTable table, string columnname, string columnvalue)
        {
            try
            {
                int index = table.getRowIndex(columnname, columnvalue);

                if (index > -1)
                {
                    report.Fail("Column '" + columnname + "' contains the value '" + columnvalue + "'");
                    return 1;
                }
                else
                {
                    report.Pass("Column '" + columnname + "' does not contain the value '" + columnvalue + "'");
                    return 0;
                }

            }
            catch (Exception e)
            {
                report.Fail("Error occurred while verifying column value: " + e.Message);
                return 1;
            }
        }

        public static int VerifyColumnContains(ngGrid grid, string columnname, string columnvalue)
        {
            try
            {
                int index = grid.getRowIndex(columnname, columnvalue);

                if (index > -1)
                {
                    report.Pass("Column '" + columnname + "' contains the value '" + columnvalue + "'");
                    return 0;
                }
                else
                {
                    report.Fail("Column '" + columnname + "' does not contain the value '" + columnvalue + "'");
                    return 1;
                }

            }
            catch (Exception e)
            {
                report.Fail("Error occurred while verifying column value: " + e.Message);
                return 1;
            }
        }

        public static int VerifyClassExists(By by, string classtext, bool expected = true)
        {
            try
            {
                WaitForElement(by);
                if (expected)
                {
                    try
                    {
                        Assert.IsTrue(driver.FindElement(by).GetAttribute("class").Contains(classtext));
                        report.Pass(by + " contains the <" + classtext + "> class");
                        return 0;
                    }
                    catch
                    {
                        report.Fail(by + " does not contain the <" + classtext + "> class.");
                        return 1;
                    }
                }
                else
                {
                    try
                    {
                        Assert.IsFalse(driver.FindElement(by).GetAttribute("class").Contains(classtext));
                        report.Pass(by + " does not contain the <" + classtext + "> class");
                        return 0;
                    }
                    catch
                    {
                        report.Fail(by + " does contain the <" + classtext + "> class.");
                        return 1;
                    }
                }
            }
            catch (Exception e)
            {
                report.Fail("Error occurred while verifying element class: " + e);
                return 1;
            }
        }

        public static string ToYesNoString(bool? value)
        {
            if (value == null) return "";
            return (value == true) ? "Yes" : "No";
        }

        public static int VerifyCookie(string name, bool expected)
        {
            try
            {
                string cookie_name = driver.Manage().Cookies.GetCookieNamed(name).Name;

                if (expected == true)
                {
                    report.Pass(name + " cookie exists as was expected");
                    return 0;
                }
                else
                {
                    report.Fail(name + " cookie exists which is not expected");
                    return 1;
                }

            }
            catch
            {
                if (expected == false)
                {
                    report.Pass(name + " cookie does not exist as was expected");
                    return 0;
                }
                else
                {
                    report.Fail(name + " cookie does not exist which is not expected");
                    return 1;
                }
            }
        }

        public static int VerifyAlert(string expectedText)
        {
            try
            {
                if (driver.SwitchTo().Alert().Text.Contains(expectedText))
                {
                    report.Pass("Alert text contains " + expectedText);
                    return 0;
                }
                report.Fail("Alert text: " + driver.SwitchTo().Alert().Text + " does not contain the expected text: " +
                            expectedText);
                return 1;
            }
            catch (Exception e)
            {
                report.Fail("Unable to verify the alert: " + e.Message);
                return 1;
            }
        }

        public static void startup(TestContext TestContext)
        {
            Automation.config.testname = TestContext.TestName;
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--disable-extensions"); //Prevents popup due to extensions
            options.AddUserProfilePreference("credentials_enable_service", false);  //Prevent prompt to save user password
            options.AddUserProfilePreference("profile.password_manager_enabled", false); //Prevent prompt to save user password

            if (TestContext.Properties["__Tfs_TestConfigurationName__"] != null) // This is only for MTM integration
            {
                string config = TestContext.Properties["__Tfs_TestConfigurationName__"].ToString();
                string buildDir = TestContext.Properties["BuildDirectory"].ToString();
                string testDir = $"{buildDir}";

                if (!string.IsNullOrEmpty(config))
                {
                    switch (config)
                    {
                        case "IE":
                            driver = new InternetExplorerDriver(testDir);
                            break;
                        case "Firefox":
                            FirefoxDriverService service = FirefoxDriverService.CreateDefaultService(testDir, "geckodriver.exe");
                            service.FirefoxBinaryPath = "C:\\Program Files (x86)\\Mozilla Firefox\\firefox.exe";
                            driver = new FirefoxDriver(service);
                            break;
                        case "Chrome":
                            driver = new ChromeDriver(testDir, options);
                            break;
                        case "Staging":
                            driver = new ChromeDriver(testDir, options);
                            env = config;
                            break;
                        case "Production":
                            driver = new ChromeDriver(testDir, options);
                            env = config;
                            break;
                        default:
                            driver = new ChromeDriver(testDir, options);
                            break;
                    }
                }
                else
                {
                    driver = new ChromeDriver(testDir, options);
                }
            }
            else // For all other options beside MTM integration
            {
                driver = new ChromeDriver(options);
            }

            if (env == "Staging")
            {
                url = ConfigurationManager.AppSettings.Get("url_staging");
            }

            if (env == "Production")
            {
                url = ConfigurationManager.AppSettings.Get("url_production");
                username = ConfigurationManager.AppSettings.Get("prod_username");
                password = ConfigurationManager.AppSettings.Get("prod_password");
            }

            vars = new testVars(driver);
        }

        public static void teardown()
        {
            driver.Quit();
            GlobalBrowser = vars.browsername;
            //This is a workaround to suppress the various Firefox crash prompt.
            if (GlobalBrowser == "firefox")
            {
                try
                {
                    //Process[] proc = Process.GetProcessesByName("plugin-container");
                    Process[] proc = Process.GetProcessesByName("WerFault");
                    proc[0].Kill();
                }
                catch (Exception e) { Console.WriteLine("Log out of Firefox", e); }
            }

            vars.verify();
        }

        public static string ListToString(List<IWebElement> list)
        {
            var result = "";
            foreach (var item in list)
            {
                result += item.Text + ", ";
            }
            return result;
        }

        public static mdTable GetTable(By by)
        {
            WaitForElement(by);
            WaitForPageToLoad();
            return new mdTable(driver.FindElement(by));
        }

        public static void zoomOut()
        {
            System.Windows.Forms.SendKeys.SendWait("^-");
        }

        public static void zoomIn()
        {
            System.Windows.Forms.SendKeys.SendWait("^=");
        }

        public static void Enter()
        {
            System.Windows.Forms.SendKeys.SendWait("{ENTER}");
        }

        public static void Tab()
        {
            System.Windows.Forms.SendKeys.SendWait("{TAB}");
        }

        public static void FailAndRecover(string message, Exception e = null)
        {
            if (e != null)
            {
                report.Fail(message, e);
            }
            else
            {
                report.Fail(message);
            }
            Recover();
        }

        public static void Recover()
        {
            //accept alert
            try
            {
                driver.SwitchTo().Alert().Accept();
                report.Step("Recover: accepted alert");
            }
            catch
            {

            }

            //close the md-select element
            try
            {
                Tab();
            }
            catch
            {

            }

            //Yes to confirmation dialog
            try
            {
                if (doesElementExist(By.XPath("//button[@ng-click='dialog.hide()']")))
                {
                    Click(driver, By.XPath("//button[@ng-click='dialog.hide()']"));
                    report.Step("Recover: accepted confirmation dialog");
                }
            }
            catch
            {

            }

            //Cancel edit dialog
            try
            {
                if (doesElementExist(By.XPath("//button[@ng-click='vm.cancel()']")))
                {
                    Click(driver, By.XPath("//button[@ng-click='vm.cancel()']"));
                    report.Step("Recover: cancelled edit dialog");
                }
            }
            catch
            {

            }
        }
    }
}
