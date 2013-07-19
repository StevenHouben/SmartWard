using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.ComponentModel;
using Raven.Abstractions.Extensions;
using SmartWard.Infrastructure;
using SmartWard.Model;
using SmartWard.Whiteboard.ViewModel;

namespace SmartWard.Whiteboard
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        public WardNode WardNode { get; set; }

        private int _roomNumber = 1;

        public ObservableCollection<PatientViewModel> Patients { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            WindowStyle = WindowStyle.SingleBorderWindow;
            WindowState = WindowState.Maximized;

            WardNode = WardNode.StartWardNodeAsSystem(WebConfiguration.DefaultWebConfiguration);

            DataContext = this;

            Patients = new ObservableCollection<PatientViewModel>();
            Patients.CollectionChanged += Patients_CollectionChanged;

            InitializeMapOverlay();

            WardNode.PatientAdded += WardNode_PatientAdded;
            WardNode.PatientRemoved += WardNode_PatientRemoved;

            WardNode.PatientChanged += WardNode_PatientChanged;
            WardNode.Patients.ForEach(p => Patients.Add(new PatientViewModel(p) {RoomNumber = _roomNumber++}));
        }

        void WardNode_PatientAdded(object sender, Patient e)
        {
            Patients.Add(new PatientViewModel(e) { RoomNumber = _roomNumber++ });
        }

        void WardNode_PatientChanged(object sender, Patient e)
        {
 	        foreach (var t in Patients.Where(t => t.Id == e.Id))
            {
                t.UpdateAllProperties(e);
                break;
            }
        }

        void Patients_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var list = e.NewItems;
                foreach (var item in list)
                {
                    var patient = item as PatientViewModel;
                    if (patient == null) return;
                    patient.PatientUpdated += patient_PatientUpdated;
                }
            }
        }

        void WardNode_PatientRemoved(object sender, Patient e)
        {
            foreach (var p in Patients)
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (p.Id == e.Id)
                            Patients.Remove(p);
                    });
                }
        }


        void patient_PatientUpdated(object sender, EventArgs e)
        {
            WardNode.UpdatePatient((Patient)sender);
        }

        private void InitializeMapOverlay()
        {
            var sysRect = Screen.PrimaryScreen.Bounds;
            var rect = new Rect(
                0,
                0,
                sysRect.Width,
                sysRect.Height);
            popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Absolute;
            popup.PlacementRectangle = rect;
            popup.Width = rect.Width;
            popup.Height = rect.Height;
            popup.AllowsTransparency = true;
            popup.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            popup.MouseDown += popup_Down;
            popup.TouchDown += popup_Down;
        }

        void popup_Down(object sender, EventArgs e)
        {
            popup.IsOpen = false;
        }

        private void btnMap_click(object sender, RoutedEventArgs e)
        {
            txtMap.Text = popup != null && (popup.IsOpen = !popup.IsOpen) ? "Close Map" : "Map";
        }

        private void btnLocationTracking_Click_1(object sender, RoutedEventArgs e)
        {
            WardNode.IsLocationEnabled = !WardNode.IsLocationEnabled;
        }

        private void btnDiscovery_Click_1(object sender, RoutedEventArgs e)
        {
            WardNode.IsBroadcastEnabled = !WardNode.IsBroadcastEnabled;
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

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            WardNode.AddPatient(new Patient());
        }
        private void BtnWebApi_OnClick(object sender, RoutedEventArgs e)
        {
            WardNode.IsWebApiEnabled = !WardNode.IsWebApiEnabled;
        }
    }
}
