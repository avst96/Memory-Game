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
    }
}