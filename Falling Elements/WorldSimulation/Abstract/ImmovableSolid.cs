using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation;

public abstract class ImmovableSolid : Particle, ISolid
{
    protected ImmovableSolid(World world) : base(world) { }
    protected ImmovableSolid(World world, float x, float y) : base(world, x, y) { }
    protected ImmovableSolid(World world, Point coordinates) : base(world, coordinates) { }

    public override RenderingUpdates? Step(float deltaTime) => null;
}
