using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation;

public class Stone : ImmovableSolid
{
    public override Color Color { get; } = Color.Silver;

    public Stone(World world, int gridX, int gridY) : base(world, gridX, gridY) { }
}
