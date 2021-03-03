using System.Collections.Generic;
using System.Text.RegularExpressions;
using TikaOnDotNet.TextExtraction;
using System.Linq;

namespace IIRMSBot2.ReportBuilders
{
    public class BuilderEvaluation : ReportBuilderBase, IReportBuilder
    {
        private List<string> DocEval = new List<string> { ".amr", ".aar", ".sddp", ".lsi", ".esr" };
        private readonly Regex _regex;
        private readonly Regex _regex2;

        public BuilderEvaluation()
        {
            _regex = new Regex(@"e?valua?tion\s*:\s*(.+)\s*$",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);

            _regex2 = new Regex(@"([a-f]).*([1-6])",
                RegexOptions.IgnoreCase);
        }

        public void Build(Dictionary<string, string> report, TextExtractionResult rawInputBody)
        {
            if (DocEval.Any(d => report[KnownReportParts.PART_CNR].ToLower().EndsWith(d)))
            {
                report.Add(KnownReportParts.PART_EVALUATION, "DOC");
                return;
            }

            var match = _regex.Match(rawInputBody.Text);
            if (match.Success)
            {
                if (match.Groups[1].Value.ToUpper().Contains("DOC"))
                {
                    report.Add(KnownReportParts.PART_EVALUATION, "DOC");
                    return;
                }
                else
                {
                    var match2 = _regex2.Match(match.Groups[1].Value.ToUpper());
                    if (match2.Success)
                    {
                        report.Add(KnownReportParts.PART_EVALUATION, match2.Groups[1].Value + match2.Groups[2].Value);
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
