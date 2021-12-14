using System;
using System.Collections.Generic;
using System.Linq;

public static class GameRules
{
    public const int BonusPoints = 25;

    public static int GetBonusPoints(List<TurnData> roundHistory, Player[] players, List<Domino> playedDominoes)
    {
        if (!roundHistory.Any()) return 0;
        var lastMove = roundHistory.Last();
        if (!lastMove.DidPlay) return 0;
        var lastPlayer = lastMove.Player;
        var lastPlayerIndex = Array.IndexOf(players, lastPlayer);
        var nextPlayer = players[(lastPlayerIndex + 1) % players.Length];
        if (roundHistory.Count() == 1 && nextPlayer.CanPlay(playedDominoes) == false) return BonusPoints;

        var otherPlayers = players.Except(new[]{lastPlayer});
        if (otherPlayers.All(p => p.CanPlay(playedDominoes) == false)) return BonusPoints;

        if (lastPlayer.IsHandEmpty() == false) return 0;

        if (lastMove.DominoPlayed == null) return 0;
        var lastDomino = lastMove.DominoPlayed.Value;
        var head = lastMove.BeforeTurn[0].LeftSide;
        var tail = lastMove.BeforeTurn.Last().RightSide;
        if (lastDomino.LeftSide == tail && lastDomino.RightSide == head ||
            lastDomino.RightSide == tail && lastDomino.LeftSide == head) return BonusPoints;
        return 0;
    }
    
    public static bool IsLocked(List<Domino> playedDominoes)
    {
        var head = playedDominoes[0].LeftSide;
        var tail = playedDominoes.Last().RightSide;
        if (head != tail) return false;

        var count = 0;
        foreach (var domino in playedDominoes)
        {
            if (domino.LeftSide == head) count++;
            if (domino.RightSide == head) count++;
        }

        return count == 8;
    }
    
    public static Move[] GetLegalMoves(IReadOnlyList<Domino> playedDominoes, List<Domino> hand)
    {
        if (playedDominoes.Count == 0) return hand.Select(d => new Move(true, d, false)).ToArray();
        var head = playedDominoes[0].LeftSide;
        var tail = playedDominoes.Last().RightSide;
        var moves = new List<Move>();
        foreach (var d in hand)
        {
            if (d.LeftSide == tail) moves.Add(new Move(true, d, false));
            if (d.RightSide == tail) moves.Add(new Move(true, d, true));
            if (d.LeftSide == head) moves.Add(new Move(false, d, true));
            if (d.RightSide == head) moves.Add(new Move(false, d, false));
        }

        return moves.ToArray();
    }
}