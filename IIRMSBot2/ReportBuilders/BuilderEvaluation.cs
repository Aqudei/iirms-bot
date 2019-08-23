using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IIRMSBot2.ReportBuilders
{
    public class BuilderEvaluation : ReportBuilderBase, IReportBuilder
    {

        private Regex _regex;
        private Regex _regex2;

        public BuilderEvaluation()
        {
            _regex = new Regex(@"e?valua?tion\s*:\s*(.+)\s*$",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);

            _regex2 = new Regex(@"([a-f]).*([1-6])",
                RegexOptions.IgnoreCase);
        }

        public void Build(Dictionary<string, string> report, string rawInputBody)
        {
            var rslt = _regex.Match(rawInputBody);
            if (rslt.Success)
            {
                if (rslt.Groups[1].Value.ToUpper().Contains("DOC"))
                {
                    report.Add(KnownReportParts.PART_EVALUATION, "DOC");
                    return;
                }
                else
                {
                    var rslt2 = _regex2.Match(rslt.Groups[1].Value.ToUpper());
                    if (rslt2.Success)
                    {
                        report.Add(KnownReportParts.PART_EVALUATION, rslt2.Groups[1].Value + rslt2.Groups[2].Value);
                        return;
                    }
                }
            }

            if (_required)
            {
                throw new PartNotFoundException("Evaluation was not found");
            }
        }
    }
}
