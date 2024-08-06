using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation;


public interface IParticle : IDisposable
{
    public Color Color { get; }

    public float X { get; set; }
    public float Y { get; set; }
    public int GridX { get;  }
    public int GridY { get; }

    public Vector2 Velocity { get; }
    public bool IsUpdating { get; set; }


    public RenderingUpdates? Step(float deltaTime);
}
