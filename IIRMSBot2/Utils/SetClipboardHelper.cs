using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IIRMSBot2.Utils
{
    class SetClipboardHelper : StaHelper
    {
        readonly string _data;

        public SetClipboardHelper(string data)
        {

            _data = data;
        }

        protected override void Work()
        {
            Clipboard.SetText(_data);
        }
    }
}
