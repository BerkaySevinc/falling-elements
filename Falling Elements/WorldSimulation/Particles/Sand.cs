using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation;

public class Sand : MovableSolid
{
    public override Color Color { get; } = Color.SandyBrown;
    public override float Mass { get; } = 40;

    public Sand(World world) : base(world) { }
    public Sand(World world, float x, float y) : base(world, x, y) { }
    public Sand(World world, Point coordinates) : base(world, coordinates) { }
}
