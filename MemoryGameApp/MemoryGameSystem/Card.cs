using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace MemoryGameSystem
{
    public class Card : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private CardStatusEnum _cardstatus = CardStatusEnum.Facedown;
        private char _cardpicture;
        public enum CardStatusEnum { Facedown, Faceup, Claimed };
        internal CardStatusEnum CardStatus
        {
            get => _cardstatus;
            set
            {
                _cardstatus = value;
                InvokePropertyChanged("ForeColor");
                InvokePropertyChanged("BackColor");
                InvokePropertyChanged("IsVisible");
            }
        }
        public char CardPicture
        {
            get => _cardpicture;
            internal set { _cardpicture = value; InvokePropertyChanged(); }
        }

        public System.Drawing.Color BackColor { get => CardStatus == CardStatusEnum.Facedown ? Color.Orange : Color.LightGoldenrodYellow; }
        public System.Drawing.Color ForeColor { get => CardStatus == CardStatusEnum.Faceup ? Color.Black : BackColor; }

        public bool IsVisible { get => CardStatus == CardStatusEnum.Claimed ? false : true; }

        private void InvokePropertyChanged([CallerMemberName] string propertyname = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
