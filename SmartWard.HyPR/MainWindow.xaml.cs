using NooSphere.Infrastructure.ActivityBase;
using NooSphere.Infrastructure.Drivers;
using NooSphere.Primitives;
using NooSphere.Users;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using SmartWard.Model;

namespace SmartWard.HyPR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HyPrDevice hyPRDevice;
        private ActivitySystem activitySystem;
        public ObservableCollection<IUser> Users { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            InitializeWindow();
            InitializeUIComponents();
            InitializeDevice();

            foreach (UIElement el in grid.Children)
            {
                if (el != txtName)
                {
                    el.PreviewTouchDown += el_PreviewTouchDown;   
                    el.MouseDown += el_MouseDown;
                }
            }
            grid.TouchDown += el_PreviewTouchDown;
            grid.MouseDown += el_MouseDown;

            activitySystem = new ActivitySystem("http://localhost:8080");
            activitySystem.UserAdded += activitySystem_UserAdded;
            activitySystem.UserRemoved += activitySystem_UserRemoved;
            activitySystem.UserChanged += activitySystem_UserUpdated;

            Users = new ObservableCollection<IUser>(activitySystem.Users.Values.ToList());
        }

        private void activitySystem_UserUpdated(object sender, UserEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                for (int i = 0; i < Users.Count; i++)
                {
                    if (Users[i].Id == e.User.Id)
                    {
                        Users[i] = e.User;
                        break;
                    }
                }
            }));
        }

        private void activitySystem_UserRemoved(object sender, UserRemovedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                for (int i = 0; i < Users.Count; i++)
                {
                    if (Users[i].Id == e.Id)
                    {
                        Users.RemoveAt(i);
                        break;
                    }
                }

            }));
        }

        private void activitySystem_UserAdded(object sender, UserEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                Users.Add(e.User);

            }));
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
            hyPRDevice = new HyPrDevice();
            hyPRDevice.RfidDataReceived += hyPRDevice_RFIDDataReceived;
            hyPRDevice.RfidResetReceived += hyPRDevice_RFIDResetReceived;
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
            this.WindowStyle = System.Windows.WindowStyle.None;
        }

        private void sliders_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var rgb = new RGB(Convert.ToByte(sliderRed.Value), Convert.ToByte(sliderGreen.Value), Convert.ToByte(sliderBlue.Value));
            UpdateRectangle(rgb);
            SendColorToHyPRDevice(rgb);

        }
        void UpdateSliders(RGB color, bool upDateSliders = false)
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

        private void UpdateRectangle(RGB color)
        {
            rect.Dispatcher.Invoke(DispatcherPriority.Normal, new System.Action(() =>
            {
                rect.Fill = new System.Windows.Media.SolidColorBrush(Color.FromArgb(255, color.Red, color.Green, color.Blue));

            }));
        }
        private void SendColorToHyPRDevice(RGB color)
        {
                hyPRDevice.UpdateColor(color);
        }
        void UpdateName(string name,string tag)
        {
            txtName.Dispatcher.Invoke(DispatcherPriority.Normal, new System.Action(() =>
            {
                txtName.Text = name;
                txtTag.Text = tag;
            }));
        }
        void UpdateUI(string name, RGB color,string tag)
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
            var user = activitySystem.FindUserByCid(e.Rfid);
            if (user != null)
            {
                UpdateUI(user.Name, user.Color,user.Tag);
                SendColorToHyPRDevice(user.Color);
            }
            else
            {
                UpdateUI("",new RGB(0,0,0),"");
                SendColorToHyPRDevice(new RGB(0, 0, 0));
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var user = activitySystem.FindUserByCid(hyPRDevice.CurrentRfid);
            if (user !=null)
            {
                user.UpdateAllProperties(
                    new Patient
                        {
                            Name = txtName.Text,
                            Color =
                                new RGB(Convert.ToByte(sliderRed.Value), Convert.ToByte(sliderGreen.Value),
                                        Convert.ToByte(sliderBlue.Value)),
                            Cid = hyPRDevice.CurrentRfid,
                            Tag = txtTag.Text
                        }
                    );
                activitySystem.UpdateUser(user);
            }
            else
            {
                activitySystem.AddUser(
                    new Patient
                    {
                        Name = txtName.Text,
                        Color = new RGB(Convert.ToByte(sliderRed.Value), Convert.ToByte(sliderGreen.Value), Convert.ToByte(sliderBlue.Value)),
                        Cid = hyPRDevice.CurrentRfid,
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
