using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Primitives
{
    public class RGB: Base
    {
        private byte red;
        public byte Red
        {
            get { return this.red; }
            set
            {
                this.red = value;
                OnPropertyChanged("red");
            }
        }
        private byte blue;
        public byte Blue
        {
            get { return this.blue; }
            set
            {
                this.blue = value;
                OnPropertyChanged("blue");
            }
        }
        private byte green;
        public byte Green
        {
            get { return this.green; }
            set
            {
                this.green = value;
                OnPropertyChanged("green");
            }
        }

        public RGB(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }
        public override string ToString()
        {
            return "C" + Red + " " + Green + " " + Blue + "#";
        }
    }

    public class RGBS
    {
        public static RGB Red { get { return new RGB(255, 0, 0); } }
        public static RGB Green { get { return new RGB(0, 255, 0); } }
        public static RGB Blue { get { return new RGB(0, 0, 255); } }
        public static RGB Yellow { get { return new RGB(255, 255, 0); } }
        public static RGB Cyan { get { return new RGB(0, 255, 255); } }
        public static RGB Magenta { get { return new RGB(255, 0, 255); } }
        public static RGB Black { get { return new RGB(0, 0, 0); } }
        public static RGB White { get { return new RGB(255, 255, 255); } }
        public static RGB Gray { get { return new RGB(192, 192, 192); } }
    }

}
