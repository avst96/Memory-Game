using MemoryGameSystem;
using System.Text.RegularExpressions;

namespace MemoryGameApp
{
    public partial class frmMemoryGame : Form
    {
        Game game = new();
        public frmMemoryGame()
        {
            InitializeComponent();
            btnStart.Click += BtnStart_Click;
            optSolo.CheckedChanged += OptSolo_CheckedChanged;
            game.PlayAgainstComputer = optSolo.Checked;
            lblMessage.DataBindings.Add("Text", game, "GameMessage");
            lblMessage.DataBindings.Add("ForeColor", game, "GameMessageColor");
            txtPlayer1Sets.DataBindings.Add("Text", game, "Player1Score");
            txtPlayer2Sets.DataBindings.Add("Text", game, "Player2Score");
            lblPlayerMode.DataBindings.Add("Text", game, "PlayerMode");
            lblPlayer2Sets.DataBindings.Add("Text", game, "Player2ScoreName");
            btnStart.DataBindings.Add("Text", game, "StartButtonText");
            optSolo.DataBindings.Add("Enabled", game, "DisableBtnDuringPlay");
            optTwoPlayer.DataBindings.Add("Enabled", game, "DisableBtnDuringPlay");
            foreach (Control c in tblMain.Controls)
            {
                if (c is Button btn)
                {
                    int index = ExtractIndexFromName(btn.Name);
                    Card current = game.Cards[index];
                    btn.Tag = index;
                    btn.Click += Card_Click;
                    btn.DataBindings.Add("ForeColor", current, "ForeColor");
                    btn.DataBindings.Add("Text", current, "CardPicture");
                    btn.DataBindings.Add("Visible", current, "IsVisible");
                    btn.DataBindings.Add("BackColor", current, "BackColor");
                }
            }
        }

        private void OptSolo_CheckedChanged(object? sender, EventArgs e)
        {
            game.PlayAgainstComputer = optSolo.Checked;
        }

        private void BtnStart_Click(object? sender, EventArgs e)
        {
            game.StartNewGame();
        }
        private void Card_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is int)
            {

                _ = game.PlayCard((int)btn.Tag);
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
    }
}
