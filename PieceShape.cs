using System;

public sealed class PieceShape
{
    private readonly Vec2I[] _offsets;

    public ArraySegment<Vec2I> Offsets { get; }
    public int Area { get; }
    public int Width { get; }
    public int Height { get; }

    public PieceShape(params Vec2I[] offsets)
    {
        if (offsets == null || offsets.Length == 0)
            throw new ArgumentException("PieceShape requires at least one offset.");

        // --- FIX СПАВНА ПОПЕРЁК КЛЕТОК ---
        // 1. Находим минимальные координаты и сдвигаем всю фигуру так,
        //    чтобы её левый нижний угол стал ровно (0,0).
        int minX = int.MaxValue, minY = int.MaxValue;
        for (int i = 0; i < offsets.Length; i++)
        {
            if (offsets[i].X < minX) minX = offsets[i].X;
            if (offsets[i].Y < minY) minY = offsets[i].Y;
        }

        _offsets = new Vec2I[offsets.Length];
        for (int i = 0; i < offsets.Length; i++)
            _offsets[i] = new Vec2I(offsets[i].X - minX, offsets[i].Y - minY);

        // 2. Убираем дубликаты (иначе Area врёт, а спавнер займет меньше клеток, чем посчитал).
        int unique = 0;
        for (int i = 0; i < _offsets.Length; i++)
        {
            bool dup = false;
            for (int j = 0; j < i; j++)
                if (_offsets[j] == _offsets[i]) { dup = true; break; }
            if (!dup) _offsets[unique++] = _offsets[i];
        }
        if (unique != _offsets.Length) Array.Resize(ref _offsets, unique);

        // 3. Детерминированный порядок (снизу-вверх, слева-направо) — стабильный рендер и хитбоксы.
        Array.Sort(_offsets, (a, b) => a.Y != b.Y ? a.Y.CompareTo(b.Y) : a.X.CompareTo(b.X));

        // 4. Габариты считаем после нормализации — они всегда корректны.
        int maxX = 0, maxY = 0;
        for (int i = 0; i < _offsets.Length; i++)
        {
            if (_offsets[i].X > maxX) maxX = _offsets[i].X;
            if (_offsets[i].Y > maxY) maxY = _offsets[i].Y;
        }
        Width = maxX + 1;
        Height = maxY + 1;
        Area = _offsets.Length;

        Offsets = new ArraySegment<Vec2I>(_offsets);
    }

    // Публичные статические экземпляры остаются публичными — сравнение ссылками в GetSpriteForPiece работает как раньше.
    public static readonly PieceShape Single  = new PieceShape(new Vec2I(0, 0));
    public static readonly PieceShape Line2H   = new PieceShape(new Vec2I(0, 0), new Vec2I(1, 0));
    public static readonly PieceShape Line2V   = new PieceShape(new Vec2I(0, 0), new Vec2I(0, 1));
    public static readonly PieceShape Square2  = new PieceShape(new Vec2I(0, 0), new Vec2I(1, 0), new Vec2I(0, 1), new Vec2I(1, 1));
    public static readonly PieceShape Rect2x3  = new PieceShape(new Vec2I(0, 0), new Vec2I(1, 0), new Vec2I(0, 1), new Vec2I(1, 1), new Vec2I(0, 2), new Vec2I(1, 2));
    public static readonly PieceShape Rect3x2  = new PieceShape(new Vec2I(0, 0), new Vec2I(1, 0), new Vec2I(2, 0), new Vec2I(0, 1), new Vec2I(1, 1), new Vec2I(2, 1));
    public static readonly PieceShape Line3H   = new PieceShape(new Vec2I(0, 0), new Vec2I(1, 0), new Vec2I(2, 0));
    public static readonly PieceShape Line3V   = new PieceShape(new Vec2I(0, 0), new Vec2I(0, 1), new Vec2I(0, 2));
    public static readonly PieceShape Line4H   = new PieceShape(new Vec2I(0, 0), new Vec2I(1, 0), new Vec2I(2, 0), new Vec2I(3, 0));
    public static readonly PieceShape Line4V   = new PieceShape(new Vec2I(0, 0), new Vec2I(0, 1), new Vec2I(0, 2), new Vec2I(0, 3));
    public static readonly PieceShape Gamma    = new PieceShape(new Vec2I(0, 0), new Vec2I(1, 0), new Vec2I(0, 1));
    public static readonly PieceShape ShapeZ   = new PieceShape(new Vec2I(1, 0), new Vec2I(2, 0), new Vec2I(0, 1), new Vec2I(1, 1));

    public static readonly PieceShape[] All =
    {
        Single, Line2H, Line2V, Square2, Rect2x3, Rect3x2,
        Line3H, Line3V, Line4H, Line4V, Gamma, ShapeZ
    };
}