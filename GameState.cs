using System;
using System.Collections.Generic;

public sealed class GameState
{
    public int Width { get; }
    public int Height { get; }
    private readonly Cell[] _cells;
    private readonly Dictionary<int, Piece> _piecesById = new Dictionary<int, Piece>();
    private readonly List<Piece> _pieces = new List<Piece>();
    private readonly Random _rng;
    private int _nextPieceId = 1;

    public IReadOnlyList<Piece> Pieces => _pieces;

    public GameState(int width, int height, int seed = 0)
    {
        Width = width;   // 5
        Height = height; // 5
        _cells = new Cell[width * height];
        for (int i = 0; i < _cells.Length; i++) _cells[i].PieceId = Cell.NoPiece;
        _rng = seed != 0 ? new Random(seed) : new Random();
    }

    public bool InBounds(Vec2I c) => c.X >= 0 && c.Y >= 0 && c.X < Width && c.Y < Height;

    public ref Cell CellAt(Vec2I c) => ref _cells[c.Y * Width + c.X];

    public bool TryGetPiece(int id, out Piece piece) => _piecesById.TryGetValue(id, out piece);

    /// <summary>Проверка: влезает ли фигура shape в anchor и не пересекается ли с другими.</summary>
    public bool CanPlace(PieceShape shape, Vec2I anchor, int ignoreId = 0)
    {
        var offs = shape.Offsets;
        for (int i = 0; i < offs.Count; i++)
        {
            Vec2I c = anchor + offs.Array[i];
            if (!InBounds(c)) return false;
            int pid = _cells[c.Y * Width + c.X].PieceId;
            if (pid != Cell.NoPiece && pid != ignoreId) return false;
        }
        return true;
    }

    public bool CanPlace(Piece piece, Vec2I anchor) => CanPlace(piece.Shape, anchor, piece.Id);

    /// <summary>Сдвиг фигуры (ЛКМ-механика). Возвращает true, если ход выполнен.</summary>
    public bool MovePiece(Piece piece, ShiftDirection dir)
    {
        if (!_piecesById.TryGetValue(piece.Id, out _)) return false;
        Vec2I newPos = piece.Position + ShiftResolver.GetDelta(dir);
        if (!CanPlace(piece.Shape, newPos, piece.Id)) return false;

        ClearOccupancy(piece);
        piece.Position = newPos;
        ApplyOccupancy(piece);
        return true;
    }

    /// <summary>
    /// СПАВН НОВОЙ ФИГУРЫ: ключевое исправление — перебираем все кандидаты и ставим
    /// только туда, где CanPlace == true (фигура целиком в границах и клетки свободны).
    /// Спавн "поперёк/поверх клеток" больше невозможен.
    /// </summary>
    public bool SpawnRandomPiece(PieceType type, out Piece spawned)
    {
        spawned = null;

        // 1. Собираем список свободных anchor-позиций для случайной формы
        PieceShape shape = PieceShape.All[_rng.Next(PieceShape.All.Length)];

        // Считаем сколько вообще мест подходит
        int candidates = 0;
        for (int y = 0; y <= Height - shape.Height; y++)
            for (int x = 0; x <= Width - shape.Width; x++)
                if (CanPlace(shape, new Vec2I(x, y))) candidates++;

        if (candidates == 0) return false; // сетка забита — хода нет

        // 2. Равновероятный выбор среди валидных позиций (Reservoir-style, один проход)
        int pick = _rng.Next(candidates);
        Vec2I chosen = Vec2I.Zero;
        int seen = 0;
        bool found = false;

        for (int y = 0; y <= Height - shape.Height && !found; y++)
        {
            for (int x = 0; x <= Width - shape.Width && !found; x++)
            {
                var anchor = new Vec2I(x, y);
                if (!CanPlace(shape, anchor)) continue;
                if (seen == pick) { chosen = anchor; found = true; }
                seen++;
            }
        }

        var p = new Piece(_nextPieceId++, type, shape, chosen);
        _pieces.Add(p);
        _piecesById[p.Id] = p;
        ApplyOccupancy(p);
        spawned = p;
        return true;
    }

    public void RemovePiece(Piece piece)
    {
        ClearOccupancy(piece);
        _pieces.Remove(piece);
        _piecesById.Remove(piece.Id);
    }

    private void ApplyOccupancy(Piece piece)
    {
        var offs = piece.Shape.Offsets;
        for (int i = 0; i < offs.Count; i++)
        {
            Vec2I c = piece.Position + offs.Array[i];
            _cells[c.Y * Width + c.X].PieceId = piece.Id;
        }
    }

    private void ClearOccupancy(Piece piece)
    {
        var offs = piece.Shape.Offsets;
        for (int i = 0; i < offs.Count; i++)
        {
            Vec2I c = piece.Position + offs.Array[i];
            _cells[c.Y * Width + c.X].PieceId = Cell.NoPiece;
        }
    }

    /// <summary>Конец боя: ни одна фигура не может сдвинуться (boxing устранён — без Enum.GetValues).</summary>
    public bool IsGameOver()
    {
        for (int i = 0; i < _pieces.Count; i++)
        {
            var p = _pieces[i];
            for (int d = 0; d < 4; d++)
                if (CanPlace(p.Shape, p.Position + ShiftResolver.GetDelta((ShiftDirection)d), p.Id))
                    return false;
        }
        return true;
    }
}