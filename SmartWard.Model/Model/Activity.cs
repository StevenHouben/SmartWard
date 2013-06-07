/****************************************************************************
 (c) 2012 Steven Houben(shou@itu.dk) and Søren Nielsen(snielsen@itu.dk)

 Pervasive Interaction Technology Laboratory (pIT lab)
 IT University of Copenhagen

 This library is free software; you can redistribute it and/or 
 modify it under the terms of the GNU GENERAL PUBLIC LICENSE V3 or later, 
 as published by the Free Software Foundation. Check 
 http://www.gnu.org/licenses/gpl.html for details.
****************************************************************************/

using System.Collections.Generic;
using SmartWard.Users;
using SmartWard.Primitives;

namespace SmartWard.Model
{
    /// <summary>
    /// Activity Base Class
    /// </summary>
    public class Activity : Noo, IActivity
    {
        #region Constructors

        public Activity()
        {
            this.BaseType = typeof(IActivity).Name;
            InitializeProperties();
        }

        #endregion

        #region Initializers

        private void InitializeProperties()
        {
            Actions = new List<Action>();
            Participants = new List<User>();
            Meta = new Metadata();
            Resources =  new List<Resource>();
        }

        #endregion

        #region Properties

        private User owner;
        public User Owner
        {
            get { return this.owner; }
            set
            {
                this.owner = value;
                OnPropertyChanged("owner");
            }
        }
        private List<User> participants;
        public List<User> Participants
        {
            get { return this.participants; }
            set
            {
                this.participants = value;
                OnPropertyChanged("participants");
            }
        }
        private List<Action> actions;
        public List<Action> Actions
        {
            get { return this.actions; }
            set
            {
                this.actions = value;
                OnPropertyChanged("actions");
            }
        }
        private Metadata meta;
        public Metadata Meta
        { 
            get{return this.meta;}
            set
            {
                this.meta=value;
                OnPropertyChanged("meta");
            }
        }
        private List<Resource> resources;
        public List<Resource> Resources
        {
            get { return this.resources; }
            set
            {
                this.resources = value;
                OnPropertyChanged("resouces");
            }
        }

        #endregion

        #region Public Methods

        public List<Resource> GetResources()
        {
            return Resources;
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(Activity act)
        {
            return Id == act.Id;
        }

        #endregion
    }
}