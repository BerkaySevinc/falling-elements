using Falling_Elements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;



namespace WorldSimulation;

public abstract class MovableSolid : MovableParticle, ISolid
{
    public override MoveDirection MoveDirection { get; } = MoveDirection.Down;

    public abstract float InertialResistance { get; }


    protected MovableSolid(World world, int gridX, int gridY) : base(world, gridX, gridY) { }


    private Random random = new();
    public override RenderingUpdates? Step(float deltaTime)
    {
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
                    : random.Next(2) is 0 ? 1 : 1
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
}
