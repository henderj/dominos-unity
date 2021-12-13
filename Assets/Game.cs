using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game
{
    private const int NumPlayers = 4;
    private int _terminatingScore = 200;
    private readonly List<TurnData> _roundHistory = new List<TurnData>(150);
    private Player _nextPlayer;
    private Player _winner;
    private bool _firstGameRoundStepFlag;

    private readonly IDisplayWrapper _displayWrapper;

    public Game(IDisplayWrapper displayWrapper = null)
    {
        _displayWrapper = displayWrapper ?? new DisplayWrapperNone();
    }

    public Player[] Players { get; private set; } = new Player[NumPlayers];

    public List<Domino> PlayedDominoes { get; } = new List<Domino>(28);

    public int CurrentTurnIndex => Array.IndexOf(Players, _nextPlayer);

    private void Reset()
    {
        PlayedDominoes.Clear();
        _roundHistory.Clear();
        foreach (var p in Players)
        {
            p.Reset();
        }
    }


    private void NewGame()
    {
        var count = 0;
        Players = new[]
        {
            new Player($"P{count++}"),
            new Player($"P{count++}"),
            new Player($"P{count++}"),
            new Player($"P{count}")
        };
        Reset();
        _winner = Players[0];
        _firstGameRoundStepFlag = true;
    }


    private void NewRound()
    {
        Reset();
        var pool = Domino.GetAllDominoes();
        pool = pool.Shuffle().ToArray();
        var i = 0;
        foreach (var p in Players)
        {
            p.GiveDominoes(pool.Skip(i * 7).Take(7));
            i++;
        }

        if (_firstGameRoundStepFlag)
        {
            foreach (var player in Players)
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
        public readonly Player Player;
        public readonly bool DidPlay;
        public readonly Domino[] BeforeTurn;
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
        var beforeTurn = new Domino[PlayedDominoes.Count()];
        PlayedDominoes.CopyTo(beforeTurn);
        if (_firstGameRoundStepFlag)
        {
            PlayedDominoes.Add(_nextPlayer.PlayDoubleSix());
            turnData = new TurnData(_nextPlayer, true, beforeTurn, new Domino(6, 6));
            _firstGameRoundStepFlag = false;
        }
        else
        {
            turnData = _nextPlayer.TakeTurn(PlayedDominoes);
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
        }
    }


    private Player GetNextPlayerIndex()
    {
        var index = Array.IndexOf(Players, _nextPlayer);
        index = (index + 1) % Players.Length;
        return Players[index];
    }


    private void GivePointsToTeam(Player player, int points)
    {
        var index = Array.IndexOf(Players, player);
        var team = new Player[2];
        if (index == 0 || index == 2)
        {
            team[0] = Players[0];
            team[1] = Players[2];
        }
        else
        {
            team[0] = Players[1];
            team[1] = Players[3];
        }

        foreach (var p in team)
        {
            p.Score += points;
        }
    }

    private void UpdateScore()
    {
        var totalPoints = Players.Sum(p => p.GetPoints());
        GivePointsToTeam(_winner, totalPoints);
    }

    private bool IsLocked()
    {
        var head = PlayedDominoes[0].LeftSide;
        var tail = PlayedDominoes.Last().RightSide;
        if (head != tail) return false;

        var count = 0;
        foreach (var domino in PlayedDominoes)
        {
            if (domino.LeftSide == head) count++;
            if (domino.RightSide == head) count++;
        }

        return count == 8;
    }

    private void DisplayScore()
    {
        foreach (var player in Players)
        {
            Debug.Log($"{player.Name}: {player.Score}");
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