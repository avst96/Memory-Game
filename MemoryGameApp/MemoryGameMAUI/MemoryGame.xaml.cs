namespace MemoryGameMAUI;

public partial class MemoryGame : ContentPage
{
    MemoryGameSystem.MemoryGame game = new();
    List<Button> allbtns = new();

    public MemoryGame()
    {
        InitializeComponent();
        game.PlayAgainstComputer = SoloOpt.IsChecked;
        BindingContext = game;
        foreach (View v in MainGrid.Children)
        {
            if (v is Button b)
            {
                allbtns.Add(b);
            }
        }
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
            _ = game.PlayCard(allbtns.IndexOf((Button)sender));
        }
    }
}
