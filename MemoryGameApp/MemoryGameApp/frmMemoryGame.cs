namespace MemoryGameApp
{
    public partial class frmMemoryGame : Form
    {
        List<Button> allcards;
        List<List<Button>> sets = new();
        Random rnd = new();

        public frmMemoryGame()
        {
            InitializeComponent();
            allcards = new() { btnCard1, btnCard2, btnCard3, btnCard4, btnCard5, btnCard6, btnCard7, btnCard8, btnCard9, btnCard10, btnCard11, btnCard12, btnCard13, btnCard14, btnCard15, btnCard16, btnCard17, btnCard18, btnCard19, btnCard20 };
            allcards.ForEach(c => c.Click += Card_Click);
            btnStart.Click += BtnStart_Click;
        }

        private void ExposeCard(Button btn)
        {
            btn.BackColor = Color.LightGoldenrodYellow;
            btn.ForeColor = Color.Black;
        }
        private void HideCard()
        {
            //Figure out how to pass in which card to hide
            btnCard1.BackColor = Color.Orange;
            btnCard1.ForeColor = btnCard1.BackColor;
        }
        private void Card_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                ExposeCard(btn);
            }
        }
        private void BtnStart_Click(object? sender, EventArgs e)
        {
            List<Button> remaingcards = allcards;

            //In this for loop it will add sets of two cards to the sets list for all cards
            for (int i = 0; i < 9; i++)
            {
                int cardone = rnd.Next(remaingcards.Count());
                int cardtwo = rnd.Next(remaingcards.Count());
                sets.Add(new() { remaingcards[cardone], remaingcards[cardtwo] });
                remaingcards.RemoveAt(cardone);
                remaingcards.RemoveAt(cardtwo);
            }
            //Will add picture to set
            string picture = "A";
            sets.ForEach(s => 
            {
                s.ForEach(c => c.Text = picture.ToString());
                picture++;
            });
        }
    }
}
//?? Do I need a set list, or should I just add the text to cardone and cardtwo and then remove them from the list of cards?