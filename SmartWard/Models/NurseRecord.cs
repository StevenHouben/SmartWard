using ABC.Model.Primitives;

namespace SmartWard.Models
{
    public class NurseRecord:Base
    {
        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        private string _body;

        public string Body
        {
            get { return _body; }
            set
            {
                _body = value;
                OnPropertyChanged("Body");
            }
        }

        private MessageFlag _flag;

        public MessageFlag Flag
        {
            get { return _flag; }
            set
            {
                _flag = value;
                OnPropertyChanged("Flag");
            }
        }

    }

    public enum MessageFlag
    {
        Normal,
        Critical
    }
}
