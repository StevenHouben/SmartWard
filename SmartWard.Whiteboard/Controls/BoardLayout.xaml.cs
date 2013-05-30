using SmartWard.Whiteboard.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace SmartWard.Whiteboard
{
    /// <summary>
    /// Interaction logic for GridHeader.xaml
    /// </summary>
    public partial class BoardLayout : UserControl
    {
        public ObservableCollection<Patient> Patients { get; set; }
        public BoardLayout()
        {
            InitializeComponent();

            this.DataContext = this;

            Patients = new ObservableCollection<Patient>();
        }


        private void SurfaceButton_Click_1(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var dp = (DockPanel)btn.Parent;
            var br = (Border)dp.Parent;
            var gr = (Grid)br.Parent;

            for (int i = 0; i < Patients.Count; i++)
            {
                if (Patients[i].User.Id == ((Patient)gr.DataContext).User.Id)
                {
                    Patients[i].Status++;
                    break;
                }
            }
        }

    }
}
