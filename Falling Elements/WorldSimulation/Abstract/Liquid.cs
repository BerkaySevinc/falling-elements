using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation;

public abstract class Liquid : Particle, IMovableParticle
{
    public abstract float Mass { get; }
    public Vector2 MoveDirection { get; } = Vector2.UnitY;


    protected Liquid(World world) : base(world) { }
    protected Liquid(World world, float x, float y) : base(world, x, y) { }
    protected Liquid(World world, Point coordinates) : base(world, coordinates) { }

    public override RenderingUpdates? Step(float deltaTime) => null;
}
