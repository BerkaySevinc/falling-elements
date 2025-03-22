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
    public override float CoefficientOfFriction { get; } = 0.05F;
    public override float InertialResistance { get; } = 0.1F;


    public Sand(World world, int gridX, int gridY) : base(world, gridX, gridY) { }
}
