using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using SmartWard.Model;
using System;

namespace SmartWard.Whiteboard.Controls
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

            DataContext = this;

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
                if (Patients[i].Id == ((Patient)gr.DataContext).Id)
                {
                    Patients[i].Status++;
                    if (PatientUpdated != null)
                        PatientUpdated(this, Patients[i]);
                    break;
                }
            }

        }

        public event EventHandler<Patient> PatientUpdated;

    }
}
