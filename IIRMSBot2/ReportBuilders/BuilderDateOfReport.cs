using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TikaOnDotNet.TextExtraction;

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
        private readonly Regex _regexSddp;

        public BuilderDateOfReport()
        {
            _regex = new Regex(@"date\s+of\s+report\s*:\s*(\d\d)\s+([a-z]+)\s+(\d+)", RegexOptions.IgnoreCase);
            _regexSddp = new Regex(@"(\d\d)\s+([a-z]+)\s+(\d\d\d\d)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        public void Build(Dictionary<string, string> report, TextExtractionResult textExtractionResult)
        {
            try
            {
                if (report[KnownReportParts.PART_CNR].ToUpper().EndsWith(".SDDP"))
                {
                    var matchSddp = _regexSddp.Match(textExtractionResult.Text);
                    if (!matchSddp.Success)
                    {
                        if (_required)
                            throw new PartNotFoundException("Date of report not found");
                    }

                    var monthStr = matchSddp.Groups[2].Value.Trim().ToLower().Substring(0, 3);
                    var monthNum = _lookup[monthStr];
                    var yearStr = matchSddp.Groups[3].Value.Trim();
                    var day = int.Parse(matchSddp.Groups[1].Value.Trim());
                    if (yearStr.Length == 2)
                    {
                        yearStr = (int.Parse(yearStr) + 2000).ToString();
                    }

                    report.Add(KnownReportParts.PART_DATEOFREPORT_STR, $"{monthNum}-{day:00}-{yearStr}");

                    report.Add(KnownReportParts.PART_DATEOFREPORT_UTC, DateTime.Parse($"{yearStr}-{monthNum}-{day:00}").ToString("0"));
                    return;
                }

                var match = _regex.Match(textExtractionResult.Text);
                if (match.Success)
                {
                    var monthStr = match.Groups[2].Value.Trim().ToLower().Substring(0, 3);
                    var monthNum = _lookup[monthStr];
                    var yearStr = match.Groups[3].Value.Trim();
                    var day = int.Parse(match.Groups[1].Value.Trim());
                    if (yearStr.Length == 2)
                    {
                        yearStr = (int.Parse(yearStr) + 2000).ToString();
                    }

                    report.Add(KnownReportParts.PART_DATEOFREPORT_STR, $"{monthNum}-{day:00}-{yearStr}");
                    report.Add(KnownReportParts.PART_DATEOFREPORT_UTC, DateTime.Parse($"{yearStr}-{monthNum}-{day:00}").ToString("O"));
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
