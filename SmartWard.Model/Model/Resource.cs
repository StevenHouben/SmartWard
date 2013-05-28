
﻿/****************************************************************************
 (c) 2012 Steven Houben(shou@itu.dk) and Søren Nielsen(snielsen@itu.dk)

 Pervasive Interaction Technology Laboratory (pIT lab)
 IT University of Copenhagen

 This library is free software; you can redistribute it and/or 
 modify it under the terms of the GNU GENERAL PUBLIC LICENSE V3 or later, 
 as published by the Free Software Foundation. Check 
 http://www.gnu.org/licenses/gpl.html for details.
****************************************************************************/

using System;
using System.Globalization;
using SmartWard.Primitives;

namespace SmartWard.Model
{
    public class Resource : Noo
    {
        public Resource()
        {
            InitializeTimeStamps();
        }
        public Resource(int size,string name)
        {
            InitializeTimeStamps();
            Name = name;
            Size = size;
        }

        private void InitializeTimeStamps()
        {
            CreationTime = DateTime.Now.ToString("u");
            LastWriteTime = DateTime.Now.ToString("u");
        }

        private Guid activityId;
        public Guid ActivityId
        {
            get { return this.activityId; }
            set
            {
                this.activityId = value;
                OnPropertyChanged("activityId");
            }
        }

        private int size;
        public int Size
        {
            get { return this.size; }
            set
            {
                this.size = value;
                OnPropertyChanged("size");
            }
        }

        private string creationTime;
        public string CreationTime
        {
            get { return this.creationTime; }
            set
            {
                this.creationTime = value;
                OnPropertyChanged("creationTime");
            }
        }

        private string lastWriteTime;
        public string LastWriteTime
        {
            get { return this.lastWriteTime; }
            set
            {
                this.lastWriteTime = value;
                OnPropertyChanged("lastWriteTime");
            }
        }

        public string RelativePath { get {return ActivityId +"/"+ Name; }}
        public string CloudPath { get { return "Activities/" + ActivityId + "/Resources/" + Id; } }
    }
}
