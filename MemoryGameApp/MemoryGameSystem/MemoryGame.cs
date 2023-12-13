namespace MemoryGameSystem
{
    public class MemoryGame
    {
        public enum GameStatusEnum { playing, finished, notstarted };
        Random rnd = new();
        List<List<Card>> sets = new();
       
        enum TurnEnum { player1, player2 };
        TurnEnum currentturn;
        public MemoryGame()
        {
            for (int i = 0; i < 20; i++)
            {
                Cards.Add(new Card());
            }
        }
        public GameStatusEnum gamestatus { get; private set; } = GameStatusEnum.notstarted;
        public List<Card> Cards { get; private set; } = new();
        public int Player1Score { get; private set; } = 0;
        public int Player2Score { get; private set; } = 0;

        public void StartNewGame()
        {
            //both following if's can run every time the function is called without effect, however it will then do the process for no reason
            //The first if runs if the game is not started or finished, the second if runs when playing or finished
            //If pressed in middle playing will be set to notstarted and will shuffle when pressed again to start the game
            if (gamestatus != GameStatusEnum.playing)
            {
                ShuffleCards();
                currentturn = TurnEnum.player1;
            }
            if (gamestatus != GameStatusEnum.notstarted)
            {
                //To turn over all cards and put them in the deck
                Cards.ForEach(c => c.CardStatus = Card.CardStatusEnum.Facedown);
            }

            //If pressed in middle playing will reset to not started
            gamestatus = gamestatus == GameStatusEnum.playing ? GameStatusEnum.notstarted : GameStatusEnum.playing;

            Player1Score = 0; Player2Score = 0;
        }
        
        private void ShuffleCards()
        {
            sets.Clear();
            char picture = 'I';
            List<Card> remaingcards = new();
            Cards.ForEach(c => remaingcards.Add(c));

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
                s.ForEach(c => c.CardPicture = picture);
                picture++;
            });
        }
    }
}
