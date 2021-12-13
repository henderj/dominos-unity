using System;
using System.Collections.Generic;
using System.Linq;

public struct Move
{
    public bool InsertAtEnd;
    public Domino Domino;
    public bool Flip;

    public Move(bool insertAtEnd, Domino domino, bool flip)
    {
        this.InsertAtEnd = insertAtEnd;
        this.Domino = domino;
        this.Flip = flip;
    }
}

public class Player
{
    public int Score;
    public string Name;

    private List<Domino> _hand;
    private IMoveChooser _chooser;

    public Player(IMoveChooser chooser = null)
    {
        _chooser = chooser ?? new RandomChooser();
    }

    public bool IsHandEmpty() => !_hand.Any();

    public void Reset()
    {
        _hand.Clear();
    }

    public Domino PlayDoubleSix()
    {
        var doubleSix = new Domino(6, 6);
        _hand.Remove(doubleSix);
        return doubleSix;
    }

    public void GiveDominoes(IEnumerable<Domino> dominoes)
    {
        _hand = dominoes.ToList();
    }

    public bool HasDoubleSix() => _hand.Contains(new Domino(6, 6));

    public Game.TurnData TakeTurn(List<Domino> playedDominoes)
    {
        var moves = GetLegalMoves(playedDominoes);
        var beforeTurn = playedDominoes.ToArray();
        if (moves.Length == 0) return new Game.TurnData(this, false, beforeTurn, null);


        var move = _chooser.Choose(moves, playedDominoes.ToArray());
        var domino = move.Flip ? move.Domino.Flip() : move.Domino;
        if (move.InsertAtEnd) playedDominoes.Add(domino);
        else playedDominoes.Insert(0, domino);
        _hand.Remove(move.Domino);
        return new Game.TurnData(this, true, beforeTurn, domino);
    }


    private Move[] GetLegalMoves(List<Domino> playedDominoes)
    {
        if (playedDominoes.Count == 0) return _hand.Select(d => new Move(true, d, false)).ToArray();
        var head = playedDominoes[0].LeftSide;
        var tail = playedDominoes.Last().RightSide;
        var moves = new List<Move>();
        foreach (var d in _hand)
        {
            if (d.LeftSide == tail) moves.Add(new Move(true, d, false));
            if (d.RightSide == tail) moves.Add(new Move(true, d, true));
            if (d.LeftSide == head) moves.Add(new Move(false, d, true));
            if (d.RightSide == head) moves.Add(new Move(false, d, false));
        }

        return moves.ToArray();
    }

    public int GetPoints() => _hand.Sum(d => d.Value);
}