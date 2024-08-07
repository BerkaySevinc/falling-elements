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

    protected MovableSolid(World world, int gridX, int gridY) : base(world, gridX, gridY) { }


    public override RenderingUpdates? Step(float deltaTime)
    {
        var (targetX, targetY) = ApplyGravity(deltaTime);

        // Returns if velocity is zero.
        if (Velocity == Vector2.Zero)
        {
            IsUpdating = false;
            return null;
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
