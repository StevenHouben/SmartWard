using ABC.Infrastructure.ActivityBase;
using ABC.Infrastructure.Drivers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ABC.Model.Primitives;
using ABC.Model.Users;
using SmartWard.Infrastructure;
using SmartWard.Model;

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
            InitializeUIComponents();
            InitializeDevice();

            grid.TouchDown += el_PreviewTouchDown;
            grid.MouseDown += el_MouseDown;

            var config = new WebConfiguration {Address = "10.6.6.148", Port = 8080};

            WardNode = WardNode.StartWardNodeAsSystem(config);
            WardNode.UserAdded += node_UserAdded;
            WardNode.UserChanged += node_UserUpdated;
            WardNode.UserRemoved += node_UserRemoved;
            Users = new ObservableCollection<IUser>(WardNode.Users.Values.ToList());
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
            Dispatcher.BeginInvoke(new System.Action(() => Users.Add(e.User)));
        }


        void el_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            TryKillKeyboard();
        }

  
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        const UInt32 WM_CHAR = 0x0102;
        const int VK_Q = 0x51;

        private static void TryKillKeyboard()
        {
            try
            {
                //SendMessage(System.Diagnostics.Process.GetProcessesByName("TabTip")[0].Handle, WM_CHAR, new IntPtr(VK_Q), new IntPtr(1));
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        void el_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TryKillKeyboard();
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

        private void InitializeUIComponents()
        {
            this.sliderRed.ValueChanged += sliders_ValueChanged;
            this.sliderBlue.ValueChanged += sliders_ValueChanged;
            this.sliderGreen.ValueChanged += sliders_ValueChanged;
        }

        private void InitializeWindow()
        {
            this.KeyDown += MainWindow_KeyDown;
            this.WindowState = System.Windows.WindowState.Maximized;
            this.WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
        }

        private void sliders_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var rgb = new Rgb(Convert.ToByte(sliderRed.Value), Convert.ToByte(sliderGreen.Value), Convert.ToByte(sliderBlue.Value));
            UpdateRectangle(rgb);
            SendColorToHyPRDevice(rgb);

        }
        void UpdateSliders(Rgb color, bool upDateSliders = false)
        {
            if (upDateSliders)
            {
                sliderRed.Dispatcher.Invoke(DispatcherPriority.Normal, new System.Action(() =>
                {
                    sliderRed.Value = color.Red;
                }));
                sliderBlue.Dispatcher.Invoke(DispatcherPriority.Normal, new System.Action(() =>
                {
                    sliderBlue.Value = color.Blue;
                }));
                sliderGreen.Dispatcher.Invoke(DispatcherPriority.Normal, new System.Action(() =>
                {
                    sliderGreen.Value = color.Green;
                }));
            }
        }

        private void UpdateRectangle(Rgb color)
        {
            rect.Dispatcher.Invoke(DispatcherPriority.Normal, new System.Action(() =>
            {
                rect.Fill = new System.Windows.Media.SolidColorBrush(Color.FromArgb(255, color.Red, color.Green, color.Blue));

            }));
        }
        private void SendColorToHyPRDevice(Rgb color)
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
        void UpdateUI(string name, Rgb color, string tag)
        {
            UpdateName(name,tag);
            UpdateSliders(color, true);
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
            var user = (Patient)FindUserByCid(e.Rfid);
            if (user != null)
            {
                UpdateUI(user.Name, user.Color,user.Tag);
                SendColorToHyPRDevice(user.Color);
            }
            else
            {
                UpdateUI("", new Rgb(0, 0, 0), "");
                SendColorToHyPRDevice(new Rgb(0, 0, 0));
            }
        }

        private Patient FindUserByCid(string rfid)
        {
            return (Patient)Users.FirstOrDefault(usr => usr.Cid == rfid);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var user = FindUserByCid(_hyPrDevice.CurrentRfid);
            if (user !=null)
            {
                user.UpdateAllProperties(
                    new Patient
                        {
                            Id = user.Id,
                            Name = txtName.Text,
                            Color =
                                new Rgb(Convert.ToByte(sliderRed.Value), Convert.ToByte(sliderGreen.Value),
                                        Convert.ToByte(sliderBlue.Value)),
                            Cid = _hyPrDevice.CurrentRfid,
                            Tag = txtTag.Text
                        }
                    );
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
    }
}
