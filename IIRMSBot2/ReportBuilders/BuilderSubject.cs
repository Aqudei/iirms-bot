using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TikaOnDotNet.TextExtraction;

namespace IIRMSBot2.ReportBuilders
{
    public class BuilderSubject : ReportBuilderBase, IReportBuilder
    {
        private readonly Regex _regex;
        private readonly Regex _regex1;
        private readonly Regex _regex3;

        private readonly List<string> OPNLS = new List<string> { ".LR", ".CR", ".FORE", ".CVR" };

        public BuilderSubject()
        {
            _regex = new Regex(@"(summary\s+of\s+info?rmation|information\s+report|reply\s+to\s+eei).+?su[bj]+ect\s*:\s*(.+?)date\s+of\s+report", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            _regex1 = new Regex(@"subject\s*:\s*(.+?\.)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            _regex3 = new Regex(@"\s+\d+\.", RegexOptions.Singleline);
        }

        public void Build(Dictionary<string, string> report, TextExtractionResult rawInputBody)
        {
            Match match = null;

            if (OPNLS.Any(o => report[KnownReportParts.PART_CNR].ToUpper().EndsWith(o)))
            {
                report.Add(KnownReportParts.PART_SUBJECT, $"Operational Report '{report[KnownReportParts.PART_CNR]}'");
            }
            else if (report[KnownReportParts.PART_CNR].ToUpper().EndsWith(".ESR"))
            {
                match = _regex1.Match(rawInputBody.Text);
                if (match.Success)
                {
                    report.Add(KnownReportParts.PART_SUBJECT, _regex3.Replace(Cleanup(match.Groups[1].Value), ""));
                }
            }

            else if (report[KnownReportParts.REPORTTYPE].ToUpper() == "AFTER MEETING REPORT"
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
            }
            else
            {
                match = _regex.Match(rawInputBody.Text);
                if (match.Success)
                {
                    report.Add(KnownReportParts.PART_SUBJECT,
                        RemoveInBetweenWhiteSpaces(match.Groups[2].Value.Trim("\r\n\t ".ToCharArray()).ToUpper()));
                }
            }


            if (!report.ContainsKey(KnownReportParts.PART_SUBJECT) && _required)
            {
                throw new PartNotFoundException("Subject was not found.");
            }
        }
    }
}
