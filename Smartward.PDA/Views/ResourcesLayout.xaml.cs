using Microsoft.Surface.Presentation.Controls;
using SmartWard.Models;
using SmartWard.PDA.ViewModels;
using SmartWard.ViewModels;
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
    /// Interaction logic for ResourcesLayout.xaml
    /// </summary>
    public partial class ResourcesLayout : UserControl
    {
        public ResourcesLayout()
        {
            InitializeComponent();
        }

        public void NavigateToResourceView(ResourceViewModelBase resourceViewModel)
        {
            switch (resourceViewModel.Resource.Type)
            {
                case "EWS":
                    NavigationService.GetNavigationService(this).Navigate(new EWSView() { DataContext = (UpdatableEWSViewModel) resourceViewModel });
                    break;
                case "Note":
                    NavigationService.GetNavigationService(this).Navigate(new NoteView() { DataContext = (UpdatableNoteViewModel) resourceViewModel });
                    break;
                default:
                    break;
            }
            
        }

        private void BoardView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SurfaceListBox src = (SurfaceListBox)e.Source;
            if (src.SelectedItems.Count > 0) 
            { 
                ResourceViewModelBase resourceViewModel = (ResourceViewModelBase)src.SelectedItems[0];
                NavigateToResourceView(resourceViewModel);
            } 
        }
    }
}
