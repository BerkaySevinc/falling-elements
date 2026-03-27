using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation;

public abstract class Gas : MovableParticle
{
    public override MoveDirection MoveDirection { get; } = MoveDirection.Up;


    protected Gas(World world, int gridX, int gridY) : base(world, gridX, gridY) { }

    public override RenderingUpdates? Step(float deltaTime) => null;
}
