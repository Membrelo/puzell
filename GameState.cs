using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public const int Size = 5;

    public Cell[,] Grid { get; }
    public List<Piece> Pieces { get; } = new List<Piece>();

    private int _nextPieceId = 1;

    public GameState()
    {
        Grid = new Cell[Size, Size];
        for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
                Grid[x, y] = new Cell();
    }

    public bool InBounds(Vector2Int p)
    {
        return p.x >= 0 && p.x < Size && p.y >= 0 && p.y < Size;
    }

    public bool CanPlace(Piece piece, Vector2Int newAnchor)
    {
        foreach (var offset in piece.Shape.Offsets)
        {
            var c = newAnchor + offset;
            if (!InBounds(c)) return false;

            var cell = Grid[c.x, c.y];
            if (cell.IsOccupied && cell.PieceId != piece.Id)
                return false;
        }
        return true;
    }

    public void PlacePiece(Piece piece)
    {
        if (piece.Id == 0) piece.Id = _nextPieceId++;
        Pieces.Add(piece);

        foreach (var c in piece.Cells())
            Grid[c.x, c.y].PieceId = piece.Id;
    }

    public void MovePiece(Piece piece, Vector2Int newAnchor)
    {
        foreach (var c in piece.Cells())
            Grid[c.x, c.y].PieceId = null;

        piece.Anchor = newAnchor;

        foreach (var c in piece.Cells())
            Grid[c.x, c.y].PieceId = piece.Id;
    }

    public Piece GetPieceById(int id)
    {
        return Pieces.Find(p => p.Id == id);
    }

    public bool IsGameOver()
    {
        foreach (var piece in Pieces)
        {
            foreach (ShiftDirection dir in System.Enum.GetValues(typeof(ShiftDirection)))
            {
                var delta = ShiftResolver.DeltaOf(dir);
                if (CanPlace(piece, piece.Anchor + delta))
                    return false;
            }
        }
        return true;
    }
}