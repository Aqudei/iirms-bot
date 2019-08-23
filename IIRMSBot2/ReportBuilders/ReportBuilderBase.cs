using System.Text.RegularExpressions;

namespace IIRMSBot2.ReportBuilders
{
    public abstract class ReportBuilderBase
    {
        protected readonly bool _required;

        protected ReportBuilderBase(bool required = true)
        {
            _required = required;
        }

        private readonly Regex _regexWhiteSpaceRemover = new Regex(@"\s+");

        protected string RemoveSpaces(string input)
        {
            return _regexWhiteSpaceRemover.Replace(input, "");
        }

        protected string RemoveRedundantSpaces(string input)
        {
            return _regexWhiteSpaceRemover.Replace(input, " ");
        }

        protected string RemoveTrails(string input)
        {
            return input.Trim("\r\n\t ".ToCharArray());
        }
        
        protected string RemoveInBetweenWhiteSpaces(string input)
        {
            var toSpaces = input.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ');
            return RemoveRedundantSpaces(toSpaces);
        }
    }
}
