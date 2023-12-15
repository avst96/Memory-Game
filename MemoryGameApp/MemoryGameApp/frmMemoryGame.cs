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
            lblMessage.DataBindings.Add("ForeColor", game, "GameMessageColor");
            txtPlayer1Sets.DataBindings.Add("Text", game, "Player1Score");
            txtPlayer2Sets.DataBindings.Add("Text", game, "Player2Score");
            lblPlayerMode.DataBindings.Add("Text", game, "Player2Name");
            btnStart.DataBindings.Add("Text", game, "StartButtonText");
            optSolo.DataBindings.Add("Enabled", game, "DisableBtnDuringPlay");
            optTwoPlayer.DataBindings.Add("Enabled", game, "DisableBtnDuringPlay");
            foreach (Control c in tblMain.Controls)
            {
                if (c is Button btn)
                {
                    Card current = game.Cards[ExtractIndexFromName(btn.Name)];
                    btn.Click += Card_Click;
                    btn.DataBindings.Add("ForeColor", current, "ForeColor");
                    btn.DataBindings.Add("Text", current, "CardPicture");
                    btn.DataBindings.Add("Visible", current, "IsVisible");
                    btn.DataBindings.Add("BackColor", current, "BackColor");
                }
            }
        }


        private void BtnStart_Click(object? sender, EventArgs e)
        {
            game.StartNewGame(optSolo.Checked);
        }
        private void Card_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                int i = ExtractIndexFromName(btn.Name);
                _ = game.PlayCard(i);
            }
        }

        private int ExtractIndexFromName(string btnname)
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

    }
}
