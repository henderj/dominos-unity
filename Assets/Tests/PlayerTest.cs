using NUnit.Framework;

namespace Tests
{
    public class PlayerTest
    {
        private Player player;
        [SetUp]
        public void Setup()
        {
            player = new Player("PT");
        }
        
        [Test]
        public void HasDoubleSix()
        {
            player.GiveDominoes(new[]{new Domino(6,6)});
            Assert.True(player.HasDoubleSix());
        }

        [Test]
        public void IsHandEmpty()
        {
            Assert.True(player.IsHandEmpty());
        }

        [Test]
        public void PlayDoubleSix()
        {
            player.GiveDominoes(new[]{new Domino(6,6)});
            var expected = new Domino(6, 6);
            var actual = player.PlayDoubleSix();
            Assert.AreEqual(expected, actual);
        }

        // // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // // `yield return null;` to skip a frame.
        // [UnityTest]
        // public IEnumerator PlayerTestWithEnumeratorPasses()
        // {
        //     // Use the Assert class to test conditions.
        //     // Use yield to skip a frame.
        //     yield return null;
        // }
    }
}
