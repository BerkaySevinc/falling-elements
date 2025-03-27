using Falling_Elements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;



namespace WorldSimulation;

public abstract class MovableSolid : MovableParticle, IMovableSolid
{
    public override MoveDirection MoveDirection { get; } = MoveDirection.Down;

    public abstract float CoefficientOfFriction { get; }
    public abstract float InertialResistance { get; }


    private float frictionForce;
    protected MovableSolid(World world, int gridX, int gridY) : base(world, gridX, gridY)
    {
        frictionForce = CoefficientOfFriction * Math.Abs(MoveForceVector.Y);
    }


    private Random random = new();
    public override RenderingUpdates? Step(float deltaTime)
    {
        // Apply gravity.
        var (targetX, targetY) = ApplyGravity(deltaTime);

        // If velocity is zero, check inertial resistance.
        if (Velocity == Vector2.Zero)
        {
            // Returns if inertial resistance is enough.
            if (IsOnGround() || random.NextSingle() < InertialResistance)
            {
                IsUpdating = false;
                return null;
            }
            // Drop particle if inertial resistance is not enough.
            else
            {
                bool isLeftDropPossible =
                    !IsOnLeftBound()
                    && IsParticleMovable(GetParticleByLocation(GridX - 1, GridY))
                    && IsParticleMovable(GetParticleByLocation(GridX - 1, GridY + 1));

                bool isRightDropPossible =
                    !IsOnRightBound()
                    && IsParticleMovable(GetParticleByLocation(GridX + 1, GridY))
                    && IsParticleMovable(GetParticleByLocation(GridX + 1, GridY + 1));

                // Returns if drop is not possible.
                if (!isLeftDropPossible && !isRightDropPossible)
                {
                    IsUpdating = false;
                    return null;
                }

                targetX = GridX +
                    (
                    !isLeftDropPossible ? 1
                    : !isRightDropPossible ? -1
                    : random.Next(2) is 0 ? 1 : -1
                    );

                targetY = GridY + 1;
            }
        }

        // If staying at the same cell update coordinates and return.
        if (IsSameCell(targetX, targetY))
        {
            X = targetX;
            Y = targetY;

            return null;
        }

        // Create grid changes to save changes.
        var renderingUpdates = new RenderingUpdates();

        // Itarete to target location.
        MoveTo(targetX, targetY,

            (pathX, pathY, particle) =>
            {
                // Save grid changes.
                renderingUpdates.Add((GridX, GridY), (Color, particle?.Color));
                renderingUpdates.Add((pathX, pathY), (particle?.Color, Color));
            },

            null
        );

        // Return changes.
        return renderingUpdates;
    }

    protected override bool IsParticleMovable(IParticle? particle)
        => particle is not ISolid;

    protected override void MoveTo(int targetX, int targetY, Action<int, int, IParticle?> iterationCallback, Action<int, int, Vector2, IParticle?>? onCollisionCallback)
    {
        Vector2 initialVelocity = Velocity;
        base.MoveTo(targetX, targetY, iterationCallback,

           (pathX, pathY, collisionDirection, collidedParticle) =>
           {
               // If collision vertical
               if (collisionDirection.Y is not 0)
               {
                   if (!IsFreeFalling)
                   {
                       // Convert it to horizontal velocity.
                       float horizontalVelocity = initialVelocity.Y / frictionForce;

                       // Set velocity direction to left if particle already has left velocity.
                       // Or set direction randomly if horizontal velocity is zero.
                       if (Velocity.X < 0 || (Velocity.X is 0 && random.Next(2) is 0))
                           horizontalVelocity *= -1;

                       Velocity += new Vector2(horizontalVelocity, 0);
                   }
               }

               onCollisionCallback?.Invoke(pathX, pathY, collisionDirection, collidedParticle);

               // Set IsUpdating to false if velocity is zero.
               if (Velocity == Vector2.Zero) IsUpdating = false;
           }
       );
    }

    protected override (float targetX, float targetY) ApplyGravity(float deltaTime)
    {
        // Set is free falling.
        IsFreeFalling = IsMoveDirectionAvailable();

        // Apply gravity force if move direction is available.
        Vector2 halfGravityForceVector = Vector2.Zero;
        Vector2 halfFrictionForceVector = Vector2.Zero;
        if (IsFreeFalling)
        {
            halfGravityForceVector = MoveForceVector * deltaTime / 2;

            Velocity += halfGravityForceVector;
        }
        // Apply friction force if move direction is not available and particle has horizontal velocity.
        else if (Velocity.X is not 0)
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
        else if (Velocity.X is not 0)
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
}
