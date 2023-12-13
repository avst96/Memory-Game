namespace MemoryGameApp
{
    public partial class frmMemoryGame : Form
    {
        List<Button> allcards;   //
        List<List<Button>> sets = new(); //
        List<Button> faceupcards = new(); //
        List<Button> cards_won = new();  //
        List<Button> availablecards = new();
        int match1 = 0;
        int match2 = 0;
        string player2msg = "Player 2";

        int score1 = 0, score2 = 0;
        Random rnd = new();
        enum GameStatusEnum { playing, finished, notstarted };
        GameStatusEnum gamestatus = GameStatusEnum.notstarted;
        enum TurnEnum { player1, player2 };
        TurnEnum currentturn;
        public frmMemoryGame()
        {
            InitializeComponent();
            allcards = new() { btnCard1, btnCard2, btnCard3, btnCard4, btnCard5, btnCard6, btnCard7, btnCard8, btnCard9, btnCard10, btnCard11, btnCard12, btnCard13, btnCard14, btnCard15, btnCard16, btnCard17, btnCard18, btnCard19, btnCard20 };
            allcards.ForEach(c => c.Click += Card_Click);
            //btnStart.Click += BtnStart_Click;
            optSolo.CheckedChanged += Opt_CheckedChanged;
        }


      //  private void StartNewGame()
        //{
        //    //both following if's can run every time the function is called without effect, however it will then do the process for no reason
        //    //The first if runs if the game is not started or finished, the second if runs when playing or finished
        //    if (gamestatus != GameStatusEnum.playing)
        //    {
        //        ShuffleCards();
        //        currentturn = TurnEnum.player1;
        //    }
        //    if (gamestatus != GameStatusEnum.notstarted)
        //    {
        //        //To turn over all cards and put them in the deck
        //        allcards.ForEach(c =>
        //        {
        //            c.BackColor = Color.Orange;
        //            c.ForeColor = Color.Orange;
        //            c.Visible = true;
        //        });
        //    }

        //    //If pressed in middle playing will reset to not started
        //    gamestatus = gamestatus == GameStatusEnum.playing ? GameStatusEnum.notstarted : GameStatusEnum.playing;

        //    score1 = 0; score2 = 0;
        //    faceupcards.Clear();
        //    cards_won.Clear();

        //    SetMessageAndBtns();
        //}
        private void PlayCard(Button btn)
        {
            if (gamestatus == GameStatusEnum.playing && faceupcards.Count < 2 && btn.BackColor == Color.Orange)
            {
                btn.BackColor = Color.LightGoldenrodYellow;
                btn.ForeColor = Color.Black;
                faceupcards.Add(btn);

                //If btn is not yet in picked card list, it is added now
                if (!cards_won.Contains(btn))
                {
                    cards_won.Add(btn);
                }


                //Once 2 cards are picked, they following will proceed 
                if (faceupcards.Count == 2)
                {
                    TwoSecDelay();

                    //To ensure that the New Game btn wasnt pressed during the wait
                    if (gamestatus == GameStatusEnum.playing)
                    {

                        //If a match
                        if (faceupcards[0].Text == faceupcards[1].Text)
                        {
                            //Hides cards and removes them from picked list
                            faceupcards.ForEach(c =>
                            {
                                c.Visible = false;
                                cards_won.Remove(c);
                            });

                            switch (currentturn)
                            {
                                case TurnEnum.player1:
                                    score1++;
                                    break;
                                default:
                                    score2++;
                                    break;
                            }
                            faceupcards.Clear();

                            //If all cards finished
                            if (allcards.All(c => c.Visible == false))
                            {
                                gamestatus = GameStatusEnum.finished;
                                SetMessageAndBtns();
                            }
                        }

                        HideCard();
                        currentturn = currentturn == TurnEnum.player1 ? TurnEnum.player2 : TurnEnum.player1;
                        SetMessageAndBtns();

                        if (optSolo.Checked && currentturn == TurnEnum.player2 && gamestatus == GameStatusEnum.playing)
                        {
                            TwoSecDelay();
                            DoComputerMove();
                        }
                    }
                }
            }
        }
        private static void TwoSecDelay()
        {
            DateTime starttime = DateTime.Now;
            while ((DateTime.Now - starttime).TotalSeconds < 2)
            {
                Application.DoEvents();
            }
        }

        private void HideCard()
        {
            //Turns back over exposed cards, and then clears list of exposed cards and changes turn
            faceupcards.ForEach(card =>
            {
                card.BackColor = Color.Orange;
                card.ForeColor = Color.Orange;
            });
            faceupcards.Clear();
        }
        private bool PickedCardsMatch()
        {
            int pickedcount = cards_won.Count;

            //This checks for any matches in picked cards
            for (int i = 0; i < pickedcount; i++)
            {
                for (int j = i + 1; j < pickedcount; j++)
                {
                    match1 = i;
                    match2 = j;
                    if (cards_won[match1].Text == cards_won[match2].Text && match1 != match2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        private void DoComputerMove()
        {
            //if set was already uncovered then pick it
            if (PickedCardsMatch())
            {
                PlayCard(cards_won[match1]);
                TwoSecDelay();
                PlayCard(cards_won[match2]);
            }

            //if set was not picked pick rnd card
            else
            {
                PlayCard(PickRndCard());
                TwoSecDelay();

                //check if matches if yes pick other card
                if (PickedCardsMatch())
                {
                    Button btn = cards_won.First(c => c.Text == faceupcards[0].Text && c != faceupcards[0]);
                    PlayCard(btn);
                }
                else
                {
                    PlayCard(PickRndCard());
                }
            }
        }
        private Button PickRndCard()
        {
            Button btn = new();
            availablecards.Clear();
            availablecards = allcards.Where(c => c.Visible == true && c.BackColor == Color.Orange).ToList();
            btn = availablecards[rnd.Next(availablecards.Count)];
            return btn;
        }
        private void ShuffleCards()
        {
            sets.Clear();
            char picture = 'I';
            List<Button> remaingcards = new();
            allcards.ForEach(b => remaingcards.Add(b));

            //In this for loop it will add sets of two cards to the sets list for all cards
            while (remaingcards.Count > 1)
            {
                int card_one = rnd.Next(remaingcards.Count);
                int card_two = rnd.Next(remaingcards.Count);

                //while loop to ensure that card one and two are not the same
                while (card_one == card_two)
                {
                    card_two = rnd.Next(remaingcards.Count);
                }
                sets.Add(new() { remaingcards[card_one], remaingcards[card_two] });

                //The following removes the cards that are already in the set list
                remaingcards.RemoveAt(card_one);
                //To make sure it removes card two, as the index can change if card one is before card two
                if (card_one < card_two)
                {
                    card_two--;
                }
                remaingcards.RemoveAt(card_two);
            }

            //Will add different picture to each set
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
            Color c = Color.Black;

            if (gamestatus == GameStatusEnum.playing)
            {
                msg = currentturn == TurnEnum.player1 ? "Player 1's Turn" : player2msg + "'s Turn";
                btnmessage = "New Game";
                c = Color.Green;
            }
            else if (gamestatus == GameStatusEnum.finished)
            {
                if (score1 == score2)
                {
                    msg = "Tie!";
                }
                else
                {
                    msg = score1 > score2 ? "Player 1 won!" : player2msg + " won!";
                }
                c = Color.MediumVioletRed;
            }


            lblMessage.ForeColor = c;
            lblMessage.Text = msg;
            btnStart.Text = btnmessage;
            txtPlayer1Sets.Text = score1.ToString();
            txtPlayer2Sets.Text = score2.ToString();
            optTwoPlayer.Enabled = gamestatus != GameStatusEnum.playing ? true : false;
            optSolo.Enabled = gamestatus != GameStatusEnum.playing ? true : false;
        }

        //Event handlers
        //private void BtnStart_Click(object? sender, EventArgs e)
        //{
        //    StartNewGame();
        //}
        private void Card_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && lblMessage.Text != "Computer's Turn")
            {
                PlayCard(btn);
            }
        }
        private void Opt_CheckedChanged(object? sender, EventArgs e)
        {
            player2msg = optSolo.Checked ? "Computer" : "Player 2";
            lblPlayerMode.Text = optSolo.Checked ? "Solo" : "2 Player";
            lblPlayer2Sets.Text = player2msg + " Sets";
        }
    }
}
