using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace WorldSimulation;

public abstract class Liquid : MovableParticle
{
    public override MoveDirection MoveDirection { get; } = MoveDirection.Down;

    public abstract int DispersionRate { get; }

    private Type type;
    protected Liquid(World world, int gridX, int gridY) : base(world, gridX, gridY)
    {
        type = GetType();
    }

    private Random random = new();
    public override RenderingUpdates? Step(float deltaTime)
    {
        // Apply gravity.
        var (targetX, targetY) = ApplyGravity(deltaTime);

        // Move sides if not falling and velocity is zero.
        if (!IsFreeFalling && Velocity == Vector2.Zero)
        {
            bool isLeftMovable = !IsOnLeftBound() && IsParticleMovable(GetParticleByLocation(GridX - 1, GridY));
            bool isRightMovable = !IsOnRightBound() && IsParticleMovable(GetParticleByLocation(GridX + 1, GridY));

            if (!isLeftMovable && !isRightMovable)
            {
                IsUpdating = false;
                return null;
            }

            int direction =
                !isLeftMovable ? 1
                : !isRightMovable ? -1
                : random.Next(2) is 0 ? 1 : -1;

            Velocity = new Vector2(DispersionRate * direction, 0);
            (targetX, targetY) = ApplyGravity(deltaTime);
        }

        // Move to target location.
        RenderingUpdates? renderingUpdates = MoveTo(targetX, targetY, null, null);

        // Return changes.
        return renderingUpdates;
    }

    protected override RenderingUpdates MoveTo(int targetX, int targetY, Action<int, int, IParticle?>? iterationCallback, Action<int, int, Vector2, IParticle?>? onCollisionCallback)
    {
        return base.MoveTo(targetX, targetY, iterationCallback,

           (pathX, pathY, collisionDirection, collidedParticle) =>
           {
               // If collision vertical
               if (collisionDirection.X is not 0)
               {
                   if (!IsFreeFalling)
                   {
                       int direction = (int)collisionDirection.X * -1;

                       // Stop if cannot move.
                       if (
                       direction is -1 && (IsOnLeftBound() || !IsParticleMovable(GetParticleByLocation(GridX - 1, GridY)))
                       || direction is 1 && (IsOnRightBound() || !IsParticleMovable(GetParticleByLocation(GridX + 1, GridY)))
                       )
                           Velocity *= Vector2.UnitY;

                       // Else change direction.
                       else Velocity = new Vector2(DispersionRate * direction, 0);
                   }
               }

               // If collision horizontal
               else
               {
                   bool isLeftMovable = !IsOnLeftBound() && IsParticleMovable(GetParticleByLocation(GridX - 1, GridY));
                   bool isRightMovable = !IsOnRightBound() && IsParticleMovable(GetParticleByLocation(GridX + 1, GridY));

                   if (isLeftMovable || isRightMovable)
                   {
                       int direction =
                           !isLeftMovable ? 1
                           : !isRightMovable ? -1
                           : random.Next(2) is 0 ? 1 : -1;

                       Velocity = new Vector2(DispersionRate * direction, 0);
                   }
               }

               onCollisionCallback?.Invoke(pathX, pathY, collisionDirection, collidedParticle);

               // Set IsUpdating to false if velocity is zero.
               if (Velocity == Vector2.Zero) IsUpdating = false;
           }
       );
    }
}
