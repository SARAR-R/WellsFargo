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

namespace WellsFargo
{
   public class report
    {
        public static int _debuglevel = int.Parse(ConfigurationManager.AppSettings.Get("debuglevel") ?? "1");

        public static void Action(string action, string controlname, string value)
        {
            if (_debuglevel < 3)
            {
                string str;
                if (value == null)
                {
                    str = string.Format("> {0}: {1}", action, controlname);
                }
                else
                {
                    str = string.Format("> {0}: {1}, {2}", action, controlname, value);
                }
                Console.WriteLine(str);
            }
        }

        public static void Action(string action, string controlname)
        {
            Action(action, controlname, null);
        }

        public static void Step(string description)
        {
            if (_debuglevel < 4)
            {
                Console.WriteLine(string.Format("STEP: {0}", description));
            }
        }

        public static void Pass(string description)
        {
            if (_debuglevel < 2)
            {
                Console.WriteLine(string.Format("PASS: {0}", description));
            }
        }

        public static void Fail(string description)
        {
            string f = string.Format("FAIL: {0}", description);
            Console.WriteLine(f);
            Validate.vars.failures = Validate.vars.failures + Environment.NewLine + f;
            Validate.vars.errorcount = Validate.vars.errorcount + 1;
        }

        public static void Fail(string description, Exception e)
        {
            string f = string.Format("FAIL: {0}", description);
            Console.WriteLine(f);
            Console.WriteLine(e.Message);
            Validate.vars.failures = Validate.vars.failures + Environment.NewLine + f + ": " + e.Message;
            Validate.vars.errorcount = Validate.vars.errorcount + 1;
        }

        public static void Skip(string description)
        {
            string s = string.Format("SKIP: {0}", description);
            Console.WriteLine(s);
            Validate.vars.skips = Validate.vars.skips + Environment.NewLine + s;
            Validate.vars.skipcount = Validate.vars.skipcount + 1;
        }

    }
}
}
