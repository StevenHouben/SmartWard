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
    /// Interaction logic for AddResourceView.xaml
    /// </summary>
    public partial class AddResourceView : Page
    {
        public AddResourceView()
        {
            InitializeComponent();
        }

        private void BoardView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SurfaceListBox listbox = (SurfaceListBox)e.Source;
            String s = listbox.SelectedItems[0].ToString();
            AddResourceViewModel addResourceViewModel = (AddResourceViewModel)listbox.DataContext;

            switch (s)
            {   
                case "EWS":


                    if (!addResourceViewModel.HasEWS())
                    {
                        EWS ews = new EWS(addResourceViewModel.Patient.Id);
                        NavigationService.GetNavigationService(this).Navigate(new EWSView() { DataContext = new EWSViewModel(ews, addResourceViewModel.WardNode) });
                    }
                    else
                    {
                        MessageBox.Show("A EWS resource for " + addResourceViewModel.Patient.Name + " (" + addResourceViewModel.Patient.Cpr + ") already exists");
                    }
                    break;
                case "Note":
                    if (!addResourceViewModel.HasNote()) 
                    {
                        Note note = new Note(addResourceViewModel.Patient.Id, "");
                        NavigationService.GetNavigationService(this).Navigate(new NoteView() { DataContext = new NoteViewModel(note, addResourceViewModel.WardNode) });
                    }
                    else
                    { 
                        MessageBox.Show("A EWS resource for " + addResourceViewModel.Patient.Name + " (" + addResourceViewModel.Patient.Cpr + ") already exists");
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
