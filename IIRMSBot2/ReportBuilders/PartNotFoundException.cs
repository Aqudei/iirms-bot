using System;

namespace IIRMSBot2.ReportBuilders
{
    public class PartNotFoundException : Exception
    {
        public PartNotFoundException(string message) : base(message)
        {
        }
    }
}
