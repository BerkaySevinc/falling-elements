using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation;


public interface IMovableParticle : IParticle
{
    public float Mass { get; }
    public float CoefficientOfFriction { get; }

    public MoveDirection MoveDirection { get; }

    public Vector2 Velocity { get; set; }

    //public bool IsMoving { get; }

    //public bool IsCollidingBottom { get; }
    //public bool IsCollidingTop { get; }
    //public bool IsCollidingRight { get; }
    //public bool IsCollidingLeft { get; }

    //public bool IsBelowStatic { get; set; }
    //public bool IsAboveStatic { get; set; }
    //public bool IsRightStatic { get; set; }
    //public bool IsLeftStatic { get; set; }

    //public bool IsMoveDirectionStatic { get; }
}
