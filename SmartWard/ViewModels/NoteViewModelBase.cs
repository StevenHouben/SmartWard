using SmartWard.Infrastructure;
using SmartWard.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.ViewModels
{
    public class NoteViewModelBase : ResourceViewModelBase
    {
        private string _identifier;
        public NoteViewModelBase(Note note, WardNode wardNode) : base(note) 
        {
            var patient = wardNode.UserCollection.FirstOrDefault(u => u.Type == typeof(Patient).Name && u.Id == note.PatientId) as Patient;
            if (patient != null) Identifier = patient.Name + ": " + patient.Cpr;
            note.PropertyChanged += NoteChanged;
            WardNode = wardNode;
        }

        public Note Note { get { return Resource as Note; } }
        public WardNode WardNode { get; set; }

        #region Note Properties
        public string Identifier
        {
            get { return _identifier; }
            set
            {
                _identifier = value;
                OnPropertyChanged("Identifier");
            }
        }
        public string PatientId
        {
            get { return Note.PatientId; }
            set
            {
                Note.PatientId = value;
                OnPropertyChanged("PatientId");
            }
        }
        public string Text
        {
            get { return Note.Text; }
            set
            {
                Note.Text = value;
                OnPropertyChanged("text");
            }
        }
        public bool Fasting
        {
            get { return Note.Fasting; }
            set
            {
                Note.Fasting = value;
                OnPropertyChanged("fasting");
            }
        }
        public string Summary
        {
            get {
                if (String.IsNullOrEmpty(Note.Text) && Note.Fasting) return "F";
                return (Note.Fasting ? "F, " : "") + Note.Text.Substring(0, Note.Text.Length > 10 ? 7 : Note.Text.Length) + (Note.Text.Length > 10 ? "..." : ""); 
            }
        }

        public void NoteChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "text" || e.PropertyName == "fasting")
            {
                OnPropertyChanged("Summary");
            }
        }
        #endregion
    }
}
