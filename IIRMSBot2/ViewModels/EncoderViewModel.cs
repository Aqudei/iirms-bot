using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using IIRMSBot2.Model;
using IIRMSBot2.ReportBuilders;
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
            get => _password;
            set => Set(ref _password, value);
        }

        private readonly MasterReportBuilder _masterBuilder;

        public void RunBot()
        {
            using (_driver = new ChromeDriver())
            {
                _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(120));
                DoLogin();

                foreach (var item in Items)
                {
                    DoEncode(item);
                }

                Debug.WriteLine("Bot done.");
            }
        }

        private string _twoFactor;
        private string _dataDirectory;

        public string TwoFactor
        {
            get => _twoFactor;
            set => Set(ref _twoFactor, value);
        }


        private void DoEncode(Item encodeItem)
        {
            var report = _masterBuilder.Build(encodeItem.FileName);

            var encodeUrl = "https://orange-green.tk/Encoder/Document";
            _driver.Navigate().GoToUrl(encodeUrl);

            var element = _wait.Until(driver => driver.FindElement(By.Name(Webpage.ENCODER_SUBJECT)));
            element.SendKeys(report[KnownReportParts.PART_SUBJECT]);

            element = _wait.Until(driver => driver.FindElement(By.Name(Webpage.ENCODER_REPORT_NR)));
            element.SendKeys(report[KnownReportParts.PART_CNR]);

            element = _wait.Until(driver => driver.FindElement(By.Name(Webpage.ENCODER_REPORT_DATE)));
            element.SendKeys(report[KnownReportParts.PART_DATEOFREPORT]);
            Debug.WriteLine("Report Date: {0}", new object[] { report[KnownReportParts.PART_DATEOFREPORT] });

            element = _wait.Until(driver => driver.FindElement(By.Name(Webpage.ENCODER_REPORT_DATE)));
            element.SendKeys(report[KnownReportParts.PART_DATEOFREPORT]);
        }

        private void DoLogin()
        {
            _driver.Navigate().GoToUrl("https://orange-green.tk/Account/Login?ReturnUrl=%2F");
            var element = _wait.Until(d => d.FindElement(By.Name(Webpage.LOGIN_USERNAME)));
            element.SendKeys(UserName);
            element = _wait.Until(d => d.FindElement(By.Name(Webpage.LOGIN_PASSWORD)));
            element.SendKeys(Password);
            element.Submit();

            element = _wait.Until(d => d.FindElement(By.Name(Webpage.LOGIN_2FA)));
            element.SendKeys(TwoFactor);
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
                File.Copy(item, Path.Combine(_dataDirectory, Path.GetFileName(item)), true);
            }

            LoadDocuments();
        }

        private void LoadDocuments()
        {
            var files = Directory.GetFiles(_dataDirectory, "*.doc*", SearchOption.TopDirectoryOnly)
                .Where(s => !s.Contains("~"));
            foreach (var item in files)
            {
                var newItem = new Item(item);
                if (Items.Contains(newItem))
                    continue;

                Items.Add(newItem);
            }
        }

        public EncoderViewModel()
        {
            DisplayName = "Auto-Encoder";
            var botConfig = BotConfig.Get();
            UserName = botConfig.UserName;
            Password = botConfig.Password;

            _masterBuilder = new MasterReportBuilder();

            _dataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BotDir");
            Directory.CreateDirectory(_dataDirectory);

            LoadDocuments();
        }
    }
}
