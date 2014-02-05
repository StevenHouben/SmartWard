using SmartWard.PDA.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartWard.PDA.Views
{
    /// <summary>
    /// Interaction logic for PDAWindow.xaml
    /// </summary>
    public partial class PDAWindow : Window
    {
        public PDAWindow()
        {
            InitializeComponent();            
        }

        public void InitializeFrame()
        {
            LoginView loginView = new LoginView();
            loginView.DataContext = new AuthenticatedViewModel(((WindowViewModel)this.DataContext).WardNode);

            ContentFrame.NavigationService.Navigate(loginView);
        }

        private void click_Close(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void PatientsButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.NavigationService.Navigate(new Patients() { DataContext = new PatientsViewModel(new List<String>(), ((WindowViewModel)this.DataContext).WardNode) });
        }
    }
}
