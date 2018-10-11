using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WellsFargo
{
    class Program
    {
        static void Main(string[] args)
        {

        }

        [SetUp]
        public void Start()
        {
              PropertyCollections.driver = new ChromeDriver();
            PropertyCollections.driver.Navigate().GoToUrl("https://www.wellsfargo.com/");
        }

        [Test]
        public void Operation()
        {

        }

        [TearDown]
        public void CleanUp()
        {
            PropertyCollections.driver.Quit();
        }

    }
}
