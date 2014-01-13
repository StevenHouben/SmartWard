using ABC.Model.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Models.Notifications
{
    public class Notification : Noo
    {
        private IList<string> _to;

        private IList<string> _seenBy;
        private string _referenceId;
        private string _referenceType;
        private string _message;

        public Notification(List<string> to, string referenceId, string referenceType, string message)
        {
            _to = to;
            _seenBy = new List<string>();
            _referenceId = referenceId;
            _referenceType = referenceType;
            _message = message;
        }

        #region properties
        public IList<string> To { get { return _to; } }
        public IList<string> SeenBy { get { return _seenBy; } }
        public string ReferenceId { get { return _referenceId; } }
        public string ReferenceType { get { return _referenceType; } }
        public string Message { get { return _message; } }
        #endregion
    }
}
