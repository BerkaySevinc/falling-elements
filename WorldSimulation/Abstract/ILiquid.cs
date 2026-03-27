namespace WorldSimulation;


public interface ILiquid : IMovableParticle
{
    public int DispersionRate { get; }
}
