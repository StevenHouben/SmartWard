using ABC.Infrastructure.ActivityBase;
using ABC.Infrastructure.Drivers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ABC.Model.Primitives;
using ABC.Model.Users;
using SmartWard.Infrastructure;
using SmartWard.Model;
using Microsoft.Surface.Presentation.Controls;

namespace SmartWard.HyPR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private HyPrDevice _hyPrDevice;
        public ObservableCollection<IUser> Users { get; set; }

        public WardNode WardNode { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            InitializeWindow();
            InitializeUiComponents();
            InitializeDevice();


            //var config = new WebConfiguration {Address = "10.6.6.148", Port = 8080};

            WardNode = WardNode.StartWardNodeAsSystem(WebConfiguration.LocalWebConfiguration);
            WardNode.UserAdded += node_UserAdded;;
            WardNode.UserChanged += node_UserUpdated;
            WardNode.UserRemoved += node_UserRemoved;
            Users = new ObservableCollection<IUser>(WardNode.Users.Values.ToList());

            whiteboard.Patients = WardNode.Patients;
        }

        private void node_UserUpdated(object sender, UserEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                for (var i = 0; i < Users.Count; i++)
                {
                    if (Users[i].Id == e.User.Id)
                    {
                        Users[i] = e.User;
                        break;
                    }
                }
            }));
        }

        private void node_UserRemoved(object sender, UserRemovedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                for (var i = 0; i < Users.Count; i++)
                {
                    if (Users[i].Id == e.Id)
                    {
                        Users.RemoveAt(i);
                        break;
                    }
                }

            }));
        }

        private void node_UserAdded(object sender, UserEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => Users.Add(e.User)));
        }

        private void InitializeDevice()
        {
            _hyPrDevice = new HyPrDevice();
            _hyPrDevice.RfidDataReceived += hyPRDevice_RFIDDataReceived;
            _hyPrDevice.RfidResetReceived += hyPRDevice_RFIDResetReceived;
        }

        void hyPRDevice_RFIDResetReceived(object sender, EventArgs e)
        {
            //no need for this now
        }

        private void InitializeUiComponents()
        {
            sliderRed.ValueChanged += sliders_ValueChanged;
            sliderBlue.ValueChanged += sliders_ValueChanged;
            sliderGreen.ValueChanged += sliders_ValueChanged;
        }

        private void InitializeWindow()
        {
            KeyDown += MainWindow_KeyDown;
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.SingleBorderWindow;
        }

        private void sliders_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var rgb = new Rgb(Convert.ToByte(sliderRed.Value), Convert.ToByte(sliderGreen.Value), Convert.ToByte(sliderBlue.Value));
            UpdateRectangle(rgb);
            SendColorToHyPrDevice(rgb);

        }
        void UpdateSliders(Rgb color)
        {
            sliderRed.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    sliderRed.Value = color.Red;
                }));
            sliderBlue.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    sliderBlue.Value = color.Blue;
                }));
            sliderGreen.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    sliderGreen.Value = color.Green;
                }));
        }

        private void UpdateRectangle(Rgb color)
        {
            rect.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                rect.Fill = new SolidColorBrush(Color.FromArgb(255, color.Red, color.Green, color.Blue));

            }));
        }
        private void SendColorToHyPrDevice(Rgb color)
        {
                _hyPrDevice.UpdateColor(color);
        }
        void UpdateName(string name,string tag)
        {
            txtName.Dispatcher.Invoke(DispatcherPriority.Normal, new System.Action(() =>
            {
                txtName.Text = name;
                txtTag.Text = tag;
            }));
        }
        void UpdateUi(string name, Rgb color, string tag)
        {
            UpdateName(name,tag);
            UpdateSliders(color);
            UpdateRectangle(color);
        }
        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Environment.Exit(0);
        }
        void hyPRDevice_RFIDDataReceived(object sender, RfdiDataReceivedEventArgs e)
        {
            txtRFID.Dispatcher.Invoke(DispatcherPriority.Normal, new System.Action(() =>
            {
                txtRFID.Content = e.Rfid;
                btnSave.IsEnabled = true;
            }));
            var user = FindUserByCid(e.Rfid);
            if (user != null)
            {
                UpdateUi(user.Name, user.Color,user.Tag);
                SendColorToHyPrDevice(user.Color);
            }
            else
            {
                UpdateUi("", new Rgb(0, 0, 0), "");
                SendColorToHyPrDevice(new Rgb(0, 0, 0));
            }
        }

        private IUser FindUserByCid(string rfid)
        {
            return (Patient)Users.FirstOrDefault(usr => usr.Cid == rfid);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var user = (Patient)FindUserByCid(_hyPrDevice.CurrentRfid);
            if (user != null)
            {
                user.Name = txtName.Text;
                user.Color = new Rgb(Convert.ToByte(sliderRed.Value), Convert.ToByte(sliderGreen.Value),
                                     Convert.ToByte(sliderBlue.Value));
                user.Cid = _hyPrDevice.CurrentRfid;
                user.Tag = txtTag.Text;
                WardNode.UpdatePatient(user);
            }
            else
            {
                WardNode.AddPatient(
                    new Patient
                    {
                        Name = txtName.Text,
                        Color = new Rgb(Convert.ToByte(sliderRed.Value), Convert.ToByte(sliderGreen.Value), Convert.ToByte(sliderBlue.Value)),
                        Cid = _hyPrDevice.CurrentRfid,
                        Tag = txtTag.Text
                    });
            }
        }

        private void txtName_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            System.Diagnostics.Process.Start(@"C:\Program Files\Common Files\Microsoft Shared\ink\TabTip.exe");
            txtName.Focus();
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void menu_click(object sender, RoutedEventArgs e)
        {
            string name = ((SurfaceButton)sender).Name;
            if(name == "btnRegister")
            {
                Register.Visibility = Visibility.Visible;
                Records.Visibility = Overview.Visibility = Visibility.Hidden;
            }
            else if (name == "btnRecords")
            {
                Records.Visibility = Visibility.Visible;
                Register.Visibility = Overview.Visibility = Visibility.Hidden;
            }
            else if (name == "btnOverview")
            {
                Overview.Visibility = Visibility.Visible;
                Register.Visibility = Records.Visibility = Visibility.Hidden;
            }

        }
    }
}
