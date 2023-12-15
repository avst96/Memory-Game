﻿using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace MemoryGameSystem
{
    public class MemoryGame : INotifyPropertyChanged
    {
        public enum GameStatusEnum { playing, finished, notstarted };
        private List<List<Card>> sets = new();
        private List<Card> pickedcards = new();
        private enum TurnEnum { player1, player2 };
        private TurnEnum currentturn;
        private bool playagainstcomputer = false;
        private Random rnd = new();
        private Card? card1 = null;
        private Card? card2 = null;
        public event PropertyChangedEventHandler? PropertyChanged;
        private List<string> allproperties = new() { "PlayerMode", "Player1Score", "Player2Score", "GameMessage", "GameMessageColor", "StartButtonText", "Player2Name", "DisableBtnDuringPlay" };
        private int match1;
        private int match2;
        private string player2 = string.Empty;
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
                        msg = currentturn == TurnEnum.player1 ? "Player 1's Turn" : player2 + "'s Turn";
                        break;
                    case GameStatusEnum.finished:
                        if (Player1Score == Player2Score) { msg = "Tie!"; }
                        else
                        {
                            msg = Player1Score > Player2Score ? "Player 1 won!" : player2 + " won!";
                        }
                        break;
                }
                return msg;
            }
        }
        public Color GameMessageColor { get => gamestatus == GameStatusEnum.notstarted ? Color.Black : gamestatus == GameStatusEnum.playing ? Color.Green : Color.MediumVioletRed; }
        public string StartButtonText { get => gamestatus == GameStatusEnum.playing ? "New Game" : "Start Game"; }
        public string Player2Name { get => playagainstcomputer ? "Computer's Sets" : "Player 2 Sets"; }
        public string PlayerMode { get => playagainstcomputer ? "Solo" : "2 Player"; }
        public bool DisableBtnDuringPlay { get => gamestatus == GameStatusEnum.playing ? false : true; }


        public void StartNewGame(bool solo = false)
        {
            playagainstcomputer = solo;
            player2 = playagainstcomputer == true ? "Computer" : "Player 2";

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
                if (!pickedcards.Contains(selcard)) { pickedcards.Add(selcard); }

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
                            pickedcards.Remove(card1);
                            pickedcards.Remove(card2);

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
                        card1 = null;
                        card2 = null;

                        if (playagainstcomputer && currentturn == TurnEnum.player2 && gamestatus == GameStatusEnum.playing)
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
