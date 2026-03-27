using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation;


public interface IMovableSolid : IParticle, ISolid
{
    public float CoefficientOfFriction { get; }
}
