using Microsoft.Surface.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace SmartWard.HyPR.Views
{
    /// <summary>
    /// Interaction logic for MenuButton.xaml
    /// </summary>
    public partial class MenuButton : SurfaceButton,INotifyPropertyChanged
    {
        private string _imageSourceString;
       public string ImageSourceString 
       {
           get { return _imageSourceString; }
           set 
           {
               _imageSourceString = value;
               OnPropertyChanged("ImageSourceString");
           }
       }

        private string _text;
       public string Text 
       {
           get {return _text; }
           set
           {
               _text = value;
               OnPropertyChanged("Text");
           }
       }

       private RenderStyle _renderStyle;
       public RenderStyle RenderStyle
       {
           get { return _renderStyle; }
           set
           {
               _renderStyle = value;
               SetRenderStyle(_renderStyle);
           }
       }

        private void SetRenderStyle(Views.RenderStyle _renderStyle)
        {
 	        if(_renderStyle == Views.RenderStyle.Icon)
                Label.Visibility = System.Windows.Visibility.Hidden;
            else
                Label.Visibility = System.Windows.Visibility.Visible;

        }
        public MenuButton()
        {
            InitializeComponent();
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {

            var handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion // INotifyPropertyChanged Members

    }
    public enum RenderStyle
    {
        Icon,
        IconAndText
    }
}
