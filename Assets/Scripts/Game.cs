using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game
{
    private const int NumPlayers = 4;

    private readonly IDisplayWrapper _displayWrapper;

    private readonly List<Domino> _playedDominoes = new List<Domino>(28);
    private readonly List<TurnData> _roundHistory = new List<TurnData>(150);
    private bool _firstGameRoundStepFlag;
    private Player _nextPlayer;
    private const int TerminatingScore = 200;
    private Player _winner;

    public Game(IDisplayWrapper displayWrapper = null)
    {
        _displayWrapper = displayWrapper ?? new DisplayWrapperNone();
    }

    public Player[] Players { get; private set; } = new Player[NumPlayers];

    public string PlayedDominoesString => string.Join(", ", _playedDominoes);

    public int CurrentTurnIndex => Array.IndexOf(Players, _nextPlayer);

    private void Reset()
    {
        _playedDominoes.Clear();
        _roundHistory.Clear();
        foreach (var p in Players) p.Reset();
    }


    public void NewGame(Player[] players = null)
    {
        var count = 0;
        Players = players ?? new[]
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


    public void NewRound()
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
            foreach (var player in Players)
            {
                if (!player.HasDoubleSix()) continue;
                _winner = player;
                break;
            }

        _nextPlayer = _winner;
    }

    public bool Step()
    {
        TurnData turnData;
        var beforeTurn = new Domino[_playedDominoes.Count()];
        _playedDominoes.CopyTo(beforeTurn);
        if (_firstGameRoundStepFlag)
        {
            _playedDominoes.Add(_nextPlayer.PlayDoubleSix());
            turnData = new TurnData(_nextPlayer, true, beforeTurn, new Domino(6, 6));
            _firstGameRoundStepFlag = false;
        }
        else
        {
            turnData = _nextPlayer.TakeTurn(_playedDominoes);
        }

        _roundHistory.Add(turnData);

        var bonusPoints = GameRules.GetBonusPoints(_roundHistory, Players, _playedDominoes);
        GivePointsToTeam(_nextPlayer, bonusPoints);

        if (_nextPlayer.IsHandEmpty())
        {
            _winner = _nextPlayer;
            GiveEndOfRoundPoints();
            _displayWrapper.DisplayGame(this);
            return false;
        }

        if (GameRules.IsLocked(_playedDominoes))
        {
            var pCurrent = _nextPlayer;
            var pNext = GetNextPlayerIndex();
            _winner = pCurrent.GetPoints() < pNext.GetPoints() ? pCurrent : pNext;
            GiveEndOfRoundPoints();
            _displayWrapper.DisplayGame(this);
            return false;
        }

        _nextPlayer = GetNextPlayerIndex();
        _displayWrapper.DisplayGame(this);
        return true;
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

        foreach (var p in team) p.Score += points;
    }

    private void GiveEndOfRoundPoints()
    {
        var totalPoints = Players.Sum(p => p.GetPoints());
        GivePointsToTeam(_winner, totalPoints);
    }


    private void DisplayScore()
    {
        foreach (var player in Players) Debug.Log($"{player.Name}: {player.Score}");
    }

    public void PlayRound()
    {
        NewRound();
        while (Step()) _displayWrapper.DisplayGame(this);

        _displayWrapper.DisplayGame(this);
        GiveEndOfRoundPoints();
    }

    public void PlayGame()
    {
        NewGame();
        while (_winner.Score < TerminatingScore) PlayRound();
    }
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