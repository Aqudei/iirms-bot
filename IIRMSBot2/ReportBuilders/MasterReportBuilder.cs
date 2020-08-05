using System.Collections.Generic;

namespace IIRMSBot2.ReportBuilders
{
    public class MasterReportBuilder
    {
        private readonly Dictionary<string, IReportBuilder> _reportBuilder = new Dictionary<string, IReportBuilder>();
        private readonly TextExtractor _textExtractor;

        public MasterReportBuilder()
        {
            _reportBuilder.Add(KnownReportParts.PART_CNR, new BuilderCNR());
            _reportBuilder.Add(KnownReportParts.PART_DATEOFREPORT_STR, new BuilderDateOfReport());
            _reportBuilder.Add(KnownReportParts.PART_EVALUATION, new BuilderEvaluation());
            _reportBuilder.Add(KnownReportParts.PART_BODY, new BuilderReportBody());
            _reportBuilder.Add(KnownReportParts.REPORTTYPE, new BuilderReportType());
            _reportBuilder.Add(KnownReportParts.PART_SUBJECT, new BuilderSubject());
            //_reportBuilder.Add(new BuilderDistribution());
           

            _textExtractor = new TextExtractor();
        }

        public void AddBuilderForKnownReportPart(string partName, IReportBuilder builder)
        {
            if (_reportBuilder.ContainsKey(partName))
                return;

            _reportBuilder.Add(partName, builder);
        }

        public void RemoveBuilderForPart(string partName)
        {
            if (_reportBuilder.ContainsKey(partName))
                _reportBuilder.Remove(partName);
        }

        public Dictionary<string, string> Build(string filename)
        {
            var content = _textExtractor.ExtractText(filename);
            var report = new Dictionary<string, string> { { KnownReportParts.PART_FILENAME, filename } };

            foreach (var builder in _reportBuilder.Values)
            {
                builder.Build(report, content);
            }

            return report;
        }
    }
}
