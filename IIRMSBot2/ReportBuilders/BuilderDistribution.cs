using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IIRMSBot2.ReportBuilders
{
    public class BuilderDistribution : ReportBuilderBase, IReportBuilder
    {
        private readonly Regex _regex;

        public BuilderDistribution()
        {
            _regex = new Regex(@"distribution\s*:\s*(.+)$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }

        private readonly Regex _redundantColon = new Regex("[;:]+");

        public void Build(Dictionary<string, string> report, string rawInputBody)
        {
            var rslt = _regex.Match(rawInputBody);
            if (rslt.Success)
            {
                var tmp = rslt.Groups[1].Value.ToUpper().Trim("\r\n ".ToCharArray());
                var dists = tmp.Split(@"/\; ".ToCharArray());
                var distribution = dists.Aggregate((left, right) =>
                {
                    return (left + ";" + right.ToUpper().Trim()).Replace("DIII", "D3").Replace("DII", "D2").Replace("DV", "D5");
                });

                distribution = distribution.Substring(0, distribution.IndexOf("FILE")).Trim("; ".ToCharArray());
                distribution = _redundantColon.Replace(distribution, ";");
                report.Add(KnownReportParts.PART_DISTRIBUTION, RemoveSpaces(distribution));
            }
            else
            {
                if (_required)
                    throw new PartNotFoundException("Distribution was not found.");
            }
        }
    }
}
