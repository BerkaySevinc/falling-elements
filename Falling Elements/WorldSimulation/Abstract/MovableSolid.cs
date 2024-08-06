using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;



namespace WorldSimulation;

public abstract class MovableSolid : Particle, ISolid, IMovableParticle
{
    public abstract float Mass { get; }
    public Vector2 MoveDirection { get; } = Vector2.UnitY;

    protected MovableSolid(World world) : base(world) { }
    protected MovableSolid(World world, float x, float y) : base(world, x, y) { }
    protected MovableSolid(World world, Point coordinates) : base(world, coordinates) { }



    private static Random random = new();
    public Dictionary<System.Drawing.Point, IParticle?>? Step2(float deltaGravitation)
    {
        // Continues if it is already on the ground.
        if (IsOnGround()) return null;

        // Calculates the remaining move.
        float remainingMove = Mass * deltaGravitation;

        var gridChanges = new Dictionary<System.Drawing.Point, IParticle?>();

        // Checks path and makes move.
        bool positionChanged = false;
        var (currentX, currentY) = (X, Y);
        for (; remainingMove > 0; remainingMove--)
        {
            // Calculates move.
            float move = remainingMove < 1 ? remainingMove : 1;

            // Gets current coordinates info.
            int floorCurrentX = (int)currentX;
            int floorCurrentY = (int)currentY;

            // Gets target coordinates info.
            float targetY = currentY + move;
            int floorTargetY = (int)targetY;

            // Breaks if particle is still on the same cell.
            if (floorTargetY == floorCurrentY)
            {
                // Sets new coordinates.
                X = currentX;
                Y = targetY;
                break;
            }

            // Sets exactly on the ground if particle gets under it.
            if (floorTargetY == world.Bottom)
            {
                targetY = floorTargetY;
                remainingMove = 0;
            }

            bool positionReplaced = false;
            IParticle? replacedParticle = null;

            // Moves down if below cell is available.
            IParticle? belowParticle = world.Grid[floorCurrentX, floorTargetY];
            if (belowParticle is not ISolid)
            {
                positionChanged = true;
                if (belowParticle is not null)
                {
                    positionReplaced = true;
                    replacedParticle = belowParticle;
                }

                // Sets current coordinates..
                currentY = targetY;
            }

            // Checks diagonal cells if below cell is not empty.
            else
            {
                // Checks left diagonal cell.
                IParticle? leftDiagonalParticle = null;
                bool isLeftDiagonalAvailable = false;

                if (floorCurrentX != 0)
                {
                    leftDiagonalParticle = world.Grid[floorCurrentX - 1, floorTargetY];

                    isLeftDiagonalAvailable = leftDiagonalParticle is not ISolid;
                    //&& Grid[floorCurrentX - 1, floorCurrentY] is null;
                }

                // Checks right diagonal cell.
                IParticle? rightDiagonalParticle = null;
                bool isRightDiagonalAvailable = false;

                if (floorCurrentX != world.Right)
                {
                    rightDiagonalParticle = world.Grid[floorCurrentX + 1, floorTargetY];

                    isRightDiagonalAvailable = rightDiagonalParticle is not ISolid;
                    //&& Grid[floorCurrentX + 1, floorTargetY] is null;
                }

                // Moves to available diagonal cell if at least one cell is available.
                if (isLeftDiagonalAvailable || isRightDiagonalAvailable)
                {
                    // Gets diagonal cell direction according to available one or randomly.
                    int direction =
                        isLeftDiagonalAvailable && isRightDiagonalAvailable
                        ? random.Next(2) is 0 ? 1 : -1
                        : isLeftDiagonalAvailable ? -1 : 1;

                    IParticle? targetParticle = direction == 1 ? rightDiagonalParticle : leftDiagonalParticle; ;

                    positionChanged = true;
                    if (targetParticle is not null)
                    {
                        positionReplaced = true;
                        replacedParticle = targetParticle;
                    }

                    // Sets current coordinates and continue loop.
                    currentX += direction;
                    currentY = targetY;
                }
            }

            // Breaks if no cell is available.
            if (!positionChanged) break;

            // Replace particle.
            if (positionReplaced)
            {
                // Sets replaced particle coordinates.
                replacedParticle!.X = currentX;
                replacedParticle.Y = currentY;

                // Moves replaced particle to location in the particle list.
                world.particlesByAltitude[floorTargetY].Remove(replacedParticle);
                world.particlesByAltitude[floorCurrentY].Add(replacedParticle);

                // Adds replaced particle to new cell location and applies to changes.
                gridChanges.Add(new(floorCurrentX, floorCurrentY), replacedParticle);
                world.Grid[floorCurrentX, floorCurrentY] = replacedParticle;
            }
        }

        if (!positionChanged) return gridChanges;

        // Gets particle coordinates info.
        int floorResultX = (int)currentX;
        int floorResultY = (int)currentY;

        // Sets new coordinates.
        X = currentX;
        Y = currentY;

        // Moves to new location in the particle list.
        world.particlesByAltitude[GridY].Remove(this);
        world.particlesByAltitude[floorResultY].Add(this);

        // Removes from old cell location and applies to changes.
        bool isInitialCellCleared = gridChanges.TryAdd(new System.Drawing.Point(GridX, GridY), null);
        if (isInitialCellCleared) world.Grid[GridX, GridY] = null;

        // Adds to new cell location and applies to changes.
        world.Grid[floorResultX, floorResultY] = this;
        gridChanges.Add(new(floorResultX, floorResultY), this);

        return gridChanges;
    }







    public override RenderingUpdates? Step(float deltaTime)
    {
        // Apply half gravity acceleration.
        Vector2 halfGravity = MoveDirection * Mass * world.Gravity * deltaTime / 2;
        Velocity += halfGravity;

        // Get target location using velocity.
        float targetX = X + Velocity.X * deltaTime;
        float targetY = Y + Velocity.Y * deltaTime;

        // Apply other half gravity acceleration.
        Velocity += halfGravity;

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
        SwitchViaPath(targetX, targetY,

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
