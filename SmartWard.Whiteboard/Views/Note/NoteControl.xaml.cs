using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;


namespace SmartWard.Whiteboard.Views.Note
{
    /// <summary>
    /// Interaction logic for EWSControl.xaml
    /// </summary>
    public partial class NoteControl : UserControl
    {
        public NoteControl()
        {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ((Popup)((Grid)this.Parent).Parent).IsOpen = false;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
