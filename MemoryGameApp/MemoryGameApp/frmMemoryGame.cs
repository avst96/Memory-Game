using MemoryGameSystem;
using System.Text.RegularExpressions;

namespace MemoryGameApp
{
    public partial class frmMemoryGame : Form
    {
        MemoryGame game = new();
        public frmMemoryGame()
        {
            InitializeComponent();
            btnStart.Click += BtnStart_Click;
            lblMessage.DataBindings.Add("Text", game, "GameMessage");
            foreach (Control c in tblMain.Controls)
            {
                if (c is Button btn)
                {
                    btn.Click += Card_Click;
                    // btn.DataBindings.Add()
                }
            }
        }



        // FOLLOWING IS FOR COMPUTER TURN
        //private bool PickedCardsMatch()
        //{
        //    int pickedcount = cardspicked.Count;

        //    //This checks for any matches in picked cards
        //    for (int i = 0; i < pickedcount; i++)
        //    {
        //        for (int j = i + 1; j < pickedcount; j++)
        //        {
        //            match1 = i;
        //            match2 = j;
        //            if (cardspicked[match1].Text == cardspicked[match2].Text && match1 != match2)
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}

        //private void DoComputerMove()
        //{
        //    //if set was already uncovered then pick it
        //    if (PickedCardsMatch())
        //    {
        //        PlayCard(cardspicked[match1]);
        //        TwoSecDelay();
        //        PlayCard(cardspicked[match2]);
        //    }

        //    //if set was not picked pick rnd card
        //    else
        //    {
        //        PlayCard(PickRndCard());
        //        TwoSecDelay();

        //        //check if matches if yes pick other card
        //        if (PickedCardsMatch())
        //        {
        //            Button btn = cardspicked.First(c => c.Text == faceupcards[0].Text && c != faceupcards[0]);
        //            PlayCard(btn);
        //        }
        //        else
        //        {
        //            PlayCard(PickRndCard());
        //        }
        //    }
        //}
        //private Button PickRndCard()
        //{
        //    Button btn = new();
        //    availablecards.Clear();
        //    availablecards = allcards.Where(c => c.Visible == true && c.BackColor == Color.Orange).ToList();
        //    btn = availablecards[rnd.Next(availablecards.Count)];
        //    return btn;
        //}




        //private void SetMessageAndBtns()
        //{
        //    string msg = "Press Start Game to start";
        //    string btnmessage = "Start Game";
        //    Color c = Color.Black;

        //    if (gamestatus == GameStatusEnum.playing)
        //    {
        //        msg = currentturn == TurnEnum.player1 ? "Player 1's Turn" : player2msg + "'s Turn";
        //        btnmessage = "New Game";
        //        c = Color.Green;
        //    }
        //    else if (gamestatus == GameStatusEnum.finished)
        //    {
        //        if (score1 == score2)
        //        {
        //            msg = "Tie!";
        //        }
        //        else
        //        {
        //            msg = score1 > score2 ? "Player 1 won!" : player2msg + " won!";
        //        }
        //        c = Color.MediumVioletRed;
        //    }


        //    lblMessage.ForeColor = c;
        //    lblMessage.Text = msg;
        //    btnStart.Text = btnmessage;
        //    txtPlayer1Sets.Text = score1.ToString();
        //    txtPlayer2Sets.Text = score2.ToString();
        //    optTwoPlayer.Enabled = gamestatus != GameStatusEnum.playing ? true : false;
        //    optSolo.Enabled = gamestatus != GameStatusEnum.playing ? true : false;
        //}


        private void BtnStart_Click(object? sender, EventArgs e)
        {
            game.StartNewGame(optSolo.Checked);
        }
        private void Card_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                int i = ExtractNumFromName(btn.Name);
                game.PlayCard(i);
            }
        }

        private int ExtractNumFromName(string btnname)
        {
            int i = -1;
            Match num = Regex.Match(btnname, @"\d+");
            if (num.Success)
            {
                i = int.Parse(num.Value);
                i--;
            }
            return i;
        }
    }
}
