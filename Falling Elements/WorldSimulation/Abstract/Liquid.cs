using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation;

public abstract class Liquid : MovableParticle
{
    public override MoveDirection MoveDirection { get; } = MoveDirection.Down;


    protected Liquid(World world, int gridX, int gridY) : base(world, gridX, gridY) { }

    public override RenderingUpdates? Step(float deltaTime) => null;
}
