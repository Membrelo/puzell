public static class ShiftResolver
{
    private static readonly Vec2I[] DirDeltas =
    {
        new Vec2I(0, 1),   // Up
        new Vec2I(0, -1),  // Down
        new Vec2I(-1, 0),  // Left
        new Vec2I(1, 0)    // Right
    };

    public static Vec2I GetDelta(ShiftDirection dir) => DirDeltas[(int)dir];

    /// <summary>Возвращает новую позицию фигуры после сдвига, если ход легален.</summary>
    public static bool TryResolve(GameState state, Piece piece, ShiftDirection dir, out Vec2I newPos)
    {
        newPos = piece.Position + GetDelta(dir);
        return state.CanPlace(piece, newPos);
    }
}