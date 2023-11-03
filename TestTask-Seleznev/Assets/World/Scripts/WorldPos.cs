///<summary> Позиция относительно сетки чанков </summary>
public struct WorldPos
{
    public int X { get; set; }
    public int Y { get; set; }

    public const int CHUNK_SIZE = 20;

    public WorldPos(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static WorldPos operator +(WorldPos p0, WorldPos p1)
    {
        WorldPos pos = new(p0.X + p1.X, p0.Y + p1.Y);
        return pos;
    }

    public static WorldPos operator -(WorldPos p0, WorldPos p1)
    {
        WorldPos pos = new(p0.X - p1.X, p0.Y - p1.Y);
        return pos;
    }
}