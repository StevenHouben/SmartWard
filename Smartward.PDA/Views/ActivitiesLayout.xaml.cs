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

        public void Navigate()
        {
            Window parentWindow = Window.GetWindow(this);

            Patients patientsWindow = new Patients();
            patientsWindow.Show();

            parentWindow.Close();
        }

        private void BoardView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Navigate();
        }
    }
}
