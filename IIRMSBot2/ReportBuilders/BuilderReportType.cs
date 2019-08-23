using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IIRMSBot2.ReportBuilders
{
    public class BuilderReportType : ReportBuilderBase, IReportBuilder
    {
        private Regex _regex;
        private readonly Dictionary<string, string> _reportTypes = new Dictionary<string, string>
        {
            {"CI" ,"SUMMARY OF INFORMATION"},
            {"EEI" ,"REPLY TO EEI"},
            {"IR" ,"INFORMATION REPORT"},
            {"SI" ,"SUMMARY OF INFORMATION"},
            {"SOI" ,"SUMMARY OF INFORMATION"},
        };
        public BuilderReportType()
        {
            _regex = new Regex(@"\.([a-z]+)$", RegexOptions.IgnoreCase);
        }

        public void Build(Dictionary<string, string> report, string rawInputBody)
        {
            if (report.ContainsKey(KnownReportParts.PART_CNR) == false)
                throw new PartNotFoundException("CNR must exist to determine report type");

            var cnr = report[KnownReportParts.PART_CNR];
            var rslt = _regex.Match(cnr);

            if (rslt.Success)
            {

                var key = rslt.Groups[1].Value.Trim().ToUpper();
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
