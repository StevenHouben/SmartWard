using SmartWard.Commands;
using SmartWard.Infrastructure;
using SmartWard.Models;
using SmartWard.PDA.Helpers;
using SmartWard.PDA.Views;
using SmartWard.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SmartWard.PDA.ViewModels
{
    public class UpdatableNoteViewModel : NoteViewModelBase
    {

        public UpdatableNoteViewModel(Note n, WardNode wardNode) : base(n, wardNode) { }

        public event EventHandler ResourceUpdated;
        private ICommand _updateResourceCommand;
        public ICommand UpdateResourceCommand
        {
            get
            {
                return _updateResourceCommand ?? (_updateResourceCommand = new RelayCommand(
                    param => UpdateResource(),
                    param => true
                    ));
            }
        }

        public void UpdateResource()
        {
            Note.UpdatedBy = AuthenticationHelper.User.Id;
            Note.Updated = DateTime.Now;
            Note.SeenBy = new List<string>(); // Resetting seenby list, after updating resource.
            if (WardNode.ResourceCollection.Where(r => r.Id.Equals(Note.Id)).ToList().FirstOrDefault() != null)
                WardNode.UpdateResource(Note);
            else
                WardNode.AddResource(Note);

            PDAWindow pdaWindow = (PDAWindow)Application.Current.MainWindow;
            NavigationHelper.NavigateBack(pdaWindow);
        }

        
    }
}
