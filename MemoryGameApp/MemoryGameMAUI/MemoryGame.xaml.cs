using MemoryGameSystem;

namespace MemoryGameMAUI;

public partial class MemoryGame : ContentPage
{
    List<Game> listgames = new() { new Game(), new Game(), new Game() };
    Game activegame;

    List<Button> allbtns = new();

    public MemoryGame()
    {
        InitializeComponent();
        activegame = listgames[0];
        BindingContext = activegame;
        Game1Rb.BindingContext = listgames[0];
        Game2Rb.BindingContext = listgames[1];
        Game3Rb.BindingContext = listgames[2];

        foreach (View v in MainGrid.Children)
        {
            if (v is Button b)
            {
                allbtns.Add(b);
            }
        }
        Loaded += MemoryGame_Loaded;
    }

    private void MemoryGame_Loaded(object sender, EventArgs e)
    {
        Game1Rb.IsChecked = true;
    }

    private void StartBtn_Clicked(object sender, EventArgs e)
    {
        activegame.StartNewGame();
    }


    private void CardBtn_Clicked(object sender, EventArgs e)
    {
        if (sender is Button b)
        {
            _ = activegame.PlayCard(allbtns.IndexOf(b));
        }
    }

    private void GameRB_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is RadioButton rb)
        {
            if (rb.BindingContext != null && rb.IsChecked)
            {
                BindingContext = rb.BindingContext as Game;
                activegame = rb.BindingContext as Game;
            }
        }
    }

}
