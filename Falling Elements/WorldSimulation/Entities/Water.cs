using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation;

public struct Water : ILiquid
{
    public Color Color { get; } = Color.DeepSkyBlue;
    public float Mass { get; } = 10;

    public Point Coordinates { get; set; }

    public Water() { }
    public Water(Point coordinates) => Coordinates = coordinates;
}
