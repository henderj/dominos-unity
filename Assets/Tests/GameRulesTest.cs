using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class GameRulesTest
    {
        private List<TurnData> _roundHistory;
        private Player[] _players;
        private List<Domino> _playedDominoes;

        [SetUp]
        public void Setup()
        {
            _roundHistory = new List<TurnData>(10);

            var count = 0;
            _players = new[]
            {
                new Player($"P{count++}"),
                new Player($"P{count++}"),
                new Player($"P{count++}"),
                new Player($"P{count}")
            };
            _playedDominoes = new List<Domino>(10);
        }

        [Test]
        public void FirstMoveBonusPoints()
        {
            _roundHistory.Add(new TurnData(_players[0], true, Array.Empty<Domino>(), new Domino(6,6)));
            _players[1].GiveDominoes(new []{new Domino(0,0)});
            _playedDominoes.Add(new Domino(6,6));
            const int expected = GameRules.BonusPoints;
            var actual = GameRules.GetBonusPoints(_roundHistory, _players, _playedDominoes);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void RoundTableBonusPoints()
        {
            _roundHistory.Add(new TurnData(_players[3], true, Array.Empty<Domino>(), new Domino(6,6)));
            _roundHistory.Add(new TurnData(_players[0], true, Array.Empty<Domino>(), new Domino(5,6)));
            _players[1].GiveDominoes(new []{new Domino(0,0)});
            _players[2].GiveDominoes(new []{new Domino(2,2)});
            _players[3].GiveDominoes(new []{new Domino(3,3)});
            _playedDominoes.Add(new Domino(6,6));
            _playedDominoes.Add(new Domino(5,6));
            const int expected = GameRules.BonusPoints;
            var actual = GameRules.GetBonusPoints(_roundHistory, _players, _playedDominoes);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CapriSomethingBonusPoints()
        {
            _roundHistory.Add(new TurnData(_players[1], true, Array.Empty<Domino>(), new Domino(6,6)));
            _playedDominoes.Add(new Domino(6,6));
            _roundHistory.Add(new TurnData(_players[2], true, Array.Empty<Domino>(), new Domino(4,6)));
            _playedDominoes.Insert(0,new Domino(4,6));
            _roundHistory.Add(new TurnData(_players[3], true, Array.Empty<Domino>(), new Domino(2,4)));
            _playedDominoes.Insert(0,new Domino(2,4));
            _roundHistory.Add(new TurnData(_players[0], true, Array.Empty<Domino>(), new Domino(6,2)));
            _playedDominoes.Insert(0,new Domino(6,2));
            _players[1].GiveDominoes(new []{new Domino(0,0)});
            _players[2].GiveDominoes(new []{new Domino(2,2)});
            _players[3].GiveDominoes(new []{new Domino(3,3)});
            const int expected = GameRules.BonusPoints;
            var actual = GameRules.GetBonusPoints(_roundHistory, _players, _playedDominoes);
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

