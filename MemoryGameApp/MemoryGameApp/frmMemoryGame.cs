namespace MemoryGameApp
{
    public partial class frmMemoryGame : Form
    {
        List<Button> allcards;
        List<List<Button>> sets = new();
        List<Button> pickedcards = new();

        Random rnd = new();
        enum GameStatusEnum { playing, finished, notstarted };
        GameStatusEnum gamestatus = GameStatusEnum.notstarted;
        enum TurnEnum { player1, player2 };
        TurnEnum currentturn = TurnEnum.player1;
        public frmMemoryGame()
        {
            InitializeComponent();
            allcards = new() { btnCard1, btnCard2, btnCard3, btnCard4, btnCard5, btnCard6, btnCard7, btnCard8, btnCard9, btnCard10, btnCard11, btnCard12, btnCard13, btnCard14, btnCard15, btnCard16, btnCard17, btnCard18, btnCard19, btnCard20 };
            allcards.ForEach(c => c.Click += Card_Click);
            btnStart.Click += BtnStart_Click;
        }

        private void ExposeCard(Button btn)
        {
            if (gamestatus == GameStatusEnum.playing && pickedcards.Count() < 2)
            {
                btn.BackColor = Color.LightGoldenrodYellow;
                btn.ForeColor = Color.Black;
                pickedcards.Add(btn);
                if (pickedcards.Count() == 2)
                {
                   HideCard();
                }
            }
        }
        private void HideCard()
        {
            //Adds a 3 second delay
                        DateTime starttime = DateTime.Now;
            while ((DateTime.Now - starttime).TotalSeconds < 3)
            {
                Application.DoEvents();
            }

            //Turns back over exposed cards, and then clears list of exposed cards and changes turn
            pickedcards.ForEach(card =>
            {
                card.BackColor = Color.Orange;
                card.ForeColor = btnCard1.BackColor;
            });
            pickedcards.Clear();
            currentturn = currentturn == TurnEnum.player1 ? TurnEnum.player2 : TurnEnum.player1;
            SetMessage();
        }
        private void ShuffleCards()
        {
            char picture = 'I';
            List<Button> remaingcards = new();
            allcards.ForEach(b => remaingcards.Add(b));

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

                //The following removes the cards that already have a picture
                remaingcards.RemoveAt(card_one);
                //To make sure it doesn't attempt to remove an index that is out off bound because card 1 was already removed
                if (card_one < card_two)
                {
                    card_two--;
                }
                remaingcards.RemoveAt(card_two);
            }

            //Will add different picture to each set
            //??Need help to figure out why the char keeps on moving forward and doesn't reuse, then change back to windings font
            sets.ForEach(s =>
                  {
                      s.ForEach(c => c.Text = picture.ToString());
                      picture++;
                  });
        }
        private void SetMessage()
        {
            string msg = "Press Start Game to start";
            if (gamestatus == GameStatusEnum.playing)
            {
                msg = currentturn == TurnEnum.player1 ? "Player 1's Turn" : "Player 2's Turn";
            }
            else if (gamestatus == GameStatusEnum.finished)
            {
                int playeronescore = int.Parse(txtPlayer1Sets.Text);
                int platertwoscore = int.Parse(txtPlayer2Sets.Text);
                if (platertwoscore == playeronescore)
                {
                    msg = "Tie!";
                }
                else
                {
                    msg = playeronescore > platertwoscore ? "Player 1 won!" : "Player 2 won!";
                }
            }
            lblMessage.Text = msg;
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
            gamestatus = GameStatusEnum.playing;
            SetMessage();
            ShuffleCards();
        }
    }
}
//After 2 cards are choosen, wait 3 seconds (do not accept clicks), hide cards and change turn
//To do that, make a list that will add a card that will be exposed, when the list count is 2 (1), call hide cards and change turn