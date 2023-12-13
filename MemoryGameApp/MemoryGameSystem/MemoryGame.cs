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
        TurnEnum currentturn;
        private bool playagainstcomputer;
        Random rnd = new();

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
        public string GameMessage { get; private set; }
        public string StartButtonText { get; private set; }
        public Color GameMessageColor { get; private set; }
        public string Player2Name { get; private set; } = "Player 2";

        private void SetMessageAndBtns()
        {
            GameMessage = "Press Start Game to start";
            StartButtonText = "Start Game";
            GameMessageColor = Color.Black;

            if (gamestatus == GameStatusEnum.playing)
            {
                GameMessage = currentturn == TurnEnum.player1 ? "Player 1's Turn" : Player2Name + "'s Turn";
                StartButtonText = "New Game";
                GameMessageColor = Color.Green;
            }
            else if (gamestatus == GameStatusEnum.finished)
            {
                if (Player1Score == Player2Score)
                {
                    GameMessage = "Tie!";
                }
                else
                {
                    GameMessage = Player1Score > Player2Score ? "Player 1 won!" : Player2Name + " won!";
                }
                GameMessageColor = Color.MediumVioletRed;
            }

            
            lblMessage.ForeColor = c;
            lblMessage.Text = msg;
            btnStart.Text = btnmessage;
            txtPlayer1Sets.Text = score1.ToString();
            txtPlayer2Sets.Text = score2.ToString();
            optTwoPlayer.Enabled = gamestatus != GameStatusEnum.playing ? true : false;
            optSolo.Enabled = gamestatus != GameStatusEnum.playing ? true : false;
        }
        public void StartNewGame(bool playagainstcomp = false)
        {
            playagainstcomputer = playagainstcomp;
            Player2Name = playagainstcomputer == true ? "Computer" : "Player 2";
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

        public async Task PlayCard(int cardindex)
        {
            Card selcard = Cards[cardindex];
            if (selcard.CardStatus == Card.CardStatusEnum.Facedown && Cards.Count(c => c.CardStatus == Card.CardStatusEnum.Faceup) < 2 && gamestatus == GameStatusEnum.playing)
            {
                selcard.CardStatus = Card.CardStatusEnum.Faceup;

                ////If btn is not yet in picked card list, it is added now
                //if (!cardspicked.Contains(btn))
                //{
                //    cardspicked.Add(btn);
                //}


                //Once 2 cards are picked, they following will proceed 
                if (Cards.Count(c => c.CardStatus == Card.CardStatusEnum.Faceup) == 2)
                {
                    Card? card1 = Cards.FirstOrDefault(c => c.CardStatus == Card.CardStatusEnum.Faceup);
                    Card? card2 = Cards.LastOrDefault(c => c.CardStatus == Card.CardStatusEnum.Faceup);
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

                        if (playagainstcomputer && currentturn == TurnEnum.player2 && gamestatus == GameStatusEnum.playing)
                        {
                            await TwoSecDelay();
                            //DoComputerMove();
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
