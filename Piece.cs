using System.Collections.Generic;
using UnityEngine;

public class Piece
{
    public int Id;
    public PieceType Type;
    public PieceShape Shape;
    public Vector2Int Anchor;

    public int Area => Shape.Area;

    public IEnumerable<Vector2Int> Cells()
    {
        foreach (var offset in Shape.Offsets)
            yield return Anchor + offset;
    }
}