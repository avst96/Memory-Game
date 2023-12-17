using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace MemoryGameSystem
{
    public class Card : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private List<string> allproperties = new() { "ForeColor", "BackColor", "IsVisible", "BackColorMAUI", "ForeColorMAUI" };
        private CardStatusEnum _cardstatus = CardStatusEnum.Facedown;
        private char _cardpicture;
        public enum CardStatusEnum { Facedown, Faceup, Claimed };
        public CardStatusEnum CardStatus
        {
            get => _cardstatus;
            set
            {
                _cardstatus = value;
                InvokePropertyChanged(true);
            }
        }
        public char CardPicture
        {
            get => _cardpicture;
            internal set { _cardpicture = value; InvokePropertyChanged(); }
        }

        public System.Drawing.Color BackColor { get => CardStatus == CardStatusEnum.Facedown ? Color.Orange : Color.LightGoldenrodYellow; }
        public Microsoft.Maui.Graphics.Color BackColorMAUI { get => ConvertToMauiColor(BackColor); }
        public System.Drawing.Color ForeColor { get => CardStatus == CardStatusEnum.Faceup ? Color.Black : BackColor; }
        public Microsoft.Maui.Graphics.Color ForeColorMAUI { get => ConvertToMauiColor(ForeColor); }
        public bool IsVisible { get => CardStatus == CardStatusEnum.Claimed ? false : true; }

        private Microsoft.Maui.Graphics.Color ConvertToMauiColor(System.Drawing.Color systemColor)
        {
            float red = systemColor.R / 255f;
            float green = systemColor.G / 255f;
            float blue = systemColor.B / 255f;
            float alpha = systemColor.A / 255f;

            return new Microsoft.Maui.Graphics.Color(red, green, blue, alpha);
        }
        private void InvokePropertyChanged(bool All = false, [CallerMemberName] string propertyname = "")
        {
            if (All) { allproperties.ForEach(p => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p))); }
            else { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname)); }
        }
    }
}
