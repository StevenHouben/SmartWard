using NooSphere.Infrastructure.Discovery;
using NooSphere.Infrastructure.Helpers;
using SmartWard.Infrastructure;
using SmartWard.Models.Devices;
using SmartWard.PDA.ViewModels;
using SmartWard.PDA.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Smartward.PDA
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var window = new PDAWindow();

            /*DiscoveryManager disco = new DiscoveryManager();
            
            disco.DiscoveryAddressAdded += (sender, discoveryEvent) =>
            {
                if (discoveryEvent.ServiceInfo.Code == "1337")
                { */
                    WebConfiguration foundWebConfiguration = new WebConfiguration("10.25.209.221", 8070);

                    WardNode wardNode = WardNode.StartWardNodeAsClient(foundWebConfiguration);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        window.DataContext = new WindowViewModel(wardNode);
                        window.InitializeFrame();
                        window.Show();
                    }
                    );

            window.Closing += (s, ev) => wardNode.RemoveClientDevice();
                /*}
            };

            disco.Find(DiscoveryType.Zeroconf);*/

            
            //var viewModel = new ActivitiesViewModel();
            // When the ViewModel asks to be closed, 

            // close the window.
            //EventHandler handler = null;
            //handler = delegate
            //{
            //    viewModel.RequestClose -= handler;
            //    window.Close();
            //};
            //viewModel.RequestClose += handler;

            // Allow all controls in the window to 
            // bind to the ViewModel by setting the 
            // DataContext, which propagates down 
            // the element tree.
            //window.DataContext = viewModel;

           
        }
    }
}
