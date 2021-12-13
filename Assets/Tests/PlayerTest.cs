using NUnit.Framework;

namespace Tests
{
    public class PlayerTest
    {
        [Test]
        public void HasDoubleSix()
        {
            var player = new Player("PT");
            player.GiveDominoes(new Domino[]{new Domino(6,6)});
            Assert.True(player.HasDoubleSix());
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
