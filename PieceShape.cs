using System.Collections.Generic;
using UnityEngine;

public class PieceShape
{
    public IReadOnlyList<Vector2Int> Offsets { get; }
    public int Area => Offsets.Count;
    public int Width { get; }
    public int Height { get; }

    public PieceShape(params Vector2Int[] offsets)
    {
        Offsets = offsets;
        int maxX = 0, maxY = 0;
        foreach (var o in offsets)
        {
            if (o.x + 1 > maxX) maxX = o.x + 1;
            if (o.y + 1 > maxY) maxY = o.y + 1;
        }
        Width = maxX;
        Height = maxY;
    }

    public static readonly PieceShape Single = new PieceShape(new Vector2Int(0, 0));
    public static readonly PieceShape Line2H = new PieceShape(new Vector2Int(0, 0), new Vector2Int(1, 0));
    public static readonly PieceShape Line2V = new PieceShape(new Vector2Int(0, 0), new Vector2Int(0, 1));
    public static readonly PieceShape Square2 = new PieceShape(new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1));
    public static readonly PieceShape Rect2x3 = new PieceShape(new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(0, 2), new Vector2Int(1, 2));
    public static readonly PieceShape Rect3x2 = new PieceShape(new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1));
    public static readonly PieceShape Line3H = new PieceShape(new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0));
    public static readonly PieceShape Line3V = new PieceShape(new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2));
    public static readonly PieceShape Line4H = new PieceShape(new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0));
    public static readonly PieceShape Line4V = new PieceShape(new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(0, 3));
    public static readonly PieceShape Gamma = new PieceShape(new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1));
    public static readonly PieceShape ShapeZ = new PieceShape(new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, 1), new Vector2Int(1, 1));

    public static readonly PieceShape[] All =
    {
        Single, Line2H, Line2V, Square2, Rect2x3, Rect3x2,
        Line3H, Line3V, Line4H, Line4V, Gamma, ShapeZ
    };
}