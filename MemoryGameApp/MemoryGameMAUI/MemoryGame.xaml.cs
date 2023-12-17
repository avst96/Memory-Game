using MemoryGameSystem;

namespace MemoryGameMAUI;

public partial class MemoryGame : ContentPage
{
	MemoryGameSystem.MemoryGame game = new();

	public MemoryGame()
	{
        InitializeComponent();
		BindingContext = game;
	}
}