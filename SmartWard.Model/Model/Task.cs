﻿/****************************************************************************
 (c) 2012 Steven Houben(shou@itu.dk) and Søren Nielsen(snielsen@itu.dk)

 Pervasive Interaction Technology Laboratory (pIT lab)
 IT University of Copenhagen

 This library is free software; you can redistribute it and/or 
 modify it under the terms of the GNU GENERAL PUBLIC LICENSE V3 or later, 
 as published by the Free Software Foundation. Check 
 http://www.gnu.org/licenses/gpl.html for details.
****************************************************************************/

using System.Collections.Generic;
using SmartWard.Primitives;
using System.ComponentModel;

namespace SmartWard.Model
{
    public class Task : Noo
    {
        public Task()
        {
            InitializeProperties();
        }

        #region Initializers
        private void InitializeProperties()
        {
            Resources = new List<Resource>();
        }
        #endregion

        #region Properties
        private List<Resource> _resources;
        public List<Resource> Resources
        {
            get { return _resources; }
            set
            {
                _resources = value;
                OnPropertyChanged("Resources");
            }
        }
        #endregion
    }
}
