using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation;

public abstract class ImmovableSolid : Particle, ISolid
{
    protected ImmovableSolid(World world, int gridX, int gridY) : base(world, gridX, gridY) { }

    public override RenderingUpdates? Step(float deltaTime) => null;
}
