using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace MemoryGameSystem
{
    public class MemoryGame : INotifyPropertyChanged
    {
        public enum GameStatusEnum { playing, finished, notstarted };
        List<List<Card>> sets = new();
        enum TurnEnum { player1, player2 };
        private TurnEnum currentturn;
        private bool playagainstcomputer;
        private Random rnd = new();
        private Card? card1 = null;
        private Card? card2 = null;
        public event PropertyChangedEventHandler? PropertyChanged;
        private List<string> allproperties = new() { "Player1Score", "Player2Score", "GameMessage", "GameMessageColor", "StartButtonText", "Player2Name", "DisableBtnDuringPlay" };

        public MemoryGame()
        {
            for (int i = 0; i < 20; i++)
            {
                Cards.Add(new Card());
            }
        }
        // ! Remember to invoke property changes somewhere for 1) Startbtntext 2) Disablebtn 3) GameColor 4) GameMsg 5)6) scores
        public GameStatusEnum gamestatus { get; private set; } = GameStatusEnum.notstarted;
        public List<Card> Cards { get; private set; } = new();
        public int Player1Score { get; private set; } = 0;
        public int Player2Score { get; private set; } = 0;
        public string GameMessage
        {
            get
            {
                string msg = "";
                switch (gamestatus)
                {

                    case GameStatusEnum.notstarted:
                        msg = "Press Start Game to start";
                        break;
                    case GameStatusEnum.playing:
                        msg = currentturn == TurnEnum.player1 ? "Player 1's Turn" : Player2Name + "'s Turn";
                        break;
                    case GameStatusEnum.finished:
                        if (Player1Score == Player2Score)
                        {
                            msg = "Tie!";
                        }
                        else
                        {
                            msg = Player1Score > Player2Score ? "Player 1 won!" : Player2Name + " won!";
                        }
                        break;
                }
                return msg;
            }
        }
        public Color GameMessageColor { get => gamestatus == GameStatusEnum.notstarted ? Color.Black : gamestatus == GameStatusEnum.playing ? Color.Green : Color.MediumVioletRed; }
        public string StartButtonText { get => gamestatus == GameStatusEnum.playing ? "New Game" : "Start Game"; }
        public string Player2Name { get => playagainstcomputer ? "Computer" : "2 Player"; }
        public bool DisableBtnDuringPlay { get => gamestatus == GameStatusEnum.playing ? false : true; }


        public void StartNewGame(bool solo = false)
        {
            playagainstcomputer = solo;
            if (gamestatus != GameStatusEnum.playing)
            {
                ShuffleCards();
                card1 = null;
                card2 = null;
                currentturn = TurnEnum.player1;
            }
            if (gamestatus != GameStatusEnum.notstarted)
            {
                Cards.ForEach(c => c.CardStatus = Card.CardStatusEnum.Facedown);
            }

            //If pressed in middle playing will reset to not started
            gamestatus = gamestatus == GameStatusEnum.playing ? GameStatusEnum.notstarted : GameStatusEnum.playing;
            Player1Score = 0; Player2Score = 0;
            
            InvokeAllPropertyChanged();
        }
        public async Task PlayCard(int cardindex)
        {
            if (!playagainstcomputer || currentturn == TurnEnum.player1)
            { await DoMove(cardindex); }
        }

        private async Task DoMove(int cardindex)
        {
            Card selcard = Cards[cardindex];

            if (selcard.CardStatus == Card.CardStatusEnum.Facedown && Cards.Count(c => c.CardStatus == Card.CardStatusEnum.Faceup) < 2 && gamestatus == GameStatusEnum.playing)
            {
                selcard.CardStatus = Card.CardStatusEnum.Faceup;
                if (card1 is null) { card1 = selcard; } else { card2 = selcard; }

                //Once 2 cards are picked, they following will proceed 
                if (Cards.Count(c => c.CardStatus == Card.CardStatusEnum.Faceup) == 2 && card2 != null)
                {

                    await TwoSecDelay();

                    //To ensure that the New Game btn wasnt pressed during the wait
                    if (gamestatus == GameStatusEnum.playing)
                    {
                        //If a match
                        if (card1.CardPicture == card2.CardPicture)
                        {
                            //Hides cards
                            card1.CardStatus = Card.CardStatusEnum.Claimed;
                            card2.CardStatus = Card.CardStatusEnum.Claimed;


                            switch (currentturn)
                            {
                                case TurnEnum.player1:
                                    Player1Score++;
                                    InvokePropertyChanged("Player1Score");
                                    break;
                                default:
                                    Player2Score++;
                                    InvokePropertyChanged("Player2Score");
                                    break;
                            }

                            //If all cards finished
                            if (Cards.Count(c => c.CardStatus == Card.CardStatusEnum.Claimed) == 20)
                            {
                                gamestatus = GameStatusEnum.finished;
                                InvokeAllPropertyChanged();
                            }
                        }
                        else
                        {
                            card1.CardStatus = Card.CardStatusEnum.Facedown;
                            card2.CardStatus = Card.CardStatusEnum.Facedown;
                        }

                        currentturn = currentturn == TurnEnum.player1 ? TurnEnum.player2 : TurnEnum.player1;
                        InvokePropertyChanged("GameMessage");

                        //if (playagainstcomputer && currentturn == TurnEnum.player2 && gamestatus == GameStatusEnum.playing)
                        //{
                        //    await TwoSecDelay();
                        //    //DoComputerMove();
                        //}
                    }
                    card1 = null;
                    card2 = null;
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

        private async Task TwoSecDelay()
        {
            await Task.Delay(2000);
        }
        private void InvokeAllPropertyChanged()
        {
            allproperties.ForEach(p => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p)));
        }
        private void InvokePropertyChanged([CallerMemberName] string propertyname = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
