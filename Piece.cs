public sealed class Piece
{
    public int Id { get; }
    public PieceType Type { get; }
    public PieceShape Shape { get; }
    public Vec2I Position { get; set; } // Anchor = левый нижний угол

    public Piece(int id, PieceType type, PieceShape shape, Vec2I position)
    {
        Id = id;
        Type = type;
        Shape = shape;
        Position = position;
    }
}