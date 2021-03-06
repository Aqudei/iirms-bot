﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using TikaOnDotNet.TextExtraction;

namespace IIRMSBot2.ReportBuilders
{
    public class BuilderReportBody : ReportBuilderBase, IReportBuilder
    {
        private readonly Regex _regex = new Regex(@"information\s*:\s*$(.+?)comment/?s?\s*:", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        private readonly Regex _regex2 = new Regex(@"references?\s*:\s*.*?$(.+)comment/?s\s*:", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);

        public void Build(Dictionary<string, string> report, TextExtractionResult rawInputBody)
        {
            if (report[KnownReportParts.PART_CNR].ToUpper().EndsWith(".AMR")
                || report[KnownReportParts.PART_CNR].ToUpper().EndsWith(".AAR")
                || report[KnownReportParts.PART_CNR].ToUpper().EndsWith(".SDDP"))
            {
                report.Add(KnownReportParts.PART_BODY, RemoveInBetweenWhiteSpaces(rawInputBody.Text));
                return;
            }

            var match = _regex.Match(rawInputBody.Text);
            if (match.Success)
            {
                report.Add(KnownReportParts.PART_BODY,
                    RemoveInBetweenWhiteSpaces(match.Groups[1].Value.Trim("\r\n\t ".ToCharArray())));
                return;
            }

            var match2 = _regex2.Match(rawInputBody.Text);
            if (match2.Success)
            {
                report.Add(KnownReportParts.PART_BODY,
                                    RemoveInBetweenWhiteSpaces(match2.Groups[1].Value.Trim("\r\n\t ".ToCharArray())));
                return;
            }


            if (string.IsNullOrWhiteSpace(rawInputBody.Text))
                throw new PartNotFoundException("Report body not found");

            report.Add(KnownReportParts.PART_BODY, Cleanup(rawInputBody.Text));
        }
    }
}
