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


        public void BrowseFile()
        {
            var ofd = new OpenFileDialog
            {
                Multiselect = false
            };

            var result = ofd.ShowDialog();
            if (result.HasValue && result.Value)
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

        public void Run()
        {
            var report = _masterReportBuilder.Build(File);
            var sb = new StringBuilder();
            foreach (var key in report.Keys)
            {
                sb.AppendLine($"{key}\t:\t{report[key]}");
            }

            Display = sb.ToString();
        }

        public bool CanRun => !string.IsNullOrWhiteSpace(File);
    }
}
