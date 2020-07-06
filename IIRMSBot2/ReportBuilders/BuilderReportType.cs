using System.Collections.Generic;
using System.Text.RegularExpressions;
using TikaOnDotNet.TextExtraction;

namespace IIRMSBot2.ReportBuilders
{
    public class BuilderReportType : ReportBuilderBase, IReportBuilder
    {
        private readonly Regex _regex;
        private readonly Dictionary<string, string> _reportTypes = new Dictionary<string, string>
        {
            {"CI" ,"SUMMARY OF INFORMATION"},
            {"EEI" ,"REPLY TO EEI"},
            {"IR" ,"INFORMATION REPORT"},
            {"SI" ,"SUMMARY OF INFORMATION"},
            {"SOI" ,"SUMMARY OF INFORMATION"},
            {"AMR" ,"AFTER MEETING REPORT"},
            {"AAR" ,"AFTER ACTIVITY REPORT"},
            {"SDDP" ,"SUMMARY OF DISCUSSION AND DECISION POINTS (SDDP)"},
        };

        public BuilderReportType()
        {
            _regex = new Regex(@"\.([a-z]+)$", RegexOptions.IgnoreCase);
        }

        public void Build(Dictionary<string, string> report, TextExtractionResult rawInputBody)
        {
            if (report.ContainsKey(KnownReportParts.PART_CNR) == false)
                throw new PartNotFoundException("CNR must exist to determine report type");

            var cnr = report[KnownReportParts.PART_CNR];
            var match = _regex.Match(cnr);

            if (match.Success)
            {

                var key = match.Groups[1].Value.Trim().ToUpper();
                var reportType = _reportTypes[key];
                report.Add(KnownReportParts.REPORTTYPE, RemoveRedundantSpaces(reportType));
            }
            else
            {
                if (_required)
                    throw new PartNotFoundException("Report Type not found.");
            }
        }
    }
}
