namespace MemoryGameApp
{
    public partial class frmMemoryGame : Form
    {
        List<Button> allcards;
        List<List<Button>> sets = new();
        List<Button> activecards = new();
        List<Button> pickedcards = new();
        List<Button> availablecards = new();
        int match1 = 0;
        int match2 = 0;

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
            btnStart.Click += BtnStart_Click;
            optSolo.CheckedChanged += Opt_CheckedChanged;
        }


        private void StartNewGame()
        {
            //both following if's can run every time the function is called without effect, however it will then do the process for no reason
            if (gamestatus != GameStatusEnum.playing)
            {
                ShuffleCards();
                currentturn = TurnEnum.player1;
            }
            if (gamestatus != GameStatusEnum.notstarted)
            {
                //To turn over all cards and put them in the deck
                allcards.ForEach(c =>
                {
                    c.BackColor = Color.Orange;
                    c.ForeColor = Color.Orange;
                    c.Visible = true;
                });
            }

            //If pressed in middle playing will reset to not started
            gamestatus = gamestatus == GameStatusEnum.playing ? GameStatusEnum.notstarted : GameStatusEnum.playing;

            score1 = 0; score2 = 0;

            SetMessageAndBtns();
        }
        private void PlayCard(Button btn)
        {
            if (gamestatus == GameStatusEnum.playing && activecards.Count() < 2 && !activecards.Contains(btn))
            {
                btn.BackColor = Color.LightGoldenrodYellow;
                btn.ForeColor = Color.Black;
                activecards.Add(btn);
                pickedcards.Add(btn);


                //Once 2 cards are picked, they following will proceed 
                if (activecards.Count() == 2)
                {
                    TwoSecDelay();

                    //To ensure that the New Game btn wasnt pressed during the wait
                    if (gamestatus == GameStatusEnum.playing)
                    {

                        //If a match
                        if (activecards[0].Text == activecards[1].Text)
                        {
                            activecards.ForEach(c => c.Visible = false);
                            switch (currentturn)
                            {
                                case TurnEnum.player1:
                                    score1++;
                                    break;
                                default:
                                    score2++;
                                    break;
                            }
                            activecards.Clear();

                            //If all cards finished
                            if (allcards.All(c => c.Visible == false))
                            {
                                gamestatus = GameStatusEnum.finished;
                                SetMessageAndBtns();
                            }
                        }

                        //Should always run to change turn
                        HideCard();

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
            activecards.ForEach(card =>
            {
                card.BackColor = Color.Orange;
                card.ForeColor = Color.Orange;
            });
            activecards.Clear();
            currentturn = currentturn == TurnEnum.player1 ? TurnEnum.player2 : TurnEnum.player1;

            SetMessageAndBtns();
        }
        private bool PickedCardsMatch()
        {
            //This for should remove sets that where won from the pickedcards list
            for (int i = pickedcards.Count - 1; i >= 0; i--)
            {
                Button btn = pickedcards[i];

                if (btn.Visible == false)
                {
                    pickedcards.Remove(btn);
                }
            }



            int pickedcount = pickedcards.Count();

            for (int i = 0; i < pickedcount - 1; i++)
            {
                for (int j = i + 1; j < pickedcount - 1; j++)
                {
                    match1 = i;
                    match2 = j;
                    if (pickedcards[match1].Text == pickedcards[match2].Text)
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
                PlayCard(pickedcards[match1]);
                TwoSecDelay();
                if (gamestatus == GameStatusEnum.playing)
                {
                    PlayCard(pickedcards[match2]);
                }
            }

            //if set was not picked pick rnd card
            else
            {
                availablecards.Clear();
                availablecards = allcards.Where(c => c.Visible == true).ToList();
                PlayCard(availablecards[rnd.Next(availablecards.Count)]);
                TwoSecDelay();

                //check if matches if yes pick other card
                if (PickedCardsMatch() && gamestatus == GameStatusEnum.playing)
                {
                    Button btn = pickedcards.FirstOrDefault(c => c.Text == activecards[0].Text);
                    PlayCard(btn);

                }

                //!TODO if not pick another rnd card


            }


        }
        private void ShuffleCards()
        {
            sets.Clear();
            char picture = 'I';
            List<Button> remaingcards = new();
            allcards.ForEach(b => remaingcards.Add(b));

            //In this for loop it will add sets of two cards to the sets list for all cards
            while (remaingcards.Count() > 1)
            {
                int card_one = rnd.Next(remaingcards.Count());
                int card_two = rnd.Next(remaingcards.Count());

                //while loop to ensure that card one and two are not the same
                while (card_one == card_two)
                {
                    card_two = rnd.Next(remaingcards.Count());
                }
                sets.Add(new() { remaingcards[card_one], remaingcards[card_two] });

                //The following removes the cards that are already in the set list
                remaingcards.RemoveAt(card_one);
                //To make sure it doesn't attempt to remove an index that is out of bound because card 1 was already removed
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
                if (optSolo.Checked)
                {
                    msg = currentturn == TurnEnum.player1 ? "Player 1's Turn" : "Computer's Turn";
                }
                else
                {
                    msg = currentturn == TurnEnum.player1 ? "Player 1's Turn" : "Player 2's Turn";
                }
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
                    msg = score1 > score2 ? "Player 1 won!" : "Player 2 won!";
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
        private void BtnStart_Click(object? sender, EventArgs e)
        {
            StartNewGame();
        }
        private void Card_Click(object? sender, EventArgs e)
        {
            //Add in to the if statement  && lblMessage.Text != "Computer's Turn"
            if (sender is Button btn)
            {
                PlayCard(btn);
            }
        }
        private void Opt_CheckedChanged(object? sender, EventArgs e)
        {
            lblPlayerMode.Text = optSolo.Checked ? "Solo" : "2 Player";
        }
    }
}

