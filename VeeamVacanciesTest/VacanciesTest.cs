using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using System.Threading;
using System.Linq;
using OpenQA.Selenium.Remote;
using System;

namespace VeeamVacanciesTest
{
    [TestFixture("Chrome", "96.0", "Windows 10", "1920x1080")]
    [Parallelizable(ParallelScope.Children)]
    public class VeeamTestSelenium
    {
        // Please insert your username and key here from https://www.lambdatest.com/
        static string username = "name";
        static string accessKey = "key";


        static string seleniumUri = "https://hub.lambdatest.com/wd/hub";

        ThreadLocal<IWebDriver> driver = new ThreadLocal<IWebDriver>();
        private string browser;
        private string version;
        private string os;
        private string resolution;

        public VeeamTestSelenium(string browser, string version, string os, string resolution)
        {
            this.browser = browser;
            this.version = version;
            this.os = os;
            this.resolution = resolution;
        }

        [SetUp]
        public void Init()
        {
            
            DesiredCapabilities capabilities = new DesiredCapabilities();
            capabilities.SetCapability("browserName", browser);
            capabilities.SetCapability("version", version);
            capabilities.SetCapability("platform", os);
            capabilities.SetCapability("resolution", resolution);


            capabilities.SetCapability("user", username);
            capabilities.SetCapability("accessKey", accessKey);

            driver.Value = new RemoteWebDriver(new Uri(seleniumUri), capabilities, TimeSpan.FromMinutes(2));
            Thread.Sleep(2000);
        }

        static object[] Parameters { get; } =
        {
            new object[] {"Разработка продуктов", new[] { "Английский" }, 6},
            new object[] {"Продажи",new[] { "Английский" }, 11},
            new object[] {"HR", new[] { "Английский", "Русский" }, 3},
        };

        [Test]
        [TestCaseSource(nameof(Parameters))]
        public void GetVacanciesCountByDepartmentAndLanguage(string department, string[] langs, int resultCount)
        {
            string targetUri = "https://careers.veeam.ru/vacancies";
            var driver = this.driver.Value; 

            driver.Manage().Window.Maximize();
            driver.Url = targetUri;
            Thread.Sleep(1000);

            var parameterBar = driver.FindElement(By.CssSelector("[class = 'col-12 col-lg-4']"));
            var buttonsParameterBar = parameterBar.FindElements(By.TagName("button"));
            buttonsParameterBar[0].Click();

            var productDevelopment = driver.FindElement(By.LinkText(department));
            productDevelopment.Click();
            buttonsParameterBar[1].Click();

            foreach (var lang in langs)
            {
                var langItem = driver.FindElements(By.CssSelector("[class = 'custom-control custom-checkbox']")).First(n => n.Text == lang);
                langItem.Click();
            }

            var vacancies = driver.FindElements(By.CssSelector("[class = 'card card-no-hover card-sm']"));
            Assert.AreEqual(resultCount, vacancies.Count);
            driver.Quit();
        }
    }
}