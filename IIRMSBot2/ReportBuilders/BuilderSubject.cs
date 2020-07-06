using System.Collections.Generic;
using System.Text.RegularExpressions;
using TikaOnDotNet.TextExtraction;

namespace IIRMSBot2.ReportBuilders
{
    public class BuilderSubject : ReportBuilderBase, IReportBuilder
    {
        private readonly Regex _regex;

        public BuilderSubject()
        {
            _regex = new Regex(@"(summary\s+of\s+info?rmation|information\s+report|reply\s+to\s+eei).+?su[bj]+ect\s*:\s*(.+?)date\s+of\s+report", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        public void Build(Dictionary<string, string> report, TextExtractionResult rawInputBody)
        {
            if (report[KnownReportParts.REPORTTYPE].ToUpper() == "AFTER MEETING REPORT"
                || report[KnownReportParts.REPORTTYPE].ToUpper() == "AFTER ACTIVITY REPORT"
                || report[KnownReportParts.PART_CNR].ToUpper().EndsWith(".SDDP"))
            {
                if (rawInputBody.Metadata.ContainsKey("subject") && !string.IsNullOrWhiteSpace(rawInputBody.Metadata["subject"]))
                {
                    report.Add(KnownReportParts.PART_SUBJECT, RemoveRedundantSpaces(rawInputBody.Metadata["subject"].ToUpper()));
                }
                else
                {
                    report.Add(KnownReportParts.PART_SUBJECT, RemoveRedundantSpaces(report[KnownReportParts.REPORTTYPE].ToUpper()));
                }
                return;
            }


            var match = _regex.Match(rawInputBody.Text);
            if (match.Success)
            {
                report.Add(KnownReportParts.PART_SUBJECT,
                    RemoveInBetweenWhiteSpaces(match.Groups[2].Value.Trim("\r\n\t ".ToCharArray()).ToUpper()));
            }
            else
            {
                if (_required)
                    throw new PartNotFoundException(
                        $"'{KnownReportParts.PART_SUBJECT}' not found.");
            }
        }
    }
}
