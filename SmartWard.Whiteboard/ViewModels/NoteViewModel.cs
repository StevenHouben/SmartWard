using SmartWard.Infrastructure;
using SmartWard.Models;
using SmartWard.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Whiteboard.ViewModels
{
    public class NoteViewModel : ViewModelBase
    {
        private readonly Note _note;
        private WardNode _wardNode;

        #region properties
        public Note Note
        {
            get { return _note; }
        }
        public WardNode WardNode
        {
            get { return _wardNode; }
            set
            {
                _wardNode = value;
            }
        }
        #endregion

        public NoteViewModel(Note note, WardNode wardNode)
        {
            _note = note;
            WardNode = wardNode;
        }

        public void UpdateNote()
        {   
            WardNode.UpdateResource(Note);
        }

        public string Summary
        {
            get { return (Note.Fasting ? "F, " : "") + Note.Text.Substring(0, Note.Text.Length > 10 ? 10 : Note.Text.Length) + "..."; }
        }

        public void NoteChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Text" || e.PropertyName == "Fasting")
            {
                OnPropertyChanged("Summary");
            }
        }

    }
}
