namespace SharpWM.Common;

public readonly record struct Rect(int X, int Y, int Width, int Height)
{
    public int Right => X + Width;
    public int Bottom => Y + Height;
    public bool IsEmpty => Width <= 0 || Height <= 0;

    public static Rect Empty => new(0,0,0,0);

    public bool Contains(int x, int y) => x >= X && x < Right && y >= Y && y < Bottom;

    public override string ToString() => $"({X}, {Y}, {Width}x{Height})";
}