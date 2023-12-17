using MemoryGameSystem;

namespace MemoryGameSystemTest
{
    public class Tests
    {

        [Test]
        public void StartGame()
        {
            MemoryGame game = new();
            Assert.IsTrue(game.gamestatus == MemoryGame.GameStatusEnum.notstarted, "Game is not in notstarted status, cannot run test");
            game.StartNewGame();
            Assert.IsTrue(game.gamestatus == MemoryGame.GameStatusEnum.playing, "Game status has not changed to playing, test failed");
            TestContext.WriteLine("Game status changed to playing from not started");
        }

        [Test]
        public void Shuffle()
        {
            MemoryGame game = new();
            int i = 0;
            char? card1 = null;
            char? card2 = null;
            List<char> beforeshuffle = new();
            List<char> aftershuffle = new();

            game.StartNewGame();
            game.Cards.ForEach(c => beforeshuffle.Add(c.CardPicture));

            do
            { game.StartNewGame(); }
            while (game.gamestatus != MemoryGame.GameStatusEnum.playing);

            game.Cards.ForEach(c => aftershuffle.Add(c.CardPicture));

            foreach (char c in beforeshuffle)
            {
                if (c != aftershuffle[i])
                {
                    card1 = c;
                    card2 = aftershuffle[i];
                    break;
                }
                i++;
            }
            Assert.That(card2, Is.Not.EqualTo(card1), "The shuffle has failed, all cards have the same picture as before shuffle");
            TestContext.WriteLine($"The shuffle has completed successfully. Card num {i++} had the following picture before shuffle '{card1}' and was changed to '{card2}' after shuffle"); ;
        }
        [TestCase(0)]
        [TestCase(5)]
        [TestCase(19)]
        public async Task PlayCard(int cardind)
        {
            MemoryGame game = new();
            game.StartNewGame();
            Assert.IsTrue(game.Cards.Count(c => c.CardStatus == Card.CardStatusEnum.Facedown) == 20 && game.gamestatus == MemoryGame.GameStatusEnum.playing, "Not all cards are face down or game has not started yet, cannot run test.");
            await game.PlayCard(cardind);
            Assert.IsTrue(game.Cards[cardind].CardStatus == Card.CardStatusEnum.Faceup, "Card was not picked");
            TestContext.WriteLine($"Card num {++cardind} was picked successfully.");
        }
        [Test]
        public async Task ClaimMatch()
        {
            MemoryGame game = new();
            game.StartNewGame();
            Assert.IsTrue(game.Cards.Count(c => c.CardStatus == Card.CardStatusEnum.Facedown) == 20
                && game.gamestatus == MemoryGame.GameStatusEnum.playing
                && game.Player1Score == 0, "Something is wrong with game setup, cannot run test.");
            int card1 = 0;
            int card2 = game.Cards.FindLastIndex(c => c.CardPicture == game.Cards[card1].CardPicture);
            Assert.IsTrue(card2 > 0, "Couldn't find matching cards, cannot run test");
            await game.PlayCard(card1);
            await game.PlayCard(card2);
            Assert.IsTrue(game.Cards[card1].CardStatus == Card.CardStatusEnum.Claimed && game.Cards[card2].CardStatus == Card.CardStatusEnum.Claimed && game.Player1Score == 1, "Cards where not claimed or player1 score did not increase.");
            TestContext.WriteLine("The picked cards where claimed and Player 1 score increased.");
        }
        [Test]
        //Following test will take at least 20 sec to conclude
        public async Task FinishGame()
        {
            MemoryGame game = new();
            game.StartNewGame();
            Assert.IsTrue(game.gamestatus == MemoryGame.GameStatusEnum.playing, "Game did not start");

            while (game.Cards.Count(c => c.IsVisible) > 1)
            {
                int card1 = game.Cards.FindIndex(c => c.IsVisible == true);
                int card2 = game.Cards.FindLastIndex(c => c.CardPicture == game.Cards[card1].CardPicture);
                await game.PlayCard(card1); await game.PlayCard(card2);
            }
            Assert.IsTrue(game.Cards.Count(c => c.IsVisible) == 0 && game.gamestatus == MemoryGame.GameStatusEnum.finished, "Game was not changed to finish status");
            TestContext.WriteLine($"Game finished with the following message {game.GameMessage}");
        }
        [Test]
        public async Task ComputerPicksMatch()
        {
            MemoryGame game = new();
            game.StartNewGame(true);
            Assert.IsTrue(game.gamestatus == MemoryGame.GameStatusEnum.playing, "Game did not start");

            await game.PlayCard(0);
            int cardnotmatch = game.Cards.FindIndex(c => c.CardPicture != game.Cards[0].CardPicture);
            await game.PlayCard(cardnotmatch);
            //Now the computer does its move

            int comscore = game.Player2Score;

            Assert.IsTrue(game.Cards[0].CardStatus == Card.CardStatusEnum.Facedown, "Computer has already found the match for the first card picked, rerun test");
            await game.PlayCard(cardnotmatch);
            int cardmatch = game.Cards.FindIndex(1, c => c.CardPicture == game.Cards[0].CardPicture);
            await game.PlayCard(cardmatch);
            
            //Now the computer does its 2nd move

            Assert.IsTrue(game.Player2Score == ++comscore, "Computer has not claimed any set in its 2nd turn");
            Assert.IsTrue(game.Cards[0].CardStatus == Card.CardStatusEnum.Claimed, "Computer has a card buy not the 1st card that was uncovered. Results are inconclusive. Rerun test");
            TestContext.WriteLine("Computer has picked the 1st card with its matching card that have been previously picked.");
        }
    }
}