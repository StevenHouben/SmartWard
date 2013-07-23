using System;
using System.Windows;
using SmartWard.Whiteboard.ViewModels;
using SmartWard.Whiteboard.Views;

namespace SmartWard.Whiteboard
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var window = new Board();

            var viewModel = new BoardViewModel();
            // When the ViewModel asks to be closed, 

            // close the window.
            EventHandler handler = null;
            handler = delegate
            {
                viewModel.RequestClose -= handler;
                window.Close();
            };
            viewModel.RequestClose += handler;

            // Allow all controls in the window to 
            // bind to the ViewModel by setting the 
            // DataContext, which propagates down 
            // the element tree.
            window.DataContext = viewModel;

            window.Show();
        }
    }
}
