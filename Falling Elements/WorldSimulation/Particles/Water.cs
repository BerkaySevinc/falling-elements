using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation;

public class Water : Liquid
{
    public override Color Color { get; } = Color.DeepSkyBlue;
    public override float Mass { get; } = 30;

    public Water(World world) : base(world) { }
    public Water(World world, float x, float y) : base(world, x, y) { }
    public Water(World world, Point coordinates) : base(world, coordinates) { }
}
