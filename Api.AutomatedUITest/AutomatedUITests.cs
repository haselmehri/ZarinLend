using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using WebFramework.Api;
using Xunit;

namespace Api.AutomatedUITest
{
    public class AutomatedUITests : IDisposable
    {
        private readonly IWebDriver _driver;
        public AutomatedUITests()
        {
            _driver = new FirefoxDriver();
            //_driver = new ChromeDriver();
        }

        #region Login form

        [Fact]
        public void Login_WhenFailed_WhenUsernameOrPasswordIsEmpty()
        {
            _driver.Navigate().GoToUrl("http://localhost:99/login");

            _driver.FindElement(By.Id("btnLogin"))
                .Click();

            var emailError = _driver.FindElement(By.Id("Email-error"))
                .Text;

            var passwordError = _driver.FindElement(By.Id("Password-error"))
                .Text;

            Assert.True(!string.IsNullOrWhiteSpace(emailError) || !string.IsNullOrWhiteSpace(passwordError));
        }

        [Fact]
        public void Login_WhenSuccessed()
        {
            _driver.Navigate().GoToUrl("http://localhost:99/login");

            _driver.FindElement(By.Id("Email"))
                .SendKeys("admin@site.com");

            _driver.FindElement(By.Id("Password"))
                .SendKeys("123456A1!");

            _driver.FindElement(By.Id("btnLogin"))
                .Click();
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => driver.Url == "http://localhost:99/" || driver.Url == "http://localhost:99/home/index");

            var title = _driver.Title;
            Assert.Equal("صفحه اصلی", _driver.Title);
        }

        bool IsAlertShown(IWebDriver driver)
        {
            try
            {
                driver.SwitchTo().Alert();
            }
            catch (NoAlertPresentException e)
            {
                return false;
            }
            return true;
        }

        [Fact]
        public void Login_WhenFailed()
        {
            var ajaxResponseId = "hdnAjaxResponse";
            _driver.Navigate().GoToUrl("http://localhost:99/login");

            _driver.FindElement(By.Id("Email"))
                .SendKeys("admin@site.com");

            _driver.FindElement(By.Id("Password"))
                .SendKeys("sdbfjsdjfndjf!");//wrong password

            _driver.FindElement(By.Id("btnLogin"))
                .Click();
            bool isAjaxFinished1 = (bool)((IJavaScriptExecutor)_driver).ExecuteScript("return jQuery.active == 0");
            long ajaxActiveConnection = (long)((IJavaScriptExecutor)_driver).ExecuteScript("return jQuery.active");

            //wait.Until(driver => driver.FindElement(By.Id(ajaxResponseId)).GetAttribute("value") != string.Empty);
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
            wait.Until(_driver => IsAlertShown(_driver));
            //wait.IgnoreExceptionTypes(typeof(UnhandledAlertException));
            var alert = _driver.SwitchTo().Alert();
            var alert_message = alert.Text;
            alert.Accept();
            bool isAjaxFinished2 = (bool)((IJavaScriptExecutor)_driver).ExecuteScript("return jQuery.active == 0");
            long ajaxActiveConnection2 = (long)((IJavaScriptExecutor)_driver).ExecuteScript("return jQuery.active");
            IWebElement firstResult = _driver.FindElement(By.Id(ajaxResponseId));
            var result = firstResult.GetAttribute("value");
            var loginResult = JsonConvert.DeserializeObject<ApiResult>(result);

            Assert.False(loginResult.IsSuccess);
        }

        #endregion

        #region Post form


        [Fact]
        public void AddPost_WhenFailed_WhenInputFieldsIsEmpty()
        {
            _driver.Navigate().GoToUrl("http://localhost:99/post/add");
            var formTitle = "ثبت پست جدید";

            _driver.FindElement(By.Id("btnRegister"))
                .Click();

            var titleError = _driver.FindElement(By.Id("Title-error"))
                .Text;

            var categoryIdError = _driver.FindElement(By.Id("CategoryId-error"))
                .Text;

            var authorIdError = _driver.FindElement(By.Id("AuthorId-error"))
                .Text;

            Assert.True((!string.IsNullOrWhiteSpace(titleError) ||
                        !string.IsNullOrWhiteSpace(categoryIdError) ||
                        !string.IsNullOrWhiteSpace(authorIdError)) &&
                        formTitle == _driver.Title);
        }

        [Fact]
        public void AddPost_WhenSuccess()
        {
            var ajaxResponseId = "hdnAjaxResponse";
            var pageLoad = _driver.Manage().Timeouts().PageLoad;
            var iImplicitWait = _driver.Manage().Timeouts().ImplicitWait;
            _driver.Navigate().GoToUrl("http://localhost:99/post/add");

            var postTitle = "پست شماره " + new Random().Next(100, 1000);
            _driver.FindElement(By.Id("Title"))
                .SendKeys(postTitle);

            _driver.FindElement(By.Id("Description"))
                .SendKeys("ندارد");

            var categoryDropDownList = new SelectElement(_driver.FindElement(By.Id("CategoryId")));
            categoryDropDownList.SelectByText("دسته بندی اولیه 2");
            //categoryDropDownList.SelectByValue("2");
            //categoryDropDownList.SelectByIndex(1);

            var authorDropDownList = new SelectElement(_driver.FindElement(By.Id("AuthorId")));
            authorDropDownList.SelectByIndex(1);

            _driver.FindElement(By.Id("btnRegister"))
                .Click();

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
            wait.Until(_driver => IsAlertShown(_driver));
            var alert = _driver.SwitchTo().Alert();
            //var alert_message = alert.Text;
            alert.Accept();

            IWebElement firstResult = _driver.FindElement(By.Id(ajaxResponseId));
            var jsonResult = firstResult.GetAttribute("value");
            //var result = JsonConvert.DeserializeObject<ApiResult<PostSelectDto>>(jsonResult);

            //Assert.True(result.IsSuccess && result.Data.Title == postTitle);
        }

        #endregion

        [Fact]
        public void Google_Search()
        {
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            _driver.Navigate().GoToUrl("https://www.google.com/ncr");
            _driver.FindElement(By.Name("q")).SendKeys("cheese" + Keys.Enter);
            wait.Until(driver => driver.FindElement(By.Id("result-stats")).Displayed);
            IWebElement firstResult = _driver.FindElement(By.Id("result-stats"));
            var result = firstResult.GetAttribute("textContent");
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
