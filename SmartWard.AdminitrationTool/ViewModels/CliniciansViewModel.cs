using SmartWard.Commands;
using SmartWard.Infrastructure;
using SmartWard.Models;
using SmartWard.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SmartWard.AdministrationTool.ViewModels
{
    internal class CliniciansViewModel
    {
        public WardNode WardNode { get; set; }
        public CliniciansViewModel(WardNode systemNode)
        {
            WardNode = systemNode;

            Clinicians = new ObservableCollection<ClinicianViewModel>();
            Clinicians.CollectionChanged += Clinicians_CollectionChanged;

            WardNode.UserAdded += WardNode_ClinicianAdded;
            WardNode.UserRemoved += WardNode_ClinicianRemoved;

            WardNode.UserChanged += WardNode_ClinicianChanged;
            WardNode.UserCollection.Where(u => u.Type.Equals(typeof(Clinician).Name)).ToList().ForEach(c => Clinicians.Add(new ClinicianViewModel((Clinician)c)));
        }
        #region CliniciansCollection
        public ObservableCollection<ClinicianViewModel> Clinicians { get; set; }
        void WardNode_ClinicianAdded(object sender, NooSphere.Model.Users.User user)
        {
            if(user.Type.Equals(typeof(Clinician).Name))
                Clinicians.Add(new ClinicianViewModel((Clinician)user));
        }
        void WardNode_ClinicianChanged(object sender, NooSphere.Model.Users.User user)
        {
            var index = -1;
            if (user.Type.Equals("Clinician")) {
                //Find clinician
                var a = Clinicians.FirstOrDefault(t => t.Id == user.Id);
                if (a == null)
                    return;

                index = Clinicians.IndexOf(a);

                if (index == -1)
                    return;

                Clinicians[index].Clinician.UpdateAllProperties((Clinician)user);
                Clinicians[index].ClinicianUpdated += ClinicianUpdated;
            }
        }
        void WardNode_ClinicianRemoved(object sender, NooSphere.Model.Users.User user)
        {
            foreach (var a in Clinicians.ToList())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (a.Id == user.Id)
                        Clinicians.Remove(a);
                });
            }
        }
        void Clinicians_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var list = e.NewItems;
                foreach (var item in list)
                {
                    var clinician = item as ClinicianViewModel;
                    if (clinician == null) return;
                    clinician.ClinicianUpdated += ClinicianUpdated;
                }
            }
        }
        void ClinicianUpdated(object sender, EventArgs e)
        {
            WardNode.UpdateUser((Clinician)sender);
        }
        #endregion

        private ICommand _addClinicianCommand;

        public ICommand AddClinicianCommand
        {
            get
            {
                return _addClinicianCommand ?? (_addClinicianCommand = new RelayCommand(
                    param => AddNewAnonymousClinician(),
                    param => true
                    ));
            }
        }

        private void AddNewAnonymousClinician()
        {
            WardNode.AddUser(new Clinician(Clinician.ClinicianTypeEnum.Doctor, "nfcId" + DateTime.Now.ToString()));
        }
    }
}
