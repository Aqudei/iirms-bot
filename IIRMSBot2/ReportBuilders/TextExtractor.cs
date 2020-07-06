using TikaOnDotNet.TextExtraction;

namespace IIRMSBot2.ReportBuilders
{
    public class TextExtractor
    {
        private readonly TikaOnDotNet.TextExtraction.TextExtractor _extractor;


        public TextExtractor()
        {
            _extractor = new TikaOnDotNet.TextExtraction.TextExtractor();
        }

        public TextExtractionResult ExtractText(string filename)
        {
            return _extractor.Extract(filename);
        }
    }
}
