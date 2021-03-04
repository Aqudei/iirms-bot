using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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
using MahApps.Metro.Controls.Dialogs;

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
        private readonly RestClient _client = new RestClient();
        // Check if folder exists
        // {BaseUrl}/api/Encoding/checkFolder [GET]
        // {BaseUrl}/api/Encoding/upload [POST] params := file [binary]
        // Add doc info
        // {BaseUrl}/api/Encoding/new [POST] params 
        /*
            DocumentId: "RECDCRO8-PROD-ALMINENDWOJDUFHY"
            FullText: "FIELD OPERATIONS REPORT S E C R E T AFTER ACTIVITY REPORT OFFICE OF ORIGIN : RO-08 FOR : LAD/SPAD, DII REPORT CNR : RO0820G181.AAR DATE OF REPORT : 09 JULY 2020 REFERENCES : RO8 LEGAL INITIATIVE ON CTG CHILD WARRIORS This is in line with the RO’s legal offensive on Communist Terrorist Group (CTG) child warriors. 080700H July 2020, RO8 Legal Desk Officers OP-843 and CI-861 left RO base in on board the ROs Toyota Hilux pick-up and proceeded to Headquarters, 802nd Infantry Brigade, 8th Infantry Division, Philippine Army (H802IB, 8ID, PA) in Camp Downes, Ormoc City. At 0930H, the duo arrived at Camp Downes and met Cpt Raagas who was tasked by Cpt Annie Lorraine Calcatan, S2 802Bde to attend to the RO legal team. OP-843 and CI-861 were ushered and introduced to the CTG surrenderee Realyn Lombog, 16 yrs old, Medical Aide, Squad Baking and Ruffa Correa-Lumbog, 19 yrs old, Finance Aide, Squad Abe, both of Platoon 1 (CN: Pingkoy), Sub-Regional Committee (SRC) LEVOX, Eastern Visayas Regional Party Committee (EVRPC). Prior to the interview, the legal desk officers accomplished the Former Rebel Information Sheet (FRIS) icow inputs obtained from the duo and have them signed it. After the interview, the duo agreed to execute affidavit statement and serve as witnesses that will be filed in court against the CTG on recruitment of child warriors under RA 9851 or RA 11188. The duo also provided significant information concerning the composition, leadership, disposition and armaments of Platoon 1 (CN: Pingkoy). The duo also shared circumstances on their recruitment as child combatants and their experience of molestation from a ranking CTG Officer and comrade. After the interview, OP-843 and CI-861 paid a courtesy call to Col Sozimo Oliveros (GSC) PA, Commanding Officer, 802 Bde, 8ID, PA. Col Oliveros appreciated the legal initiative of NICA RO8 under the platform of Situation Awareness and Knowledge Management (SAKM) and Legal Cooperation Clusters (LCC) of RTF ELCAC. The activity ended at 1430H, same day. OP-843 and CI-861 left Ormoc City at 1530H bound for Tacloban City. OP-843 and CI-861 at RO base at 1800H of same day. COMMENTS: 1. Report prepared by OP-843; reviewed by: OP-811; processed by: RO-859. 2. OP-843 and CI-861 incurred a total of PhP2,900.00 during the conduct of operational activity, broken down into PhP900.00 for meals of OP-843 and CI-861, PhpP1,000.00 for diesel, and PhP1,000.00 as monetary incentive given to both former rebels @ PhP500.00 each. 3. OO’s comments: a. The information gathered from both FRs will be submitted in the form of SOIs with FRIS attachment. Said information will likewise be converted into a judicial affidavit. b. The two (2) FRs are cooperative during the interview. 4. No problem on security was encountered during the whole duration of this activity. 5. Attached are the photo documentations. Distribution: 1-DII / 2-File ALEX MIZON ATTACHMENT OP-843 (interviewer) and Ruffa Correa-Lumbog @ Ernie/Roselyn/Geraldine, Finance Aide, Sqd Abe, Platoon 1 (CN: Pingkoy), SRC Levox, EVRPC (FR/interviewee) CI-861 (interviewer) and Realyn Lombog @ Shien, Medical Aide, Sqd Baking, Platoon 1 (CN: Pingkoy), SRC Levox, EVRPC (FR/interviewee) Courtesy Call with Col Sozimo Oliveros (GSC) PA, CO, 802nd Bde, 8ID, PA OP-843 (interviewer) and Ruffa Correa-Lumbog @ Ernie/Roselyn/Geraldine, Finance Aide, Sqd Abe, Platoon 1 (CN: Pingkoy), SRC Levox, EVRPC (FR/interviewee) Ruffa Correa-Lumbog @ Ernie/Roselyn/Geraldine, Finance Aide, Sqd Abe, Platoon 1 (CN: Pingkoy), SRC Levox, EVRPC S E C R E T Page 4 of 7 pages Copy 1 of 2 copies"
            InfoAccId: "37"
            OriginId: "D018"
            ReportDate: "2020-09-06T16:00:00.000Z"
            ReportNo: "RO0820G181.AAR"
            ReportType: "D018-R024"
            SecClassId: "2"
            SourceId: "D018"
            SubjectTitle: "AFTER ACTIVITY REPORT"
            WrittenBy: "Aqudei"
         */

        public string CurrentError
        {
            get => _currentError;
            set
            {
                Set(ref _currentError, value);
                NotifyOfPropertyChange(nameof(HasCurrentError));
            }
        }

        public string BaseUrl { get; set; } = "https://orange-green.tk";

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

        public bool IsFileServerAvailable
        {
            get
            {
                var url = $"{BaseUrl}/api/Encoding/checkFolder";
                var req = new RestRequest(url, Method.GET);
                var response = _client.Execute(req);
                return true;
            }
        }
        public bool CanRunBot => !IsLoading;
        public bool CanImport => !IsLoading;

        //public async void RunBot()
        //{
        //    IsLoading = true;

        //    await Task.Run(() =>
        //    {
        //        ChromeOptions options = new ChromeOptions();
        //        options.AddArguments("--disable-notifications"); // to disable notification
        //        using (_driver = new ChromeDriver(options))
        //        {
        //            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(120));
        //            DoLogin();

        //            foreach (var item in Items) DoEncode(item);

        //            Debug.WriteLine("Bot done.");
        //        }
        //    });

        //    IsLoading = false;
        //}

        public IEnumerable<IResult> RunBot()
        {
            yield return Task.Run(async () =>
           {
               IsLoading = true;
               try
               {
                   var options = new ChromeOptions();
                   options.AddArguments("--disable-notifications"); // to disable notification
                   using (_driver = new ChromeDriver(options))
                   {
                       _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(120));
                       await DoLogin();
                   }

                   foreach (var item in Items)
                   {
                       var uploadResult = UploadFile(item);
                       if (uploadResult == null) continue;

                       var result = SubmitDocumentInfo(uploadResult, item);
                       if (result == null)
                       {
                           continue;
                       }
                   }

                   Debug.WriteLine("Bot done.");

               }
               finally
               {
                   IsLoading = false;
               }

           }).AsResult();

        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                Set(ref _isLoading, value);
                NotifyOfPropertyChange(nameof(CanRunBot));
                NotifyOfPropertyChange(nameof(CanImport));
            }
        }

        private int _importedCount = 0;

        public int ImportedCount
        {
            get { return _importedCount; }
            set { Set(ref _importedCount, value); }
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

        public IEnumerable<IResult> Reset()
        {
            yield return Task.Run(() =>
            {
                try
                {
                    IsLoading = true;
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
                finally
                {
                    ImportedCount = Items.Count;
                    IsLoading = false;
                }
            }).AsResult();

        }

        public IEnumerable<IResult> RemoveSuccessful()
        {
            yield return Task.Run(() =>
            {
                try
                {
                    IsLoading = true;
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
                    ImportedCount = Items.Count;
                }
                finally
                {
                    IsLoading = false;
                }
            }).AsResult();
        }

        private void WaitForAlert()
        {
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    _wait.Until(EC.AlertIsPresent()).Dismiss();
                    Thread.sleep(2000);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void DoEncode(Item encodeItem)
        {
            try
            {
                Execute.OnUIThread(() => encodeItem.ItemStatus = Item.ITEM_STATUS.READING);

                var report = _masterBuilder.Build(encodeItem.FileName);
                if (CheckDuplicate(report[KnownReportParts.PART_CNR]))
                {
                    Execute.OnUIThread(() =>
                    {
                        encodeItem.ItemStatus = Item.ITEM_STATUS.SUCCESS;
                        encodeItem.Error = "This item already exists in IIRMS";
                    });

                    return;
                }

                //var encodeUrl = "{BaseUrl}k/Encoder/Document";
                // var encodeUrl1 = "{BaseUrl}k";
                var encodeUrl2 = $"{BaseUrl}/Encoder#/Docs";
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
                element.SendKeys(report[KnownReportParts.PART_DATEOFREPORT_STR].Replace("-", ""));
                Debug.WriteLine("Report Date: {0}", new object[] { report[KnownReportParts.PART_DATEOFREPORT_STR] });

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
                WaitForAlert();
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
                _driver.Navigate().Refresh();
            }
        }

        public bool HasCurrentError => !string.IsNullOrWhiteSpace(CurrentError);

        public void OpenDestination()
        {
            Process.Start("explorer.exe", _dataDirectory);
        }

        private bool CheckDuplicate(string reportNumber)
        {
            var url =
                $"{BaseUrl}/api/Encoding/reportnumber?reportNumber={reportNumber}";
            var request = new RestRequest(url, Method.GET);
            var result = _client.Execute<DuplicateResult>(request);
            return result.Data.Duplicate;
        }

        private Task DoLogin()
        {
            return Task.Run(async () =>
            {
                _driver.Navigate().GoToUrl($"{BaseUrl}/Account/Login?ReturnUrl=%2F");
                var element = _wait.Until(d => d.FindElement(By.Name(Webpage.LOGIN_USERNAME)));
                element.SendKeys(UserName);
                element = _wait.Until(EC.ElementExists(By.Name(Webpage.LOGIN_PASSWORD)));
                element.SendKeys(Password);
                element.Submit();

                var code = await _dialogCoordinator.ShowInputAsync(this, "Login", "Please enter 2FA code:");

                element = _wait.Until(EC.ElementExists(By.Name(Webpage.LOGIN_2FA)));
                //element.SendKeys(TwoFactor);
                element.SendKeys(code);
                element.Submit();
                _wait.Until(EC.ElementExists(By.ClassName("profile_img")));
                _client.CookieContainer = new CookieContainer();
                foreach (var cookie in _driver.Manage().Cookies.AllCookies)
                {
                    _client.CookieContainer.Add(new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
                }
            });


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

        public IEnumerable<IResult> Import()
        {
            yield return Task.Run(() =>
             {
                 IsLoading = true;

                 try
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
                 finally
                 {
                     IsLoading = false;
                 }

             }).AsResult();

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

                Execute.OnUIThread(() => Items.Add(newItem));
            }

            Execute.OnUIThread(() => ImportedCount = Items.Count);
        }

        private readonly Lookups _lookups = new Lookups();
        private readonly IDialogCoordinator _dialogCoordinator;

        private NewResult SubmitDocumentInfo(UploadResult uploadResult, Item encodeItem)
        {
            try
            {
                Execute.OnUIThread(() => encodeItem.ItemStatus = Item.ITEM_STATUS.READING);

                var report = _masterBuilder.Build(encodeItem.FileName);
                if (CheckDuplicate(report[KnownReportParts.PART_CNR]))
                {
                    Execute.OnUIThread(() =>
                    {
                        encodeItem.ItemStatus = Item.ITEM_STATUS.FAILURE;
                        encodeItem.Error = "This item already exists in IIRMS";
                    });

                    return null;
                }

                var encodePayload = new EncodePayload
                {
                    SubjectTitle = report[KnownReportParts.PART_SUBJECT],
                    ReportNo = report[KnownReportParts.PART_CNR],
                    WrittenBy = WrittenBy,
                    ReportDate = report[KnownReportParts.PART_DATEOFREPORT_UTC],
                    SourceId = _lookups.Offices[SourceOffice],
                    InfoAccId = _lookups.Accuracy.ContainsKey(report[KnownReportParts.PART_EVALUATION])
                        ? _lookups.Accuracy[report[KnownReportParts.PART_EVALUATION]]
                        : "DOC",
                    DocumentId = uploadResult.Id,
                    ReportType = _lookups.ReportTypes[report[KnownReportParts.REPORTTYPE]],
                    SecClassId = _lookups.SecurityClass[SecurityClassification],
                    OriginId = _lookups.Offices[OriginOffice],
                    FullText = report[KnownReportParts.PART_BODY]
                };

                var url = $"{BaseUrl}/api/Encoding/new";
                var request = new RestRequest(url, Method.POST);
                request.AddJsonBody(encodePayload);
                var response = _client.Execute<NewResult>(request);
                if (!response.IsSuccessful)
                {
                    throw new Exception($"Failed to add document info for file: {uploadResult.Id}");
                }

                Execute.OnUIThread(() =>
                {
                    encodeItem.ItemStatus = Item.ITEM_STATUS.SUCCESS;
                    encodeItem.Error = "";
                });
                return response.Data;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                Execute.OnUIThread(() =>
                {
                    encodeItem.ItemStatus = Item.ITEM_STATUS.FAILURE;
                    encodeItem.Error = e.Message;
                });
                return null;
            }
        }

        private UploadResult UploadFile(Item item)
        {
            Execute.OnUIThread(() =>
            {
                item.ItemStatus = Item.ITEM_STATUS.UPLOADING;
            });

            if (!IsFileServerAvailable)
            {
                Execute.OnUIThread(() =>
                {
                    item.ItemStatus = Item.ITEM_STATUS.FAILURE;
                    item.Error = "File Server is not Online! Attachment will not be uploaded";
                });

                return null;
            }

            var regex = new Regex(@"\b[a-z0-9\-]+?\d\d[a-l]\d+\.[a-z]+", RegexOptions.IgnoreCase);
            var match = regex.Match(item.FileName);
            if (!match.Success)
            {
                Execute.OnUIThread(() =>
                {
                    item.ItemStatus = Item.ITEM_STATUS.FAILURE;
                    item.Error = "Unable to find CNR";
                });

                return null;
            }

            if (CheckDuplicate(match.Value))
            {
                Execute.OnUIThread(() =>
                {
                    item.ItemStatus = Item.ITEM_STATUS.SUCCESS;
                    item.Error = "Report already exists in IIRMS database!";
                });

                return null;
            }

            var request = new RestRequest($"{BaseUrl}/api/Encoding/upload", Method.POST);
            request.AddFile("file", item.FileName);
            var result = _client.Execute<UploadResult>(request);
            return result.IsSuccessful ? result.Data : null;
        }

        public EncoderViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;

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