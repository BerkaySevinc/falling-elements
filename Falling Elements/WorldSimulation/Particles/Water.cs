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
    public override float CoefficientOfFriction { get; } = 1;


    public Water(World world, int gridX, int gridY) : base(world, gridX, gridY) { }
}
