using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IIRMSBot2.Model
{
    class EncodePayload
    {
        public string DocumentId { get; set; }

        public string FullText { get; set; }
        public string InfoAccId { get; set; } = "37"; // SECRET
        public string OriginId { get; set; } = "D018"; // NICA-RO-08
        public string ReportDate { get; set; }
        public string ReportNo { get; set; }
        public string ReportType { get; set; } = "D018-R024"; // AAR
        public string SecClassId { get; set; }
        public string SourceId { get; set; } = "D018"; // NICA-RO-08
        public string SubjectTitle { get; set; }
        public string WrittenBy { get; set; } = "Aqudei";
    }
}
