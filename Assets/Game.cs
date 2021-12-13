using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IDisplayWrapper
{
    public void DisplayGame(Game game);
}


public class Game : MonoBehaviour
{
    private int _numPlayers = 4;
    private int _terminatingScore = 200;
    private int _gameNum = 1;
    private int _roundNum = 1;
    private List<Domino> _playedDominos = new List<Domino>(28);
    private List<TurnData> _roundHistory = new List<TurnData>(150);
    private Player[] _players = new Player[4];
    private Player _nextPlayer;
    private Player _winner;
    private bool _firstGameRoundStepFlag = false;

    private IDisplayWrapper _displayWrapper;

    private void Reset()
    {
        _playedDominos.Clear();
        _roundHistory.Clear();
        foreach (var p in _players)
        {
            p.Reset();
        }
    }


    private void NewGame()
    {
        _players = new[] {new Player(), new Player(), new Player(), new Player()};
        Reset();
        _winner = _players[0];
        _firstGameRoundStepFlag = true;
    }


    private void NewRound()
    {
        Reset();
        var pool = Domino.GetAllDominoes();
        pool = pool.Shuffle().ToArray();
        var i = 0;
        foreach (var p in _players)
        {
            p.GiveDominoes(pool.Skip(i * 7).Take(7));
            i++;
        }

        if (_firstGameRoundStepFlag)
        {
            foreach (var player in _players)
            {
                if (!player.HasDoubleSix()) continue;
                _winner = player;
                break;
            }
        }

        _nextPlayer = _winner;
    }


    public struct TurnData
    {
        public Player Player;
        public bool DidPlay;
        public Domino[] BeforeTurn;
        public Domino? DominoPlayed;

        public TurnData(Player player, bool didPlay, Domino[] beforeTurn, Domino? domino)
        {
            Player = player;
            DidPlay = didPlay;
            BeforeTurn = beforeTurn;
            DominoPlayed = domino;
        }
    }

    private bool Step()
    {
        TurnData turnData;
        var beforeTurn = new Domino[_playedDominos.Count()];
        _playedDominos.CopyTo(beforeTurn);
        if (_firstGameRoundStepFlag)
        {
            _playedDominos.Add(_nextPlayer.PlayDoubleSix());
            turnData = new TurnData(_nextPlayer, true, beforeTurn, new Domino(6, 6));
            _firstGameRoundStepFlag = false;
        }
        else
        {
            turnData = _nextPlayer.TakeTurn(_playedDominos);
        }

        _roundHistory.Add(turnData);

        CheckForBonusPoints();

        if (_nextPlayer.IsHandEmpty())
        {
            _winner = _nextPlayer;
            return false;
        }

        if (IsLocked())
        {
            var pCurrent = _nextPlayer;
            var pNext = GetNextPlayerIndex();
            _winner = pCurrent.GetPoints() < pNext.GetPoints() ? pCurrent : pNext;
            return false;
        }

        _nextPlayer = GetNextPlayerIndex();
        return true;
    }

    private void CheckForBonusPoints()
    {
        const int bonusPoints = 25;
        if (_roundHistory.Count() < 2) return;
        var lastMove = _roundHistory.Last();
        if (lastMove.DidPlay) return;
        var lastPlayer = lastMove.Player;
        if (_roundHistory.Count() == 2)
        {
            GivePointsToTeam(lastPlayer, bonusPoints);
            return;
        }

        if (_roundHistory.Count() < 4) return;
        var lastFour = _roundHistory.Skip(_roundHistory.Count() - 4).Take(4).ToArray();
        if (lastFour[0].DidPlay && lastFour.Skip(1).Take(3).All(move => move.DidPlay == false))
        {
            var player = lastFour[0].Player;
            GivePointsToTeam(player, bonusPoints);
            return;
        }

        if (lastPlayer.IsHandEmpty() == false) return;

        if (lastMove.DominoPlayed == null) return;
        var lastDomino = lastMove.DominoPlayed.Value;
        var head = lastMove.BeforeTurn[0].LeftSide;
        var tail = lastMove.BeforeTurn.Last().RightSide;
        if ((lastDomino.LeftSide == tail && lastDomino.RightSide == head) ||
            (lastDomino.RightSide == tail && lastDomino.LeftSide == head))
        {
            GivePointsToTeam(lastPlayer, bonusPoints);
            return;
        }
    }


    private Player GetNextPlayerIndex()
    {
        var index = Array.IndexOf(_players, _nextPlayer);
        index = (index + 1) % _players.Length;
        return _players[index];
    }


    private void GivePointsToTeam(Player player, int points)
    {
        var index = Array.IndexOf(_players, player);
        var team = new Player[2];
        if (index == 0 || index == 2)
        {
            team[0] = _players[0];
            team[1] = _players[2];
        }
        else
        {
            team[0] = _players[1];
            team[1] = _players[3];
        }

        foreach (var p in team)
        {
            p.Score += points;
        }
    }

    private void UpdateScore()
    {
        var totalPoints = _players.Sum(p => p.GetPoints());
        GivePointsToTeam(_winner, totalPoints);
    }

    private bool IsLocked()
    {
        var head = _playedDominos[0].LeftSide;
        var tail = _playedDominos.Last().RightSide;
        if (head != tail) return false;

        var count = 0;
        foreach (var domino in _playedDominos)
        {
            if (domino.LeftSide == head) count++;
            if (domino.RightSide == head) count++;
        }

        return count == 8;
    }

    private void DisplayScore()
    {
        foreach (var player in _players)
        {
            print($"{player.Name}: {player.Score}");
        }
    }

    public void PlayRound()
    {
        NewRound();
        while (Step())
        {
            _displayWrapper.DisplayGame(this);
        }

        _displayWrapper.DisplayGame(this);
        UpdateScore();
    }

    public void PlayGame()
    {
        NewGame();
        while (_winner.Score < _terminatingScore) PlayRound();
    }
}