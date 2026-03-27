using System.Runtime.InteropServices;

namespace WorldSimulation;

[StructLayout(LayoutKind.Sequential)]
public struct Point
{
    public float X, Y;

    public Point(int x, int y) => (X, Y) = (x, y);
    public Point(float x, float y) => (X, Y) = (x, y);

    public void Deconstruct(out float x, out float y) => (x, y) = (X, Y);
    public (int X, int Y) Floor() => ((int)X, (int)Y);
}
