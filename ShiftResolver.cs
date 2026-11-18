using UnityEngine;

public static class ShiftResolver
{
    public static Vector2Int DeltaOf(ShiftDirection dir)
    {
        switch (dir)
        {
            case ShiftDirection.Up:    return new Vector2Int(0, 1);
            case ShiftDirection.Down:  return new Vector2Int(0, -1);
            case ShiftDirection.Left:  return new Vector2Int(-1, 0);
            case ShiftDirection.Right: return new Vector2Int(1, 0);
            default: return Vector2Int.zero;
        }
    }

    public static bool TryShift(GameState state, Piece piece, ShiftDirection dir)
    {
        var newAnchor = piece.Anchor + DeltaOf(dir);

        if (!state.CanPlace(piece, newAnchor))
            return false;

        state.MovePiece(piece, newAnchor);

        return true;
    }
}