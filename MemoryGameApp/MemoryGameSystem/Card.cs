using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace MemoryGameSystem
{
    public class Card : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public enum CardStatusEnum { Facedown, Faceup, Hidden};
        internal CardStatusEnum CardStatus { get; set; }
       //public char CardPicture {  get; internal set; }

        public System.Drawing.Color BackColor { get => Color.FromArgb(CardStatus == CardStatusEnum.Hidden ? 255 : 0, 255, 165, 0); }
        public System.Drawing.Color ForeColor { get => CardStatus == CardStatusEnum.Faceup ? Color.Black : BackColor; }
        //public void ShowCard()
        //{

        //}
        //public void HideCard()
        //{

        //}

        //public void Clear()
        //{
           
        //}




        private void InvokePropertyChanged([CallerMemberName] string propertyname = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
