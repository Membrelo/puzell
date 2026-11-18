public struct Cell
{
    public const int NoPiece = 0;

    public CellEffectType Effect;
    public int PieceId; // 0 == Cell.NoPiece

    public bool IsOccupied => PieceId != NoPiece;
    public bool IsEmpty => PieceId == NoPiece;
}