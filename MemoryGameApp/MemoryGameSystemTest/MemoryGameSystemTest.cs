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

            // USE FOLLOWING FOR SHUFFLE TEST
            //List<char> beforeshuffle = new();
            //List<char> aftershuffle = new();
            //MemoryGame game = new();
            //game.StartNewGame();
            //game.Cards.ForEach(c => beforeshuffle.Add(c.CardPicture));
            //game.StartNewGame();
                 
        }
    }
}