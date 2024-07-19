using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation;

public struct Sand : IMovableSolid
{
    public Color Color { get; } = Color.SandyBrown;
    public float Mass { get; } = 15;

    public Point Coordinates { get; set; }

    public Sand() { }
    public Sand(Point coordinates) => Coordinates = coordinates;
}
