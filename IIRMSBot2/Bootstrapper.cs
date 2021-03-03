using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using IIRMSBot2.ViewModels;
using ikvm.extensions;
using MahApps.Metro.Controls.Dialogs;
using Unity;

namespace IIRMSBot2
{
    class Bootstrapper : BootstrapperBase
    {
        private readonly IUnityContainer _container = new UnityContainer();

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void Configure()
        {
            _container.RegisterType<IWindowManager, WindowManager>();
            _container.RegisterSingleton<IEventAggregator, EventAggregator>();
            _container.RegisterInstance(DialogCoordinator.Instance);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.ResolveAll(service);
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.Resolve(service, key);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainViewModel>();
        }
    }
}
