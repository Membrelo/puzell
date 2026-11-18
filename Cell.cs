public class Cell
{
    public CellEffectType Effect = CellEffectType.None;
    public int? PieceId = null;

    public bool IsOccupied => PieceId.HasValue;
    public bool IsEmpty => !PieceId.HasValue;
}