﻿using System.Collections.Generic;

namespace IIRMSBot2.ReportBuilders
{
    public interface IReportBuilder
    {
        void Build(Dictionary<string, string> report, TikaOnDotNet.TextExtraction.TextExtractionResult textExtractionResult);
    }
}
