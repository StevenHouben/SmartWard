using Microsoft.Surface.Presentation.Controls;
using SmartWard.AdministrationTool.ViewModels;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SmartWard.AdministrationTool.Views
{
    /// <summary>
    /// Interaction logic for CliniciansLayout.xaml
    /// </summary>
    public partial class CliniciansLayout : UserControl
    {
        public CliniciansLayout()
        {
            InitializeComponent();
        }

        private void NavigateToSelection(object sender, SelectionChangedEventArgs e)
        {
            NavigationService.GetNavigationService(this).Navigate(new ClinicianPage() { DataContext = (UpdatableClinicianViewModel)((SurfaceListBox)e.Source).SelectedItem });
        }
    }
}
