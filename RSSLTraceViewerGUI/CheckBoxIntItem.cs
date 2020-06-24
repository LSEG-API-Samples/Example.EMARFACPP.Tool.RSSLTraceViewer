using System.ComponentModel;
using System.Runtime.CompilerServices;
using RSSLTraceViewerGUI.Annotations;

namespace RSSLTraceViewerGUI
{
    public class CheckBoxIntItem : INotifyPropertyChanged
    {
        private bool _isChecked;
        private int _item;

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        public int Item
        {
            get => _item;
            set
            {
                _item = value;
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