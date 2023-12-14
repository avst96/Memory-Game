using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using static System.Formats.Asn1.AsnWriter;

namespace MemoryGameSystem
{
    public class MemoryGame : INotifyPropertyChanged
    {
        public enum GameStatusEnum { playing, finished, notstarted };
        List<List<Card>> sets = new();
        enum TurnEnum { player1, player2 };
        private TurnEnum currentturn;
        private bool playagainstcomputer;
        Random rnd = new();
        Card? card1 = null;
        Card? card2 = null;
        private string _gamemessage = "Press Start Game to start";
        public event PropertyChangedEventHandler? PropertyChanged;


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
        public string GameMessage {
            get => _gamemessage;
            private set { _gamemessage = value; InvokePropertyChanged(); } }
        public Color GameMessageColor { get; private set; }
        public string StartButtonText { get; private set; } = string.Empty;
        public string Player2Name { get; private set; } = "Player 2";
        public bool DisableBtnDuringPlay { get; private set; }


        public void StartNewGame(bool solo = false)
        {
            playagainstcomputer = solo;
            Player2Name = playagainstcomputer == true ? "Computer" : "Player 2";
            SetMessageAndBtns();
            if (gamestatus != GameStatusEnum.playing)
            {
                ShuffleCards();
                currentturn = TurnEnum.player1;
            }
            if (gamestatus != GameStatusEnum.notstarted)
            {
                Cards.ForEach(c => c.CardStatus = Card.CardStatusEnum.Facedown);
            }

            //If pressed in middle playing will reset to not started
            gamestatus = gamestatus == GameStatusEnum.playing ? GameStatusEnum.notstarted : GameStatusEnum.playing;

            Player1Score = 0; Player2Score = 0;
        }
        public void PlayCard(int cardindex)
        {
            if(!playagainstcomputer || currentturn  == TurnEnum.player1) 
            {  DoMove(cardindex); }
        }

        private async Task DoMove(int cardindex)
        {
            Card selcard = Cards[cardindex];
            // !! FIGURE OUT WHY COUNT IS 20 AND GAMESTATUS NOT STARTED
            if (selcard.CardStatus == Card.CardStatusEnum.Facedown && Cards.Count(c => c.CardStatus == Card.CardStatusEnum.Faceup) < 2 && gamestatus == GameStatusEnum.playing)
            {
                selcard.CardStatus = Card.CardStatusEnum.Faceup;
                if (card1 is null) { card1 = selcard; } else { card2 = selcard; }

                //Once 2 cards are picked, they following will proceed 
                if (Cards.Count(c => c.CardStatus == Card.CardStatusEnum.Faceup) == 2)
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
                                    break;
                                default:
                                    Player2Score++;
                                    break;
                            }

                            //If all cards finished
                            if (Cards.Count(c => c.CardStatus == Card.CardStatusEnum.Claimed) == 20)
                            {
                                gamestatus = GameStatusEnum.finished;
                                SetMessageAndBtns();
                            }
                        }
                        else
                        {
                            card1.CardStatus = Card.CardStatusEnum.Facedown;
                            card2.CardStatus = Card.CardStatusEnum.Facedown;
                        }

                        currentturn = currentturn == TurnEnum.player1 ? TurnEnum.player2 : TurnEnum.player1;
                        SetMessageAndBtns();

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

        private void SetMessageAndBtns()
        {
            string msg = "Press Start Game to start";
            string btnmsg = "Start Game";
            Color c = Color.Black;

            if (gamestatus == GameStatusEnum.playing)
            {
                msg = currentturn == TurnEnum.player1 ? "Player 1's Turn" : Player2Name + "'s Turn";
                btnmsg = "New Game";
                c = Color.Green;
            }
            else if (gamestatus == GameStatusEnum.finished)
            {
                if (Player1Score == Player2Score)
                {
                    msg = "Tie!";
                }
                else
                {
                    msg = Player1Score > Player2Score ? "Player 1 won!" : Player2Name + " won!";
                }
                c = Color.MediumVioletRed;
            }


            GameMessageColor = c;
            GameMessage = msg;
            StartButtonText = btnmsg;
            DisableBtnDuringPlay = gamestatus == GameStatusEnum.playing ? false : true;
        }

        private async Task TwoSecDelay()
        {
            await Task.Delay(2000);
        }
        private void InvokePropertyChanged([CallerMemberName] string propertyname = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
