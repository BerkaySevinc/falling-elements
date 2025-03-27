using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation;

public class Water : Liquid
{
    public override Color Color { get; protected set; } = Color.FromArgb(50, 205, 255);
    protected override float ColorShiftFactor { get; } = 0.05F;

    public override float Mass { get; } = 30;

    public override int DispersionRate { get; } = 50;

    public Water(World world, int gridX, int gridY) : base(world, gridX, gridY) { }
}
