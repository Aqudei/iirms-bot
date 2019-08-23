using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using IIRMSBot2.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace IIRMSBot2.ViewModels
{
    sealed class EncoderViewModel : Screen
    {
        private string _baseUrl = "https://orange-green.tk";

        public string BaseUrl
        {
            get { return _baseUrl; }
            set { _baseUrl = value; }
        }

        private string _username;

        public string UserName
        {
            get { return _username; }
            set { Set(ref _username, value); }
        }

        private string _password;
        private ChromeDriver _driver;
        private WebDriverWait _wait;
        public ObservableCollection<Item> Items { get; set; }
            = new ObservableCollection<Item>();

        public string Password
        {
            get { return _password; }
            set { Set(ref _password, value); }
        }


        public void Start()
        {
            DoLogin();
        }

        private void DoLogin()
        {
            _driver.Navigate().GoToUrl("https://orange-green.tk/Account/Login?ReturnUrl=%2F");
            var element = _wait.Until(d => d.FindElement(By.Name(Webpage.LOGIN_USERNAME)));
            element.SendKeys(UserName);
            element = _wait.Until(d => d.FindElement(By.Name(Webpage.LOGIN_PASSWORD)));
            element.SendKeys(Password);
            element.Submit();
        }

        public void SaveLogin()
        {
            var botConfig = BotConfig.Get();
            botConfig.UserName = UserName;
            botConfig.Password = Password;
            botConfig.Save();
        }

        public void Import()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = "Document Files |*.doc*"
            };

            var result = dialog.ShowDialog();
            if (!result.HasValue || !result.Value)
                return;

            Items.Clear();
            foreach (var item in dialog.FileNames)
            {
                Items.Add(new Item
                {
                    FileName = item
                });
            }
        }


        public EncoderViewModel()
        {
            DisplayName = "Auto-Encoder";
            _driver = new ChromeDriver();
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(120));

            var botConfig = BotConfig.Get();
            UserName = botConfig.UserName;
            Password = botConfig.Password;
        }
    }
}
