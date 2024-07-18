using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WorldSimulation;

[StructLayout(LayoutKind.Sequential)]
public struct Point
{
    public float X, Y;

    public Point(int x, int y) => (X, Y) = (x, y);
    public Point(float x, float y) => (X, Y) = (x, y);
    public Point(System.Drawing.Point pt) : this(pt.X, pt.Y) { }
    

    public static explicit operator System.Drawing.Point(Point p) => new System.Drawing.Point((int)p.X, (int)p.Y);

    public static implicit operator Point(System.Drawing.Point p) => new Point(p.X, p.Y);

    public void Deconstruct(out float x, out float y) => (x, y) = (X, Y);

    public (int X, int Y) Floor() => ((int)X, (int)Y);

}

