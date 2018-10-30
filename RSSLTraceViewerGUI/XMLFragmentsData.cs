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
        private int _streamId;
        private string _timeStamp;
        private string _xmlRawData;

        private string _requestKeyName;

        public XMLFragmentsData()
        {
            this._guid = string.Empty;
            this._mrnFragmentNumber = 0;
        }

        public int Index
        {
            get => this._index;
            set
            {
                this._index = value;
                this.OnPropertyChanged();
            }
        }

        public string TimeStamp
        {
            get => this._timeStamp;

            set
            {
                this._timeStamp = value;
                this.OnPropertyChanged();
            }
        }
        public string RequestKeyName
        {
            get => this._requestKeyName;

            set
            {
                this._requestKeyName = value;
                this.OnPropertyChanged();
            }
        }

        public string MsgDirection
        {
            get => this._msgDirection;

            set
            {
                this._msgDirection = value;
                this.OnPropertyChanged();
            }
        }


        public int MrnFragmentNumber
        {
            get => this._mrnFragmentNumber;
            set
            {
                this._mrnFragmentNumber = value;
                this.OnPropertyChanged();
            }
        }

        public string RdmMessageType
        {
            get => this._rdmMessageType;
            set
            {
                this._rdmMessageType = value;
                this.OnPropertyChanged();
            }
        }

        public int StreamId
        {
            get => this._streamId;
            set
            {
                this._streamId = value;
                this.OnPropertyChanged();
            }
        }

        public string DomainType
        {
            get => this._domainType;
            set
            {
                this._domainType = value;
                this.OnPropertyChanged();
            }
        }

        public string GUID
        {
            get => this._guid;
            set
            {
                this._guid = value;
                this.OnPropertyChanged();
            }
        }

        public string XmlRawData
        {
            get => this._xmlRawData;
            set
            {
                this._xmlRawData = value;
                this.OnPropertyChanged();
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