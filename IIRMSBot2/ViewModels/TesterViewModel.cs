using Caliburn.Micro;
using IIRMSBot2.ReportBuilders;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IIRMSBot2.ViewModels
{
    class TesterViewModel : Screen
    {
        private MasterReportBuilder _masterReportBuilder;

        private string _file;

        public string File
        {
            get { return _file; }
            set
            {
                Set(ref _file, value);
                NotifyOfPropertyChange(nameof(CanRun));
            }
        }

        public TesterViewModel()
        {
            DisplayName = "Tester";
            _masterReportBuilder = new MasterReportBuilder();
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { Set(ref _isBusy, value); }
        }

        public void BrowseFile()
        {
            var ofd = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog
            {
                Multiselect = false
            };

            var result = ofd.ShowDialog();
            if (result == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
            {
                File = ofd.FileName;
            }
        }

        private string _display;

        public string Display
        {
            get { return _display; }
            set { Set(ref _display, value); }
        }

        public IEnumerable<IResult> Run()
        {
            yield return Task.Run(() =>
            {
                IsBusy = true;
                try
                {
                    var report = _masterReportBuilder.Build(File);
                    var sb = new StringBuilder();
                    foreach (var key in report.Keys)
                    {
                        sb.AppendLine($"{key}\t:\t{report[key]}");
                    }

                    Display = sb.ToString();
                }
                finally
                {
                    IsBusy = false;
                }

            }).AsResult();
        }

        public bool CanRun => !string.IsNullOrWhiteSpace(File) && !IsBusy;
    }
}
