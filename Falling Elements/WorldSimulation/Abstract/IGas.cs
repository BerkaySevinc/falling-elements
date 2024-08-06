using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation;

public abstract class Gas : Particle, IMovableParticle
{
    public abstract float Mass { get; }
    public Vector2 MoveDirection { get; } = -Vector2.UnitY;


    protected Gas(World world) : base(world) { }
    protected Gas(World world, float x, float y) : base(world, x, y) { }
    protected Gas(World world, Point coordinates) : base(world, coordinates) { }

    public override RenderingUpdates? Step(float deltaTime) => null;
}
