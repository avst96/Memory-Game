using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MemoryGameSystem
{
    /// <summary>
    /// A memory game 
    /// </summary>

    public class Game : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;
        public static event EventHandler? TotalScoreChanged;
        public enum GameStatusEnum { Playing, Finished, Notstarted };
        private List<List<Card>> sets = new();
        private List<Card> pickedcards = new();
        private enum TurnEnum { player1, player2 };
        private TurnEnum currentturn;
        private Random rnd = new();
        private Card? card1 = null;
        private Card? card2 = null;
        private List<string> allproperties = new() { "PlayerMode", "Player1Score", "Player2Score", "GameMessage", "GameMessageColor", "GameMessageColorMAUI", "StartButtonText", "Player2Name", "DisableBtnDuringPlay" };
        private int match1;
        private int match2;
        private string player2 = string.Empty;
        private static int numgame = 0, player1wins = 0, player2wins = 0, computerwins = 0, ties = 0;
        private bool _playagainstcomputer = false;

        public Game()
        {
            numgame++;
            GameNum = "Game " + numgame;
            for (int i = 0; i < 20; i++)
            {
                Cards.Add(new Card());
            }
        }
        /// <summary>
        /// Number of games started
        /// </summary>
        public string GameNum { get; private set; }
        public bool PlayAgainstComputer
        {
            get => _playagainstcomputer;
            set { _playagainstcomputer = value; InvokePropertyChanged(false, "PlayerMode"); InvokePropertyChanged(false, "Player2ScoreName"); }
        }
        /// <summary>
        /// Current status of game. Options of Notstarted, Playing and Finished
        /// </summary>
        public GameStatusEnum GameStatus { get; private set; } = GameStatusEnum.Notstarted;
        /// <summary>
        /// A list of all cards in game
        /// </summary>
        public List<Card> Cards { get; private set; } = new();
        public int Player1Score { get; private set; } = 0;
        public int Player2Score { get; private set; } = 0;
        public static string TotalScore { get => $"Player 1 won {player1wins} time(s) | Player 2 won {player2wins} time(s) {Environment.NewLine} Computer won {computerwins} time(s) | Game tied {ties} time(s)."; }
        public string GameMessage
        {
            get
            {
                string msg = "";
                switch (GameStatus)
                {

                    case GameStatusEnum.Notstarted:
                        msg = "Press Start Game to start";
                        break;
                    case GameStatusEnum.Playing:
                        msg = currentturn == TurnEnum.player1 ? "Player 1's Turn" : player2 + "'s Turn";
                        break;
                    case GameStatusEnum.Finished:
                        msg = Player1Score == Player2Score ? "Tie!" : Player1Score > Player2Score ? "Player 1 won!" : player2 + " won!";
                        break;
                }
                return msg;
            }
        }

        /// <summary>
        ///  Gets Game message color in system color, to get MAUI color use GameMessageColorMAUI
        /// </summary>
        public System.Drawing.Color GameMessageColor { get => GameStatus == GameStatusEnum.Notstarted ? System.Drawing.Color.Black : GameStatus == GameStatusEnum.Playing ? System.Drawing.Color.Green : System.Drawing.Color.MediumVioletRed; }
        public Microsoft.Maui.Graphics.Color GameMessageColorMAUI { get => ConvertToMauiColor(GameMessageColor); }
        public string StartButtonText { get => GameStatus == GameStatusEnum.Playing ? "New Game" : "Start Game"; }
        public string Player2ScoreName { get => PlayAgainstComputer ? "Computer's Sets" : "Player 2 Sets"; }
        public string PlayerMode { get => PlayAgainstComputer ? "Solo" : "2 Player"; }
        public bool DisableBtnDuringPlay { get => GameStatus == GameStatusEnum.Playing ? false : true; }


        public void StartNewGame()
        {
            player2 = PlayAgainstComputer == true ? "Computer" : "Player 2";

            if (GameStatus != GameStatusEnum.Playing)
            {
                ShuffleCards();
                card1 = null;
                card2 = null;
                currentturn = TurnEnum.player1;
            }
            if (GameStatus != GameStatusEnum.Notstarted)
            {
                Cards.ForEach(c => c.CardStatus = Card.CardStatusEnum.Facedown);
            }

            //If pressed in middle playing will reset to not started
            GameStatus = GameStatus == GameStatusEnum.Playing ? GameStatusEnum.Notstarted : GameStatusEnum.Playing;
            Player1Score = 0; Player2Score = 0;
            InvokePropertyChanged(true);
        }
        /// <summary>
        /// Play card that is passed in
        /// </summary>
        /// <param name="cardindex">A int representing the index (in Cards) of card chosen </param>
        public async Task PlayCard(int cardindex)
        {
            if (!PlayAgainstComputer || currentturn == TurnEnum.player1)
            { await DoMove(cardindex); }
        }

        private async Task DoMove(int cardindex)
        {
            Card selcard = Cards[cardindex];

            if (selcard.CardStatus == Card.CardStatusEnum.Facedown && Cards.Count(c =>
            c.CardStatus == Card.CardStatusEnum.Faceup) < 2 && GameStatus == GameStatusEnum.Playing)
            {
                selcard.CardStatus = Card.CardStatusEnum.Faceup;
                if (card1 is null) { card1 = selcard; } else { card2 = selcard; }
                if (!pickedcards.Contains(selcard)) { pickedcards.Add(selcard); }

                //Once 2 cards are picked, they following will proceed 
                if (Cards.Count(c => c.CardStatus == Card.CardStatusEnum.Faceup) == 2 && card2 != null)
                {

                    await TwoSecDelay();

                    //To ensure that the New Game btn wasnt pressed during the wait
                    if (GameStatus == GameStatusEnum.Playing)
                    {
                        //If a match
                        if (card1.CardPicture == card2.CardPicture)
                        {
                            //Hides cards
                            card1.CardStatus = Card.CardStatusEnum.Claimed;
                            card2.CardStatus = Card.CardStatusEnum.Claimed;
                            pickedcards.Remove(card1);
                            pickedcards.Remove(card2);

                            switch (currentturn)
                            {
                                case TurnEnum.player1:
                                    Player1Score++;
                                    InvokePropertyChanged(false, "Player1Score");
                                    break;
                                default:
                                    Player2Score++;
                                    InvokePropertyChanged(false, "Player2Score");
                                    break;
                            }

                            //If all cards finished
                            if (Cards.Count(c => c.CardStatus == Card.CardStatusEnum.Claimed) == 20)
                            {
                                GameStatus = GameStatusEnum.Finished;
                                if (Player1Score == Player2Score) { ties++; }
                                else if (Player1Score > Player2Score) { player1wins++; }
                                else if (PlayAgainstComputer) { computerwins++; }
                                else { player2wins++; }
                                TotalScoreChanged?.Invoke(this, new());
                                InvokePropertyChanged(true);
                            }
                        }
                        else
                        {
                            card1.CardStatus = Card.CardStatusEnum.Facedown;
                            card2.CardStatus = Card.CardStatusEnum.Facedown;
                        }
                        if (GameStatus == GameStatusEnum.Playing)
                        {
                            currentturn = currentturn == TurnEnum.player1 ? TurnEnum.player2 : TurnEnum.player1;
                            InvokePropertyChanged(false, "GameMessage");
                        }
                        card1 = null;
                        card2 = null;

                        if (PlayAgainstComputer && currentturn == TurnEnum.player2 && GameStatus == GameStatusEnum.Playing)
                        {
                            await TwoSecDelay();
                            await DoComputerMove();
                        }
                    }
                }
            }
        }

        private void ShuffleCards()
        {
            sets.Clear();
            char picture = 'I';
            List<Card> remaingcards = new();
            Cards.ForEach(remaingcards.Add);

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

        // Following is for computer turn
        private bool PickedCardsMatch()
        {
            int pickedcount = pickedcards.Count;

            //This checks for any matches in picked cards
            for (int i = 0; i < pickedcount; i++)
            {
                for (int j = i + 1; j < pickedcount; j++)
                {
                    if (pickedcards[i].CardPicture == pickedcards[j].CardPicture)
                    {
                        match1 = i;
                        match2 = j;
                        return true;
                    }
                }
            }
            return false;
        }

        private async Task DoComputerMove()
        {
            //if set was already uncovered then pick it
            if (PickedCardsMatch())
            {
                await DoMove(Cards.IndexOf(pickedcards[match1]));
                await TwoSecDelay();
                await DoMove(Cards.IndexOf(pickedcards[match2]));
            }

            //if set was not picked pick rnd card
            else
            {
                int playcard = PickRndCard();
                await DoMove(playcard);
                await TwoSecDelay();

                //check if matches if yes pick other card
                if (PickedCardsMatch())
                {
                    int match = Cards.FindIndex(c => c.CardPicture == Cards[playcard].CardPicture && c != Cards[playcard]);
                    await DoMove(match);
                }
                else
                {
                    await DoMove(PickRndCard());
                }
            }
        }
        private int PickRndCard()
        {
            List<Card> availablecards = new();
            availablecards = Cards.Where(c => c.CardStatus == Card.CardStatusEnum.Facedown).ToList();
            Card topick = availablecards[rnd.Next(availablecards.Count)];
            return Cards.IndexOf(topick);
        }
        private async Task TwoSecDelay()
        {
            await Task.Delay(2000);
        }
        private Microsoft.Maui.Graphics.Color ConvertToMauiColor(System.Drawing.Color systemColor)
        {
            float red = systemColor.R / 255f;
            float green = systemColor.G / 255f;
            float blue = systemColor.B / 255f;
            float alpha = systemColor.A / 255f;

            return new Microsoft.Maui.Graphics.Color(red, green, blue, alpha);
        }

        private void InvokePropertyChanged(bool All = false, [CallerMemberName] string propertyname = "")
        {
            if (All) { allproperties.ForEach(p => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p))); }
            else { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname)); }
        }
    }
}
