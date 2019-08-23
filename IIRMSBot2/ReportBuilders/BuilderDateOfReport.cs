using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IIRMSBot2.ReportBuilders
{
    public class BuilderDateOfReport : ReportBuilderBase, IReportBuilder
    {
        private readonly Dictionary<string, string> _lookup = new Dictionary<string, string>
        {
            { "jan","01"},
            { "feb","02"},
            { "mar","03"},
            { "apr","04"},
            { "may","05"},
            { "jun","06"},
            { "jul","07"},
            { "aug","08"},
            { "sep","09"},
            { "oct","10"},
            { "nov","11"},
            { "dec","12"},
        };

        private readonly Regex _regex;

        public BuilderDateOfReport()
        {
            _regex = new Regex(@"date\s+of\s+report\s*:\s*(\d\d)\s+([a-z]+)\s+(\d+)", RegexOptions.IgnoreCase);
        }

        public void Build(Dictionary<string, string> report, string rawInputBody)
        {
            try
            {
                var rslt = _regex.Match(rawInputBody);
                if (rslt.Success)
                {
                    var monthStr = rslt.Groups[2].Value.Trim().ToLower().Substring(0, 3);
                    var monthNum = _lookup[monthStr];
                    var yearStr = rslt.Groups[3].Value.Trim();
                    var day = int.Parse(rslt.Groups[1].Value.Trim());
                    if (yearStr.Length == 2)
                    {
                        yearStr = (int.Parse(yearStr) + 2000).ToString();
                    }

                    report.Add(KnownReportParts.PART_DATEOFREPORT, $"{monthNum}{day:00}{yearStr}");
                }
                else
                {
                    if (_required)
                        throw new PartNotFoundException("Date of report not found");
                }
            }
            catch (Exception)
            {
                throw new PartNotFoundException("Date of report not found");
            }

        }
    }
}
