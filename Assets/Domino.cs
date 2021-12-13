using System;
using System.Collections.Generic;
using System.Linq;

public struct Domino
{
    public int LeftSide;
    public int RightSide;

    public int Value => LeftSide + RightSide;

    public Domino(int leftSide, int rightSide)
    {
        LeftSide = leftSide;
        RightSide = rightSide;
    }

    public Domino Flip()
    {
        return new Domino(RightSide, LeftSide);
    }

    public override string ToString()
    {
        return $"({LeftSide},{RightSide})";
    }

    public static Domino[] GetAllDominoes()
    {
        return new[]
        {
            new Domino(0, 0),
            new Domino(0, 1),
            new Domino(0, 2),
            new Domino(0, 3),
            new Domino(0, 4),
            new Domino(0, 5),
            new Domino(0, 6),
            new Domino(1, 1),
            new Domino(1, 2),
            new Domino(1, 3),
            new Domino(1, 4),
            new Domino(1, 5),
            new Domino(1, 6),
            new Domino(2, 2),
            new Domino(2, 3),
            new Domino(2, 4),
            new Domino(2, 5),
            new Domino(2, 6),
            new Domino(3, 3),
            new Domino(3, 4),
            new Domino(3, 5),
            new Domino(3, 6),
            new Domino(4, 4),
            new Domino(4, 5),
            new Domino(4, 6),
            new Domino(5, 5),
            new Domino(5, 6),
            new Domino(6, 6)
        };
    }
}


public static class EnumerableExtensions
{
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> dominoes)
    {
        var rnd = new Random();
        return dominoes.OrderBy(x => rnd.Next()).ToArray();
    }
}