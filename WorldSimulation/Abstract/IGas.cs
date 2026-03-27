namespace WorldSimulation;

public abstract class Gas : MovableParticle
{
    public override MoveDirection MoveDirection { get; } = MoveDirection.Up;


    protected Gas(World world, int gridX, int gridY) : base(world, gridX, gridY) { }

    public override RenderingUpdates? Step(float deltaTime) => null;
}
