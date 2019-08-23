using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IIRMSBot2.ReportBuilders
{
    public class BuilderSubject : ReportBuilderBase, IReportBuilder
    {
        private readonly Regex _regex;

        public BuilderSubject()
        {
            _regex = new Regex(@"(summary\s+of\s+info?rmation|information\s+report|reply\s+to\s+eei).+?su[bj]+ect\s*:\s*(.+?)date\s+of\s+report", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        public void Build(Dictionary<string, string> report, string rawInputBody)
        {
            var rslt = _regex.Match(rawInputBody);
            if (rslt.Success)
            {
                report.Add(KnownReportParts.PART_SUBJECT,
                    RemoveInBetweenWhiteSpaces(rslt.Groups[2].Value.Trim("\r\n\t ".ToCharArray()).ToUpper()));
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
