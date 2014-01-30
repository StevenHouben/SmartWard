using SmartWard.Infrastructure;
using SmartWard.Models.Notifications;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace SmartWard.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {

        protected ViewModelBase(){}

        protected ViewModelBase(bool throwOnInvalidPropertyName)
        {
            ThrowOnInvalidPropertyName = throwOnInvalidPropertyName;
        }

        #region RequestClose [event]

        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        public event EventHandler RequestClose;

        public virtual void OnRequestClose()
        {
            var handler = RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion // RequestClose [event]

        #region DisplayName

        /// <summary>
        /// Returns the user-friendly name of this object.
        /// Child classes can set this property to a new value,
        /// or override it to determine the value on-demand.
        /// </summary>
        public virtual string DisplayName { get; protected set; }

        #endregion // DisplayName

        #region Debugging Aides

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This 
        /// method does not exist in a Release build.
        /// </summary>
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] != null) return;
            var msg = "Invalid property name: " + propertyName;

            if (ThrowOnInvalidPropertyName)
                throw new Exception(msg);
            Debug.Fail(msg);
        }

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might 
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion // Debugging Aides

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
            VerifyPropertyName(propertyName);

            var handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion // INotifyPropertyChanged Members

        #region IDisposable Members

        /// <summary>
        /// Invoked when this object is being removed from the application
        /// and will be subject to garbage collection.
        /// </summary>
        public void Dispose()
        {
            OnDispose();
        }

        /// <summary>
        /// Child classes can override this method to perform 
        /// clean-up logic, such as removing event handlers.
        /// </summary>
        protected virtual void OnDispose()
        {
        }

#if DEBUG
        /// <summary>
        /// Useful for ensuring that ViewModel objects are properly garbage collected.
        /// </summary>
        ~ViewModelBase()
        {
            string msg = string.Format("{0} ({1}) ({2}) Finalized", GetType().Name, DisplayName, GetHashCode());
            Debug.WriteLine(msg);
        }
#endif

        #endregion // IDisposable Members

        #region Global Notifications

        public WardNode NotificationsNode { get; set;}
        public bool NotificationsEnabled { get { return NotificationsNode != null; } }
        public ObservableCollection<NotificationViewModel> Notifications { get; set; }

        protected void InitializeNotifications(WardNode node)
        {
            NotificationsNode = node;
            Notifications = new ObservableCollection<NotificationViewModel>();
            Notifications.CollectionChanged += Notifications_CollectionChanged;

            NotificationsNode.NotificationAdded += WardNode_NotificationAdded;
            NotificationsNode.NotificationRemoved += WardNode_NotificationRemoved;
            NotificationsNode.NotificationChanged += WardNode_NotificationChanged;

            NotificationsNode.NotificationCollection.ToList().ForEach(n => Notifications.Add(new NotificationViewModel((Notification)n)));
        }

        void Notifications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var list = e.NewItems;
                foreach (var item in list)
                {
                    var notification = item as NotificationViewModel;
                    if (notification == null) return;
                    notification.NotificationUpdated += NotificationUpdated;
                }
            }
        }

        void NotificationUpdated(object sender, EventArgs e)
        {
            NotificationsNode.UpdateNotification((Notification)sender);
        }

        void WardNode_NotificationAdded(object sender, NooSphere.Model.Notifications.Notification notification)
        {
            Notifications.Add(new NotificationViewModel((Notification)notification));
        }

        void WardNode_NotificationChanged(object sender, NooSphere.Model.Notifications.Notification notification)
        {
            var index = -1;
            //Find notification
            var n = Notifications.FirstOrDefault(nn => nn.Id == notification.Id);
            if (n == null)
                return;

            index = Notifications.IndexOf(n);

            if (index == -1)
                return;

            Notifications[index] = new NotificationViewModel((Notification)notification);
            Notifications[index].NotificationUpdated += NotificationUpdated;
        }
        void WardNode_NotificationRemoved(object sender, NooSphere.Model.Notifications.Notification notification)
        {
            foreach (var n in Notifications.ToList())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (n.Id == notification.Id)
                        Notifications.Remove(n);
                });
            }
        }
        #endregion
    }
}
