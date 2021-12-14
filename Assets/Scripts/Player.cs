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

    private List<Domino> _hand;
    public int Score;

    public Player(string name, IMoveChooser chooser = null)
    {
        Name = name;
        _chooser = chooser ?? new RandomChooser();
        _hand = new List<Domino>(7);
    }

    public string HandString => string.Join(", ", _hand);

    public bool IsHandEmpty()
    {
        return !_hand.Any();
    }

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

    public bool HasDoubleSix()
    {
        return _hand.Contains(new Domino(6, 6));
    }

    public TurnData TakeTurn(List<Domino> playedDominoes)
    {
        var moves = GameRules.GetLegalMoves(playedDominoes, _hand);
        var beforeTurn = playedDominoes.ToArray();
        if (moves.Length == 0) return new TurnData(this, false, beforeTurn, null);


        var move = _chooser.Choose(moves, playedDominoes.ToArray());
        var domino = move.Flip ? move.Domino.Flip() : move.Domino;
        if (move.InsertAtEnd) playedDominoes.Add(domino);
        else playedDominoes.Insert(0, domino);
        _hand.Remove(move.Domino);
        return new TurnData(this, true, beforeTurn, domino);
    }

    public bool CanPlay(List<Domino> playedDominoes)
    {
        return GameRules.GetLegalMoves(playedDominoes, _hand).Length > 0;
    }

    public int GetPoints()
    {
        return _hand.Sum(d => d.Value);
    }
}