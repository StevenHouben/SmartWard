using NooSphere.Model.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWard.Models.Notifications
{
    public class Notification : NooSphere.Model.Notifications.Notification
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
            Type = typeof(Notification).Name;
        }

        #region properties
        public IList<string> To { get { return _to; } set { _to = value; } }
        public IList<string> SeenBy { get { return _seenBy; } set { _seenBy = value; } }
        public string ReferenceId { get { return _referenceId; } set { _referenceId = value; } }
        public string ReferenceType { get { return _referenceType; } set { _referenceType = value; } }
        public string Message { get { return _message; } set { _message = value; } }
        #endregion

        internal void SetSeenBy(NooSphere.Model.Users.User user)
        {
            throw new NotImplementedException();
        }
    }
}
