namespace MemoryGameSystem
{
    internal class MemoryGame
    {
        List<Card> allcards = new();
        public MemoryGame() 
        { 
            for(int i = 0; i < 20; i++)
            {
                allcards.Add(new Card());
            }
        }
    }
}
