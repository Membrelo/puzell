using System;
using System.Runtime.CompilerServices;

public readonly struct Vec2I : IEquatable<Vec2I>
{
    public readonly int X;
    public readonly int Y;

    public Vec2I(int x, int y) { X = x; Y = y; }

    public static readonly Vec2I Zero = new Vec2I(0, 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec2I operator +(Vec2I a, Vec2I b) => new Vec2I(a.X + b.X, a.Y + b.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec2I operator -(Vec2I a, Vec2I b) => new Vec2I(a.X - b.X, a.Y - b.Y);

    public bool Equals(Vec2I other) => X == other.X && Y == other.Y;
    public override bool Equals(object obj) => obj is Vec2I v && Equals(v);
    public override int GetHashCode() => (X * 397) ^ Y;
    public static bool operator ==(Vec2I a, Vec2I b) => a.Equals(b);
    public static bool operator !=(Vec2I a, Vec2I b) => !a.Equals(b);

    public override string ToString() => $"({X}, {Y})";
}