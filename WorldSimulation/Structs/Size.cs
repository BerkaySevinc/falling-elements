namespace WorldSimulation;

public readonly struct Size
{
    public int Width { get; }
    public int Height { get; }

    public Size(int width, int height) => (Width, Height) = (width, height);
    public Size(int size) => (Width, Height) = (size, size);
}
