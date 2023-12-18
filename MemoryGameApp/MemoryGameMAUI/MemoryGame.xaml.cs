using System.Text.RegularExpressions;

namespace MemoryGameMAUI;

public partial class MemoryGame : ContentPage
{
    MemoryGameSystem.MemoryGame game = new();

    public MemoryGame()
    {
        InitializeComponent();
        game.PlayAgainstComputer = SoloOpt.IsChecked;
        BindingContext = game;
    }

    private void StartBtn_Clicked(object sender, EventArgs e)
    {
        game.StartNewGame();

    }

    private void SoloOpt_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        game.PlayAgainstComputer = SoloOpt.IsChecked;
    }

    private void CardBtn_Clicked(object sender, EventArgs e)
    {
        if (sender is Button b)
        {
            // !! Change to loop in begining of game?
            _ = game.PlayCard(ExtractIndexFromName(b.StyleId));
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
