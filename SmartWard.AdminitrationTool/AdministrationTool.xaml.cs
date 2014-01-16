using SmartWard.AdministrationTool.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SmartWard.AdministrationTool
{
    /// <summary>
    /// Interaction logic for AdministrationTool.xaml
    /// </summary>
    public partial class AdministrationTool : Window
    {
        public AdministrationTool()
        {
            InitializeComponent();

            ContentFrame.NavigationService.Navigate(new MainMenu());
        }

        private void click_Close(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
