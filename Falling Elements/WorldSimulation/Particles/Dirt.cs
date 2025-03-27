using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation;

public class Dirt : MovableSolid
{
    public override Color Color { get; protected set; } = Color.SaddleBrown;
    protected override float ColorShiftFactor { get; } = 0.2F;

    public override float Mass { get; } = 40;
    public override float CoefficientOfFriction { get; } = 0.1F;
    public override float InertialResistance { get; } = 0.6F;


    public Dirt(World world, int gridX, int gridY) : base(world, gridX, gridY) { }
}
