using UnityEngine;

public class RandomChooser : IMoveChooser
{
    public Move Choose(Move[] moves, Domino[] playedDominoes)
    {
        return moves[Random.Range(0, moves.Length)];
    }
}