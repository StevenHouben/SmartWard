using Microsoft.Surface.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Extensions
{
    public class OnScreenKeyboardTextBox : SurfaceTextBox
    {
        private Process _keyboard;
        public OnScreenKeyboardTextBox()
        {
            GotFocus += LaunchKeyboard;
            LostFocus += CloseKeyboard;
        }

        private void LaunchKeyboard(object sender, System.Windows.RoutedEventArgs e)
        {
            _keyboard = Process.Start(@"C:\Program Files\Common Files\Microsoft Shared\ink\TabTip.exe");
            
        }

        private void CloseKeyboard(object sender, System.Windows.RoutedEventArgs e)
        {
            _keyboard.CloseMainWindow();
            _keyboard.Close();
        }
    }
}
