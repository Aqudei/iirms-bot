using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IIRMSBot2.ReportBuilders
{
    public class BuilderCNR : ReportBuilderBase, IReportBuilder
    {

        private readonly Regex _regex;

        public BuilderCNR()
        {
            _regex = new Regex(@"\b[a-z0-9\-]+?\d\d[a-l]\d+\.[a-z]+", RegexOptions.IgnoreCase);
        }

        public void Build(Dictionary<string, string> report, string rawInputBody)
        {
            if (report.ContainsKey(KnownReportParts.PART_FILENAME) == false)
                throw new PartNotFoundException("A filename is needed in order to parse the CNR of report.");

            var rslt = _regex.Match(report[KnownReportParts.PART_FILENAME]);
            if (rslt.Success)
            {
                report[KnownReportParts.PART_CNR] = rslt.Groups[0].Value.Trim().ToUpper();
            }
            else
            {
                if (_required)
                    throw new PartNotFoundException("CNR was not found.");
            }
        }
    }
}
