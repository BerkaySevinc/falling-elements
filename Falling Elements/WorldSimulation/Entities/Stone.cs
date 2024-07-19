using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation;

public struct Stone : IImmovableSolid
{
    public Color Color { get; } = Color.Silver;

    public Point Coordinates { get; set; }

    public Stone() { }
    public Stone(Point coordinates) => Coordinates = coordinates;
}
