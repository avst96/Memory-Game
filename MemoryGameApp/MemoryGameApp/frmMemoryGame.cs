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
            List<Button> remaingcards = new();
            remaingcards = allcards;

            //In this for loop it will add sets of two cards to the sets list for all cards
            while (remaingcards.Count() > 1)
            {
                int card_one = rnd.Next(remaingcards.Count());
                int card_two = rnd.Next(remaingcards.Count());
                while (card_one == card_two)
                {
                    card_two = rnd.Next(remaingcards.Count());
                }

                sets.Add(new() { remaingcards[card_one], remaingcards[card_two] });
                remaingcards.RemoveAt(card_one);
                //To make sure it doesn't attempt to remove an index that is out off bound because card 1 was already removed
                if (card_one < card_two)
                {
                    card_two--;
                }
                remaingcards.RemoveAt(card_two);
            }
            //Will add different picture to set
            char picture = 'I';
            sets.ForEach(s =>
            {
                s.ForEach(c => c.Text = picture.ToString());
                picture++;
            });
        }
    }
}
