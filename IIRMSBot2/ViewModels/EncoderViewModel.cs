﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using IIRMSBot2.Model;
using IIRMSBot2.ReportBuilders;
using ikvm.extensions;
using java.lang;
using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using RestSharp;
using Cookie = System.Net.Cookie;
using Exception = System.Exception;
using Process = System.Diagnostics.Process;
using EC = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace IIRMSBot2.ViewModels
{
    internal sealed class EncoderViewModel : Screen
    {
        private string _username;
        private string _password;
        private ChromeDriver _driver;
        private WebDriverWait _wait;
        private string _twoFactor;
        private readonly string _dataDirectory;
        private string _securityClassification = "CONFIDENTIAL";
        private string _originOffice = "NICA-RO-08";
        private bool _isLoading;
        private string _currentError;
        private Item _selectedItem;
        private RestClient _client = new RestClient();

        public string CurrentError
        {
            get => _currentError;
            set
            {
                Set(ref _currentError, value);
                NotifyOfPropertyChange(nameof(HasCurrentError));
            }
        }

        public string BaseUrl { get; set; } = "https://orange-green.cf";

        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                Set(ref _selectedItem, value);
                CurrentError = value?.Error;
            }
        }

        public string UserName
        {
            get => _username;
            set => Set(ref _username, value);
        }



        public ObservableCollection<Item> Items { get; set; }
            = new ObservableCollection<Item>();

        private string _sourceOffice = "NICA-RO-08";

        public string SecurityClassification
        {
            get => _securityClassification;
            set => Set(ref _securityClassification, value);
        }

        public string SourceOffice
        {
            get => _sourceOffice;
            set => Set(ref _sourceOffice, value);
        }

        public string OriginOffice
        {
            get => _originOffice;
            set => Set(ref _originOffice, value);
        }

        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        private readonly MasterReportBuilder _masterBuilder;


        private string _writtenBy = "Aqudei";

        public string WrittenBy
        {
            get => _writtenBy;
            set => Set(ref _writtenBy, value);
        }


        public bool CanRunBot => !IsLoading && !string.IsNullOrWhiteSpace(TwoFactor);

        public async void RunBot()
        {
            IsLoading = true;

            await Task.Run(() =>
            {
                ChromeOptions options = new ChromeOptions();
                options.AddArguments("--disable-notifications"); // to disable notification
                using (_driver = new ChromeDriver(options))
                {
                    _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(120));
                    DoLogin();

                    foreach (var item in Items) DoEncode(item);

                    Debug.WriteLine("Bot done.");
                }
            });

            IsLoading = false;
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                Set(ref _isLoading, value);
                NotifyOfPropertyChange(nameof(CanRunBot));
            }
        }


        public string TwoFactor
        {
            get => _twoFactor;
            set
            {
                Set(ref _twoFactor, value);
                NotifyOfPropertyChange(nameof(CanRunBot));
            }
        }


        public void Reset()
        {
            foreach (var item in Items)
                try
                {
                    File.Delete(item.FileName);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }

            Items.Clear();
        }

        public void RemoveSuccessful()
        {
            var removables = new List<Item>();

            foreach (var item in Items.Where(item => item.ItemStatus == Item.ITEM_STATUS.SUCCESS))
                try
                {
                    File.Delete(item.FileName);
                    removables.Add(item);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }

            foreach (var removable in removables) Items.Remove(removable);
        }

        private void WaitForAlert()
        {
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    _wait.Until(EC.AlertIsPresent()).Dismiss();
                }
            }
            catch (Exception)
            {
            }
        }

        private void DoEncode(Item encodeItem)
        {
            try
            {
                Execute.OnUIThread(() => encodeItem.ItemStatus = Item.ITEM_STATUS.READING);

                var report = _masterBuilder.Build(encodeItem.FileName);
                if (CheckExists(report[KnownReportParts.PART_CNR]))
                    return;

                //var encodeUrl = "https://orange-green.cf/Encoder/Document";
                // var encodeUrl1 = "https://orange-green.cf";
                var encodeUrl2 = "https://orange-green.cf/Encoder#/Docs";
                //_driver.Navigate().GoToUrl(encodeUrl1);
                _driver.Navigate().GoToUrl(encodeUrl2);
                _wait.Until(EC.ElementExists(By.LinkText("New Record"))).Click();


                var element = _wait.Until(driver => driver.FindElement(By.Name(Webpage.ENCODER_SUBJECT)));
                element.SendKeys(report[KnownReportParts.PART_SUBJECT]);

                element = _wait.Until(driver => driver.FindElement(By.Name(Webpage.ENCODER_REPORT_NR)));
                element.SendKeys(report[KnownReportParts.PART_CNR]);

                element = _wait.Until(driver => driver.FindElement(By.Name(Webpage.ENCODER_WRITTEN_BY)));
                element.SendKeys(WrittenBy);

                element = _wait.Until(driver => driver.FindElement(By.Name(Webpage.ENCODER_REPORT_DATE)));
                element.SendKeys(report[KnownReportParts.PART_DATEOFREPORT].Replace("-", ""));
                Debug.WriteLine("Report Date: {0}", new object[] { report[KnownReportParts.PART_DATEOFREPORT] });

                element = _wait.Until(EC.ElementExists(By.CssSelector($"select[ng-model='{Webpage.ENCODER_SOURCE_OFFICE}']")));
                var selectElement = new SelectElement(element);
                selectElement.SelectByText(SourceOffice);

                element = _wait.Until(EC.ElementExists(By.CssSelector($"select[ng-model='{Webpage.ENCODER_ACCURACY}']")));
                selectElement = new SelectElement(element);
                selectElement.SelectByText(report[KnownReportParts.PART_EVALUATION]);

                element = _wait.Until(driver => driver.FindElement(By.Name(Webpage.ENCODER_FILE)));
                element.SendKeys(encodeItem.FileName);

                Thread.sleep(2000);
                element = _wait.Until(EC.ElementExists(By.CssSelector($"select[ng-model='{Webpage.ENCODER_REPORT_TYPE}']")));
                selectElement = new SelectElement(element);
                selectElement.SelectByText(report[KnownReportParts.REPORTTYPE]);

                element = _wait.Until(EC.ElementExists(By.CssSelector($"select[ng-model='{Webpage.ENCODER_SECURITY_CLASSIFICATION}']")));
                selectElement = new SelectElement(element);
                selectElement.SelectByText(SecurityClassification);

                element = _wait.Until(EC.ElementExists(By.CssSelector($"select[ng-model='{Webpage.ENCODER_ORIGIN_OFFICE}']")));
                selectElement = new SelectElement(element);
                selectElement.SelectByText(OriginOffice);

                element = _wait.Until(driver => driver.FindElement(By.Name(Webpage.ENCODER_FULLTEXT)));
                //new Utils.SetClipboardHelper(report[KnownReportParts.PART_BODY]).Go();
                element.SendKeys(report[KnownReportParts.PART_BODY]);
                //element.SendKeys(OpenQA.Selenium.Keys.Control + 'v' );
                encodeItem.ItemStatus = Item.ITEM_STATUS.UPLOADING;
                element.Submit();

                //_wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.CssSelector("button.close")));
                Execute.OnUIThread(() =>
                {
                    encodeItem.ItemStatus = Item.ITEM_STATUS.SUCCESS;
                    encodeItem.Error = "";
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                Execute.OnUIThread(() =>
                {
                    encodeItem.ItemStatus = Item.ITEM_STATUS.FAILURE;
                    encodeItem.Error = e.Message;
                });
            }
            finally
            {
                WaitForAlert();
                _driver.Navigate().Refresh();
            }
        }

        public bool HasCurrentError => !string.IsNullOrWhiteSpace(CurrentError);

        public void OpenDestination()
        {
            Process.Start("explorer.exe", _dataDirectory);
        }

        private bool CheckExists(string reportNumber)
        {
            var url =
                $"https://orange-green.cf/api/RECORD/getdoc/?dateend=&datestart=&itemsPerPage=20&page=1&region=&reportnumber={reportNumber}&reverse=false&search=&sortBy=SubjectTitle";
            var request = new RestRequest(url, Method.GET);
            var result = _client.Execute<CheckResult>(request);
            return result.IsSuccessful && result.Data.Count > 0;
        }

        private void DoLogin()
        {
            _driver.Navigate().GoToUrl("https://orange-green.cf/Account/Login?ReturnUrl=%2F");
            var element = _wait.Until(d => d.FindElement(By.Name(Webpage.LOGIN_USERNAME)));
            element.SendKeys(UserName);
            element = _wait.Until(EC.ElementExists(By.Name(Webpage.LOGIN_PASSWORD)));
            element.SendKeys(Password);
            element.Submit();

            element = _wait.Until(EC.ElementExists(By.Name(Webpage.LOGIN_2FA)));
            element.SendKeys(TwoFactor);
            element.Submit();
            _wait.Until(EC.ElementExists(By.LinkText("Documents")));
            _client.CookieContainer = new CookieContainer();
            foreach (var cookie in _driver.Manage().Cookies.AllCookies)
            {
                _client.CookieContainer.Add(new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
            }
        }

        public void SaveLogin()
        {
            var botConfig = BotConfig.Get();
            botConfig.UserName = UserName;
            botConfig.Password = Password;
            botConfig.OriginOffice = OriginOffice;
            botConfig.SourceOffice = SourceOffice;
            botConfig.SecurityClassification = SecurityClassification;
            botConfig.WrittenBy = WrittenBy;
            botConfig.Save();
        }

        public void Import()
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Document Files |*.doc*"
            };

            var result = dialog.ShowDialog();
            if (!result.HasValue || !result.Value)
                return;

            foreach (var item in dialog.FileNames)
                File.Copy(item, Path.Combine(_dataDirectory, Path.GetFileName(item)), true);

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
            SourceOffice = botConfig.SourceOffice;
            OriginOffice = botConfig.OriginOffice;
            SecurityClassification = botConfig.SecurityClassification;
            WrittenBy = botConfig.WrittenBy;

            _masterBuilder = new MasterReportBuilder();

            _dataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "BotDir");
            Directory.CreateDirectory(_dataDirectory);

            LoadDocuments();
        }
    }
}