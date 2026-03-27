namespace WorldSimulation;

public readonly struct Color : IEquatable<Color>
{
    public byte A { get; }
    public byte R { get; }
    public byte G { get; }
    public byte B { get; }

    public Color(byte r, byte g, byte b) => (A, R, G, B) = (255, r, g, b);
    public Color(byte a, byte r, byte g, byte b) => (A, R, G, B) = (a, r, g, b);

    public static Color FromArgb(int r, int g, int b) => new((byte)r, (byte)g, (byte)b);
    public static Color FromArgb(int a, int r, int g, int b) => new((byte)a, (byte)r, (byte)g, (byte)b);

public bool Equals(Color other) => A == other.A && R == other.R && G == other.G && B == other.B;
    public override bool Equals(object? obj) => obj is Color c && Equals(c);
    public override int GetHashCode() => HashCode.Combine(A, R, G, B);
    public static bool operator ==(Color left, Color right) => left.Equals(right);
    public static bool operator !=(Color left, Color right) => !left.Equals(right);
}
