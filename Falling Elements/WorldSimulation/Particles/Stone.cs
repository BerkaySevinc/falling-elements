using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation;

public class Stone : ImmovableSolid
{
    public override Color Color { get; } = Color.Silver;

    public Stone(World world) : base(world) { }
    public Stone(World world, float x, float y) : base(world, x, y) { }
    public Stone(World world, Point coordinates) : base(world, coordinates) { }
}
