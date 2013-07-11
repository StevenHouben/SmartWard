using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using SmartWard.Model;
using System.ComponentModel;

namespace SmartWard.HyPR.Controls
{
    /// <summary>
    /// Interaction logic for GridHeader.xaml
    /// </summary>
    public partial class BoardLayout : UserControl,INotifyPropertyChanged
    {
        public ObservableCollection<Patient> Patients { get; set; }


        private int _fontSize;

        public int DefaultFontSize
        { 
            get{return _fontSize;}
            set
            {
                _fontSize = value;
                OnPropertyChanged("_fontSize");
            }
        }
        public BoardLayout()
        {
            InitializeComponent();

            DataContext = this;

            Patients = new ObservableCollection<Patient>();

            DefaultFontSize = 35;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
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
                    break;
                }
            }
        }

    }
}
