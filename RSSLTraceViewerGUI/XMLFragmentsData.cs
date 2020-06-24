using System.ComponentModel;
using System.Runtime.CompilerServices;
using RSSLTraceViewerGUI.Annotations;

namespace RSSLTraceViewerGUI
{
    public partial class XMLFragmentsData : INotifyPropertyChanged
    {
        private string _domainType;
        private string _guid;
        private int _index;
        private int _mrnFragmentNumber;
        private string _msgDirection;
        private string _rdmMessageType;

        private string _requestKeyName;
        private int _streamId;
        private string _timeStamp;
        private string _xmlRawData;

        public XMLFragmentsData()
        {
            _guid = string.Empty;
            _mrnFragmentNumber = 0;
        }

        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                OnPropertyChanged();
            }
        }

        public string TimeStamp
        {
            get => _timeStamp;

            set
            {
                _timeStamp = value;
                OnPropertyChanged();
            }
        }

        public string RequestKeyName
        {
            get => _requestKeyName;

            set
            {
                _requestKeyName = value;
                OnPropertyChanged();
            }
        }

        public string MsgDirection
        {
            get => _msgDirection;

            set
            {
                _msgDirection = value;
                OnPropertyChanged();
            }
        }


        public int MrnFragmentNumber
        {
            get => _mrnFragmentNumber;
            set
            {
                _mrnFragmentNumber = value;
                OnPropertyChanged();
            }
        }

        public string RdmMessageType
        {
            get => _rdmMessageType;
            set
            {
                _rdmMessageType = value;
                OnPropertyChanged();
            }
        }

        public int StreamId
        {
            get => _streamId;
            set
            {
                _streamId = value;
                OnPropertyChanged();
            }
        }

        public string DomainType
        {
            get => _domainType;
            set
            {
                _domainType = value;
                OnPropertyChanged();
            }
        }

        public string GUID
        {
            get => _guid;
            set
            {
                _guid = value;
                OnPropertyChanged();
            }
        }

        public string XmlRawData
        {
            get => _xmlRawData;
            set
            {
                _xmlRawData = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}