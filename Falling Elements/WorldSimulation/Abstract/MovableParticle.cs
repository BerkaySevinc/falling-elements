using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation;

public abstract class MovableParticle : Particle
{
    public abstract float Mass { get; }
    public abstract float CoefficientOfFriction { get; }

    public bool IsFreeFalling { get; private set; }
    public abstract MoveDirection MoveDirection { get; }


    private float frictionForce;
    private Vector2 moveForceVector;
    protected MovableParticle(World world, int gridX, int gridY) : base(world, gridX, gridY)
    {
        Vector2 moveDirectionVector = MoveDirection is MoveDirection.Down ? Vector2.UnitY : -Vector2.UnitY;
        moveForceVector = moveDirectionVector * Mass * world.Gravity;

        frictionForce = CoefficientOfFriction * Math.Abs(moveForceVector.Y);
    }


    private readonly Random random = new();
    protected override void MoveTo(int targetX, int targetY, Action<int, int, IParticle?> iterationCallback, Action<int, int, Vector2, IParticle?>? onCollisionCallback)
    {
        base.MoveTo(targetX, targetY, iterationCallback,

           (pathX, pathY, collisionDirection, collidedParticle) =>
           {
               onCollisionCallback?.Invoke(pathX, pathY, collisionDirection, collidedParticle);

               // Apply collision.
               if (collisionDirection.X is not 0)
               {
                   X = collisionDirection.X > 0 ? pathX + 0.999F : pathX;

                   // Reset horizontal velocity.
                   Velocity *= Vector2.UnitY;
               }
               if (collisionDirection.Y is not 0)
               {
                   // Return if collided to falling particle.
                   if (collidedParticle is MovableParticle movableParticle && movableParticle.IsFreeFalling)
                   {
                       Velocity = new Vector2(Velocity.X, Math.Min(Velocity.Y, collidedParticle.Velocity.Y));
                       return;
                   }

                   Y = collisionDirection.Y > 0 ? pathY + 0.999F : pathY;

                   // Convert it to horizontal velocity.
                   float horizontalVelocity = Velocity.Y / frictionForce;

                   // Set velocity direction to left if particle already has left velocity.
                   // Or set direction randomly if horizontal velocity is zero.
                   if (Velocity.X < 0 || (Velocity.X is 0 && random.Next(2) is 0)) 
                       horizontalVelocity *= -1;

                   Velocity += new Vector2(horizontalVelocity, 0);

                   // Reset vertical velocity.
                   Velocity *= Vector2.UnitX;

                   // Set IsFreeFalling to false if horizontal velocity is zero.
                   IsFreeFalling = false;
               }

               // Set IsUpdating to false if velocity is zero.
               if (Velocity == Vector2.Zero) IsUpdating = false;
           }
       );
    }

    protected (float targetX, float targetY) ApplyGravity(float deltaTime)
    {
        // Set is free falling.
        IsFreeFalling = IsMoveDirectionAvailable();

        // Apply gravity force if move direction is available.
        Vector2 halfGravityForceVector = Vector2.Zero;
        Vector2 halfFrictionForceVector = Vector2.Zero;
        if (IsFreeFalling)
        {
            halfGravityForceVector = moveForceVector * deltaTime / 2;

            Velocity += halfGravityForceVector;
        }
        // Apply friction force if move direction is not available and particle has horizontal velocity.
        else if (Velocity.X != 0)
        {
            float halfFrictionForce = frictionForce * deltaTime / 2;

            // Reset horizontal velocity if friction force is bigger than velocity.
            if (Math.Abs(Velocity.X) < halfFrictionForce)
            {
                Velocity *= Vector2.UnitY;
            }
            // Apply friction force if friction force is smaller than velocity.
            else
            {
                if (Velocity.X > 0) halfFrictionForce *= -1;

                halfFrictionForceVector = new Vector2(halfFrictionForce, 0);
                Velocity += halfFrictionForceVector;
            }
        }

        // Get target location using velocity.
        float targetX = X + Velocity.X * deltaTime;
        float targetY = Y + Velocity.Y * deltaTime;

        // Apply other half gravity force if move direction is available.
        if (IsFreeFalling)
        {
            Velocity += halfGravityForceVector;
        }
        // Apply other half friction force if move direction is not available and particle has x axis velocity.
        else if (Velocity.X != 0)
        {
            // Reset horizontal velocity if friction force is bigger than velocity.
            if (Math.Abs(Velocity.X) < Math.Abs(halfFrictionForceVector.X))
            {
                Velocity *= Vector2.UnitY;
            }

            // Apply friction force if friction force is smaller than velocity.
            else Velocity += halfFrictionForceVector;
        }

        return (targetX, targetY);
    }


    private bool IsMoveDirectionAvailable()
    {
        if (MoveDirection is MoveDirection.Down)
        {
            if (Y - GridY < 0.99) return true;

            if (GridY == world.Bottom) return false;
        }
        else
        {
            if (Y > GridY) return true;

            if (GridY == world.Top) return false;
        }

        // Gets target particle.
        IParticle? targetParticle = GetParticleByLocation(GridX, GridY + (int)MoveDirection);

        bool isParticleMovable = IsParticleMovable(targetParticle);

        return isParticleMovable || targetParticle is MovableParticle movableParticle && movableParticle.IsFreeFalling;
    }
}
