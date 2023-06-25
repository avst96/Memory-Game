namespace MemoryGameApp
{
    public partial class frmMemoryGame : Form
    {
        List<Button> allcards;
        List<List<Button>> sets = new();
        List<Button> pickedcards = new();
        int score1 = 0, score2 = 0;
        Random rnd = new();
        enum GameStatusEnum { playing, finished, notstarted };
        GameStatusEnum gamestatus;
        enum TurnEnum { player1, player2 };
        TurnEnum currentturn;
        public frmMemoryGame()
        {
            InitializeComponent();
            allcards = new() { btnCard1, btnCard2, btnCard3, btnCard4, btnCard5, btnCard6, btnCard7, btnCard8, btnCard9, btnCard10, btnCard11, btnCard12, btnCard13, btnCard14, btnCard15, btnCard16, btnCard17, btnCard18, btnCard19, btnCard20 };
            allcards.ForEach(c => c.Click += Card_Click);
            btnStart.Click += BtnStart_Click;
        }
        private void StartNewGame()
        {
            //If pressed in middle playing will reset to not started
            gamestatus = gamestatus == GameStatusEnum.playing ? GameStatusEnum.notstarted : GameStatusEnum.playing;

            currentturn = TurnEnum.player1;
            score1 = 0; score2 = 0;
            allcards.ForEach(c =>
            {
                c.BackColor = Color.Orange;
                c.ForeColor = Color.Orange;
                c.Visible = true;
            });
            SetMessageAndBtns();
            ShuffleCards();
        }
        private void ExposeCard(Button btn)
        {
            if (gamestatus == GameStatusEnum.playing && pickedcards.Count() < 2 && !pickedcards.Contains(btn))
            {
                btn.BackColor = Color.LightGoldenrodYellow;
                btn.ForeColor = Color.Black;
                pickedcards.Add(btn);

                //Once 2 cards are picked, they following will proceed 
                if (pickedcards.Count() == 2)
                {
                    ThreeSecWait();

                    //To ensure that the New Game btn wasnt pressed during the wait
                    if (gamestatus == GameStatusEnum.playing)
                    {


                        //If a match
                        if (pickedcards[0].Text == pickedcards[1].Text)
                        {
                            pickedcards.ForEach(c => c.Visible = false);
                            switch (currentturn)
                            {
                                case TurnEnum.player1:
                                    score1++;
                                    break;
                                default:
                                    score2++;
                                    break;
                            }
                            pickedcards.Clear();

                            //If all cards finished
                            if (allcards.All(c => c.Visible == false))
                            {
                                gamestatus = GameStatusEnum.finished;
                                SetMessageAndBtns();
                            }
                        }
                    }

                    //Should always run to change turn
                    HideCard();

                }
            }
        }
        private void HideCard()
        {
            //Turns back over exposed cards, and then clears list of exposed cards and changes turn
            pickedcards.ForEach(card =>
            {
                card.BackColor = Color.Orange;
                card.ForeColor = Color.Orange;
            });
            pickedcards.Clear();
            currentturn = currentturn == TurnEnum.player1 ? TurnEnum.player2 : TurnEnum.player1;

            SetMessageAndBtns();
        }

        private void ThreeSecWait()
        {
            DateTime starttime = DateTime.Now;
            while ((DateTime.Now - starttime).TotalSeconds < 3)
            {
                Application.DoEvents();
            }
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
        private void SetMessageAndBtns()
        {
            string msg = "Press Start Game to start";
            string btnmessage = "Start Game";

            if (gamestatus == GameStatusEnum.playing)
            {
                msg = currentturn == TurnEnum.player1 ? "Player 1's Turn" : "Player 2's Turn";
                btnmessage = "New Game";
            }
            else if (gamestatus == GameStatusEnum.finished)
            {
                if (score1 == score2)
                {
                    msg = "Tie!";
                }
                else
                {
                    msg = score1 > score2 ? "Player 1 won!" : "Player 2 won!";
                }
            }
            lblMessage.Text = msg;
            btnStart.Text = btnmessage;
            txtPlayer1Sets.Text = score1.ToString();
            txtPlayer2Sets.Text = score2.ToString();
            optTwoPlayer.Enabled = gamestatus != GameStatusEnum.playing ? true : false;
            optSolo.Enabled = gamestatus != GameStatusEnum.playing ? true : false;
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
            StartNewGame();
        }
    }
}

