using MemoryGameSystem;

namespace MemoryGameSystemTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

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
            int i = 0;
            char? card1 = null;
            char? card2 = null;
            List<char> beforeshuffle = new();
            List<char> aftershuffle = new();
            MemoryGame game = new();
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
            Assert.That(card2, Is.Not.EqualTo(card1), "The shuffle has failed");
            TestContext.WriteLine($"The shuffle has completed successfully. Card num {i++} had the following picture before shuffle '{card1}' and was changed to '{card2}' after shuffle"); ;
        }
    }
}