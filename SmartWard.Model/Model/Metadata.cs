/****************************************************************************
 (c) 2012 Steven Houben(shou@itu.dk) and Søren Nielsen(snielsen@itu.dk)

 Pervasive Interaction Technology Laboratory (pIT lab)
 IT University of Copenhagen

 This library is free software; you can redistribute it and/or 
 modify it under the terms of the GNU GENERAL PUBLIC LICENSE V3 or later, 
 as published by the Free Software Foundation. Check 
 http://www.gnu.org/licenses/gpl.html for details.
****************************************************************************/

using SmartWard.Primitives;
namespace SmartWard.Model
{
    public class Metadata:Base
    {
        public Metadata()
        {
        }
        private string header;
        public string Header
        {
            get { return this.header; }
            set
            {
                this.header = value;
                OnPropertyChanged("header");
            }
        }
        private string data;
        public string Data
        {
            get { return this.data; }
            set
            {
                this.data = value;
                OnPropertyChanged("data");
            }
        }
    }
}
