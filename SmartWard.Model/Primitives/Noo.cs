/****************************************************************************
 (c) 2012 Steven Houben(shou@itu.dk) and Søren Nielsen(snielsen@itu.dk)

 Pervasive Interaction Technology Laboratory (pIT lab)
 IT University of Copenhagen

 This library is free software; you can redistribute it and/or 
 modify it under the terms of the GNU GENERAL PUBLIC LICENSE V3 or later, 
 as published by the Free Software Foundation. Check 
 http://www.gnu.org/licenses/gpl.html for details.
****************************************************************************/

using System;
using System.ComponentModel;

namespace SmartWard.Primitives
{
    public class Noo : Base
    {
        public Noo()
        {
            Name = "default";
            Id = Guid.NewGuid().ToString();
            Description = "default";
        }

        private string id;
        public string Id 
        { 
            get{return this.id;}
            set
            {
                this.id=value;
                OnPropertyChanged("id");
            }
        }

        private string name;
        public string Name
        {
            get { return this.name; }
            set
            {
                this.name = value;
                OnPropertyChanged("name");
            }
        }

        private string description;
        public string Description
        {
            get { return this.description; }
            set
            {
                this.description = value;
                OnPropertyChanged("description");
            }
        }

        private string uri;
        public string Uri
        {
            get { return this.uri; }
            set
            {
                this.uri = value;
                OnPropertyChanged("uri");
            }
        }
        public bool Equals(Noo id)
        {
            return Id == id.Id;
        }
    }
}
