using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IIRMSBot2.ReportBuilders
{
    public class BuilderReportBody : ReportBuilderBase, IReportBuilder
    {
        private readonly Regex _regex = new Regex(@"information\s*:\s*$(.+?)comment/?s?\s*:", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        private readonly Regex _regex2 = new Regex(@"references?\s*:\s*.*?$(.+)comment/?s\s*:", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);

        public void Build(Dictionary<string, string> report, string rawInputBody)
        {
            var rslt = _regex.Match(rawInputBody);
            if (rslt.Success)
            {
                report.Add(KnownReportParts.PART_BODY,
                    RemoveInBetweenWhiteSpaces(rslt.Groups[1].Value.Trim("\r\n\t ".ToCharArray())));
                return;
            }

            var rslt2 = _regex2.Match(rawInputBody);
            if (rslt2.Success)
            {
                report.Add(KnownReportParts.PART_BODY,
                                    RemoveInBetweenWhiteSpaces(rslt2.Groups[1].Value.Trim("\r\n\t ".ToCharArray())));
                return;
            }
            
            throw new PartNotFoundException("Report body not found");
        }
    }
}
