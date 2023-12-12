using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace MemoryGameSystem
{
    public class Card : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;


        public char CardPicture {  get; internal set; }

        public Color CardColors { get; private set; }

        //add for forecolor??
        public void ShowCard()
        {

        }
        public void HideCard()
        {

        }

        public void Clear()
        {
           
        }




        private void InvokePropertyChanged([CallerMemberName] string propertyname = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
