using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation;

public abstract class MovableParticle : Particle, IMovableParticle
{
    public abstract float Mass { get; }

    public abstract MoveDirection MoveDirection { get; }

    public Vector2 Velocity { get; set; }

    public bool IsFreeFalling { get; protected set; }

    protected Vector2 MoveForceVector { get; }

    protected MovableParticle(World world, int gridX, int gridY) : base(world, gridX, gridY)
    {
        Vector2 moveDirectionVector = MoveDirection is MoveDirection.Down ? Vector2.UnitY : -Vector2.UnitY;
        MoveForceVector = moveDirectionVector * Mass * world.Gravity;
    }


    private readonly Random random = new();
    protected override RenderingUpdates MoveTo(int targetX, int targetY, Action<int, int, IParticle?>? iterationCallback, Action<int, int, Vector2, IParticle?>? onCollisionCallback)
    {
        return base.MoveTo(targetX, targetY, iterationCallback,

           (pathX, pathY, collisionDirection, collidedParticle) =>
           {
               // If collision horizontal
               if (collisionDirection.X is not 0)
               {
                   X = collisionDirection.X > 0 ? pathX + 0.999F : pathX;

                   // Reset horizontal velocity.
                   Velocity *= Vector2.UnitY;
               }

               // If collision vertical
               else
               {
                   Y = collisionDirection.Y > 0 ? pathY + 0.999F : pathY;

                   // Return if collided to falling particle.
                   if (collidedParticle is MovableParticle movableParticle && movableParticle.IsFreeFalling)
                   {
                       Velocity = new Vector2(Velocity.X, Math.Min(Velocity.Y, movableParticle.Velocity.Y));
                       return;
                   }

                   Velocity *= Vector2.UnitX;

                   // Set IsFreeFalling to false.
                   IsFreeFalling = false;
               }

               onCollisionCallback?.Invoke(pathX, pathY, collisionDirection, collidedParticle);
           }
       );
    }

    protected virtual (float targetX, float targetY) ApplyGravity(float deltaTime)
    {
        // Set is free falling.
        IsFreeFalling = IsMoveDirectionAvailable();

        Vector2 halfGravityForceVector = Vector2.Zero;
        if (IsFreeFalling)
        {
            // Apply gravity force if move direction is available.
            halfGravityForceVector = MoveForceVector * deltaTime / 2;

            Velocity += halfGravityForceVector;
        }

        // Get target location using velocity.
        float targetX = X + Velocity.X * deltaTime;
        float targetY = Y + Velocity.Y * deltaTime;

        // Apply other half gravity force if move direction is available.
        if (IsFreeFalling) Velocity += halfGravityForceVector;

        return (targetX, targetY);
    }


    protected bool IsMoveDirectionAvailable()
    {
        if (MoveDirection is MoveDirection.Down)
        {
            if (Y - GridY < 0.99)
                return true;

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
