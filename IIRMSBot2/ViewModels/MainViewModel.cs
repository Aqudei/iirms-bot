using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace IIRMSBot2.ViewModels
{
    sealed class MainViewModel : Conductor<object>.Collection.OneActive
    {
        public MainViewModel()
        {
            ActivateItem(IoC.Get<SettingsViewModel>());
            Items.Add(IoC.Get<EncoderViewModel>());
        }
    }
}
