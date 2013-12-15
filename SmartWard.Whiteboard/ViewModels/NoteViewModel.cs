using SmartWard.Infrastructure;
using SmartWard.Models;
using SmartWard.ViewModels;
using System;
using System.Collections.Generic;
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
        public event EventHandler EWSUpdated;

        public NoteViewModel(Note note, WardNode wardNode)
        {
            _note = note;
            WardNode = wardNode;
        }

    }
}
