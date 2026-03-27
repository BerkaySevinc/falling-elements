namespace WorldSimulation;


public interface IMovableSolid : IParticle, ISolid
{
    public float CoefficientOfFriction { get; }
}
