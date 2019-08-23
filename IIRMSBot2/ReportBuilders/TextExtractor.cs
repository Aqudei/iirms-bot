namespace IIRMSBot2.ReportBuilders
{
    public class TextExtractor
    {
        private readonly TikaOnDotNet.TextExtraction.TextExtractor _extractor;


        public TextExtractor()
        {
            _extractor = new TikaOnDotNet.TextExtraction.TextExtractor();
        }

        public string ExtractText(string filename)
        {
            var extracted = _extractor.Extract(filename);
            return extracted.Text;
        }
    }
}
