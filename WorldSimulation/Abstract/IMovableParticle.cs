using System.Numerics;

namespace WorldSimulation;


public interface IMovableParticle : IParticle
{
    public float Mass { get; }

    public MoveDirection MoveDirection { get; }

    public Vector2 Velocity { get; set; }
}
