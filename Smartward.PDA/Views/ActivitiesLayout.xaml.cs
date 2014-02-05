using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Navigation;
using SmartWard.PDA.ViewModels;
using SmartWard.Models;

namespace SmartWard.PDA.Views
{
    /// <summary>
    /// Interaction logic for ActivitiesLayout.xaml
    /// </summary>
    public partial class ActivitiesLayout
    {
        public ActivitiesLayout()
        {
            InitializeComponent();
        }

        public void NavigateToPatientsView(ActivityViewModel vm)
        {
            NavigationService.GetNavigationService(this).Navigate(new Patients() { DataContext = new PatientsViewModel(((RoundActivity)vm.Activity).GetPatientIds(), vm.WardNode) });
        }

        private void BoardView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((SurfaceListBox)e.Source).SelectedItems.Count > 0)
            {
                ActivityViewModel aViewModel = (ActivityViewModel)((SurfaceListBox)e.Source).SelectedItems[0];
                if (aViewModel.Activity.Type.Equals(typeof(RoundActivity).Name))
                {
                    NavigateToPatientsView(aViewModel);
                }
            }
        }
    }
}
