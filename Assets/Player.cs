using System.Collections.Generic;
using System.Linq;

public struct Move
{
    public readonly bool InsertAtEnd;
    public Domino Domino;
    public readonly bool Flip;

    public Move(bool insertAtEnd, Domino domino, bool flip)
    {
        InsertAtEnd = insertAtEnd;
        Domino = domino;
        Flip = flip;
    }
}

public class Player
{
    private readonly IMoveChooser _chooser;
    public readonly string Name;
    public int Score;

    public Player(string name, IMoveChooser chooser = null)
    {
        Name = name;
        _chooser = chooser ?? new RandomChooser();
        Hand = new List<Domino>(7);
    }

    public List<Domino> Hand { get; private set; }

    public bool IsHandEmpty()
    {
        return !Hand.Any();
    }

    public void Reset()
    {
        Hand.Clear();
    }

    public Domino PlayDoubleSix()
    {
        var doubleSix = new Domino(6, 6);
        Hand.Remove(doubleSix);
        return doubleSix;
    }

    public void GiveDominoes(IEnumerable<Domino> dominoes)
    {
        Hand = dominoes.ToList();
    }

    public bool HasDoubleSix()
    {
        return Hand.Contains(new Domino(6, 6));
    }

    public Game.TurnData TakeTurn(List<Domino> playedDominoes)
    {
        var moves = GetLegalMoves(playedDominoes);
        var beforeTurn = playedDominoes.ToArray();
        if (moves.Length == 0) return new Game.TurnData(this, false, beforeTurn, null);


        var move = _chooser.Choose(moves, playedDominoes.ToArray());
        var domino = move.Flip ? move.Domino.Flip() : move.Domino;
        if (move.InsertAtEnd) playedDominoes.Add(domino);
        else playedDominoes.Insert(0, domino);
        Hand.Remove(move.Domino);
        return new Game.TurnData(this, true, beforeTurn, domino);
    }


    private Move[] GetLegalMoves(List<Domino> playedDominoes)
    {
        if (playedDominoes.Count == 0) return Hand.Select(d => new Move(true, d, false)).ToArray();
        var head = playedDominoes[0].LeftSide;
        var tail = playedDominoes.Last().RightSide;
        var moves = new List<Move>();
        foreach (var d in Hand)
        {
            if (d.LeftSide == tail) moves.Add(new Move(true, d, false));
            if (d.RightSide == tail) moves.Add(new Move(true, d, true));
            if (d.LeftSide == head) moves.Add(new Move(false, d, true));
            if (d.RightSide == head) moves.Add(new Move(false, d, false));
        }

        return moves.ToArray();
    }

    public int GetPoints()
    {
        return Hand.Sum(d => d.Value);
    }
}