using Microsoft.Surface.Presentation.Controls;
using SmartWard.Models;
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
    /// Interaction logic for PatientsLayout.xaml
    /// </summary>
    public partial class PatientsLayout : UserControl
    {
        private SurfaceListBoxItem selected;

        public PatientsLayout()
        {
            InitializeComponent();
        }

        public void NavigateToPatientView(PatientsLayoutViewModel plViewModel)
        {
            NavigationService.GetNavigationService(this).Navigate(new PatientView() { DataContext = plViewModel });
        }

        private void Patient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: Show the patient journal in view!
        }

        public void PreviewMouseUpHandler(object sender, MouseEventArgs e)
        {
            selectHandler(sender as SurfaceListBoxItem);
        }

        public void PreviewTouchUpHandler(object sender, TouchEventArgs e)
        {
            selectHandler(sender as SurfaceListBoxItem);
        }
        /// <summary>
        /// Checks if item already has been selected. If so, navigates to PatientView.
        /// </summary>
        /// <param name="item"></param>
        public void selectHandler(SurfaceListBoxItem item)  
        {
            if (selected != null && selected.Equals(item))
            {
                NavigateToPatientView((PatientsLayoutViewModel)item.DataContext);
            }
            else
            {
                selected = item;
            }
        }
    }
}
    