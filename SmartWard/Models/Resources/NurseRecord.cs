using NooSphere.Model.Primitives;

namespace SmartWard.Models
{
    public class NurseRecord : Base
    {
        private string _title;


        public NurseRecord()
        {
            Date = System.String.Format("{0:d/M/yyyy HH:mm:ss}", System.DateTime.Now);
        }
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

        private MessageFlags _flag;

        public MessageFlags MessageFlag
        {
            get { return _flag; }
            set
            {
                _flag = value;
                OnPropertyChanged("Flag");
            }
        }
        private string _date;
        public string Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged("Date");
            }
        }
    }

    public enum MessageFlags
    {
        Comment,
        Event,
        Surgery,
        In,
        Out
    }
}
