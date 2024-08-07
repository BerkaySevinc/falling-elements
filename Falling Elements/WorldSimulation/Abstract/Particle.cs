using Falling_Elements;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.AxHost;

namespace WorldSimulation;


public abstract class Particle : IParticle
{
    public abstract Color Color { get; }

    private float _x;
    public float X
    {
        get => _x;
        set
        {
            _x = value;
            GridX = (int)value;
        }
    }
    public int GridX { get; private set; }

    private float _y;
    public float Y
    {
        get => _y;
        set
        {
            _y = value;
            GridY = (int)value;
        }
    }
    private int _gridY;
    public int GridY
    {
        get => _gridY;
        private set
        {
            // Returns if same.
            if (_gridY == value) return;

            if (IsUpdating)
            {
                world.updatingParticlesByAltitude[_gridY].Remove(this);
                world.updatingParticlesByAltitude[value].Add(this);
            }

            _gridY = value;
        }
    }

    public Vector2 Velocity { get; protected set; }

    private bool _isUpdating = true;
    public bool IsUpdating
    {
        get => _isUpdating;
        set
        {
            if (value == _isUpdating) return;

            _isUpdating = value;

            if (value) world.updatingParticlesByAltitude[GridY].Add(this);
            else world.updatingParticlesByAltitude[GridY].Remove(this);
        }
    }


    protected World world;
    public Particle(World world, int gridX, int gridY)
    {
        this.world = world;

        _gridY = gridY;

        X = gridX + 0.5F;
        Y = gridY + 0.5F;

        // Adds particle to grid.
        world.Grid[GridX, GridY] = this;

        // Add particle to list.
        world.updatingParticlesByAltitude[GridY].Add(this);

        UpdateParticlesAround();
    }

    public virtual void Dispose()
    {
        world.Grid[GridX, GridY] = null;

        // Remove particle from list.
        world.updatingParticlesByAltitude[GridY].Remove(this);

        UpdateParticlesAround();
    }

    public abstract RenderingUpdates? Step(float deltaTime);


    protected IParticle? GetParticleByLocation(int gridX, int gridY) => world.Grid[gridX, gridY];
    protected IParticle? GetAboveParticle()
    {
        if (GridY == 0) return null;

        return GetParticleByLocation(GridX, GridY - 1);
    }
    protected List<IParticle> GetParticlesAround(int gridX, int gridY)
    {
        var particlesAround = new List<IParticle>();

        bool isOnRightBound = IsOnRightBound(gridX);
        bool isOnLeftBound = IsOnLeftBound(gridX);

        if (!IsOnGround(gridY))
        {
            var belowParticle = GetParticleByLocation(gridX, gridY + 1);
            if (belowParticle is not null) particlesAround.Add(belowParticle);

            if (!isOnLeftBound)
            {
                var belowLeftParticle = GetParticleByLocation(gridX - 1, gridY + 1);
                if (belowLeftParticle is not null) particlesAround.Add(belowLeftParticle);
            }

            if (!isOnRightBound)
            {
                var belowRightParticle = GetParticleByLocation(gridX + 1, gridY + 1);
                if (belowRightParticle is not null) particlesAround.Add(belowRightParticle);
            }
        }

        if (!IsOnCeiling(gridY))
        {
            var aboveParticle = GetParticleByLocation(gridX, gridY - 1);
            if (aboveParticle is not null) particlesAround.Add(aboveParticle);

            if (!isOnLeftBound)
            {
                var aboveLeftParticle = GetParticleByLocation(gridX - 1, gridY - 1);
                if (aboveLeftParticle is not null) particlesAround.Add(aboveLeftParticle);
            }

            if (!isOnRightBound)
            {
                var aboveRightParticle = GetParticleByLocation(gridX + 1, gridY - 1);
                if (aboveRightParticle is not null) particlesAround.Add(aboveRightParticle);
            }
        }

        if (!isOnLeftBound)
        {
            var leftParticle = GetParticleByLocation(gridX - 1, GridY);
            if (leftParticle is not null) particlesAround.Add(leftParticle);
        }

        if (!isOnRightBound)
        {
            var rightParticle = GetParticleByLocation(gridX + 1, GridY);
            if (rightParticle is not null) particlesAround.Add(rightParticle);
        }

        return particlesAround;
    }
    protected List<IParticle> GetParticlesAround() => GetParticlesAround(GridX, GridY);

    protected void UpdateParticlesAround(int gridX, int gridY)
    {
        foreach (var particle in GetParticlesAround(gridX, gridY))
            particle.IsUpdating = true;
    }
    protected void UpdateParticlesAround() => UpdateParticlesAround(GridX, GridY);



    protected bool IsOnGround(int gridY) => gridY == world.Bottom;
    protected bool IsOnGround() => IsOnGround(GridY);

    protected bool IsOnCeiling(int gridY) => gridY == world.Top;
    protected bool IsOnCeiling() => IsOnCeiling(GridY);

    protected bool IsOnLeftBound(int gridX) => gridX == world.Left;
    protected bool IsOnLeftBound() => IsOnLeftBound(GridX);

    protected bool IsOnRightBound(int gridX) => gridX == world.Right;
    protected bool IsOnRightBound() => IsOnRightBound(GridX);

    protected bool IsOnWorld(int gridX, int gridY) => gridX >= world.Left && gridY >= world.Top && gridX <= world.Right && gridY <= world.Bottom;


    protected bool IsSameCell(int gridX, int gridY) => GridX == gridX && GridY == gridY;
    protected bool IsSameCell(float x, float y) => IsSameCell((int)x, (int)y);

    protected virtual bool IsParticleMovable(IParticle? particle) => particle is null;


    private void SwitchDirectly(int targetX, int targetY)
    {
        if (IsSameCell(targetX, targetY)) return;

        // Gets target particle.
        IParticle? targetParticle = GetParticleByLocation(targetX, targetY);

        // Switch particles on grid.
        world.Grid[targetX, targetY] = this;
        world.Grid[GridX, GridY] = targetParticle;

        if (targetParticle is not null)
        {
            // Set target particles new location.
            targetParticle.X = X;
            targetParticle.Y = Y;
        }

        UpdateParticlesAround(GridX, GridY);
        UpdateParticlesAround(targetX, targetY);

        // Set new location.
        X = targetX + 0.5F;
        Y = targetY + 0.5F;

    }

    protected virtual void MoveTo(int targetX, int targetY, Action<int, int, IParticle?> iterationCallback, Action<int, int, Vector2, IParticle?>? onCollisionCallback)
    {
        IParticle? collidedParticle = null;

        IterateAndApplyToTargetCell(targetX, targetY,
            (pathX, pathY) =>
            {
                IParticle? targetParticle = GetParticleByLocation(pathX, pathY);

                // Checks if target particle movable.
                if (!IsParticleMovable(targetParticle))
                {
                    collidedParticle = targetParticle;
                    return false;
                }

                iterationCallback?.Invoke(pathX, pathY, targetParticle);

                // Switch particles on grid.
                world.Grid[pathX, pathY] = this;
                world.Grid[GridX, GridY] = targetParticle;

                if (targetParticle is not null)
                {
                    // Set target particles new location.
                    targetParticle.X = GridX + 0.5F;
                    targetParticle.Y = GridY + 0.5F;
                }

                UpdateParticlesAround(GridX, GridY);
                UpdateParticlesAround(pathX, pathY);

                // Set new location.
                X = pathX + 0.5F;
                Y = pathY + 0.5F;

                return true;
            },

            (gridX, gridY, collisionDirection)
                => onCollisionCallback?.Invoke(gridX, gridY, collisionDirection, collidedParticle)
        );
    }
    protected void MoveTo(float targetX, float targetY, Action<int, int, IParticle?> iterationCallback, Action<int, int, Vector2, IParticle?>? onCollisionCallback)
    {
        bool isCollided = false;

        MoveTo((int)targetX, (int)targetY, iterationCallback,
            (gridX, gridY, collisionDirection, collidedParticle) =>
            {
                isCollided = true;
                onCollisionCallback?.Invoke(gridX, gridY, collisionDirection, collidedParticle);
            });

        // Make floating number digits equal.
        if (!isCollided)
        {
            X = GridX + (targetX - (int)targetX);
            Y = GridY + (targetY - (int)targetY);
        }
    }

    private void IterateAndApplyToTargetCellInBothAxis(int targetX, int targetY, Func<int, int, bool> iterationCallback, Action<int, int, Vector2>? onCollisionCallback)
    {
        // Gets current location.
        int startX = GridX;
        int startY = GridY;

        // Calculates diffs.
        int xDiff = targetX - startX;
        int yDiff = targetY - startY;

        // Gets which is larger.
        bool isYDiffIsLarger = Math.Abs(xDiff) > Math.Abs(yDiff);

        // Get longer & shorter sides.
        (int longerSide, int shorterSide) = isYDiffIsLarger ? (xDiff, yDiff) : (yDiff, xDiff);

        // Calculates slope.
        float slope = (float)shorterSide / longerSide;


        Vector2 collisionDirection = Vector2.Zero;

        int longerSideAbs = Math.Abs(longerSide);
        int longerSideModifier = longerSide > 0 ? 1 : -1;

        int currentX = startX, currentY = startY;
        for (int i = 1; i <= longerSideAbs; i++)
        {
            int longerSideIncrease = i * longerSideModifier;
            int shorterSideIncrease = (int)Math.Round(i * slope, MidpointRounding.AwayFromZero);

            int newX, newY;
            if (isYDiffIsLarger)
            {
                newX = startX + longerSideIncrease;
                newY = startY + shorterSideIncrease;
            }
            else
            {
                newY = startY + longerSideIncrease;
                newX = startX + shorterSideIncrease;
            }

            // Edit if target is outside of the world.
            bool isOutOfWorld = false;
            if (newX < world.Left)
            {
                isOutOfWorld = true;
                collisionDirection = new Vector2(-1, 0);
            }
            else if (newX >= world.Width)
            {
                isOutOfWorld = true;
                collisionDirection = new Vector2(1, 0);
            }
            if (newY < world.Top)
            {
                isOutOfWorld = true;
                collisionDirection += new Vector2(0, -1);
            }
            else if (newY >= world.Height)
            {
                isOutOfWorld = true;
                collisionDirection += new Vector2(0, 1);
            }

            // Invoke iterationCallback if not collided
            bool isCollided = isOutOfWorld;
            if (!isOutOfWorld) isCollided = !iterationCallback.Invoke(newX, newY);

            // Invoke onCollisionCallback if collided.
            if (isCollided)
            {
                if (onCollisionCallback is null) return;

                if (!isOutOfWorld)
                {
                    int shorterSideDirection = 0;
                    if (currentX != newX) shorterSideDirection = slope is 0 ? 0 : slope > 0 ? 1 : -1;

                    collisionDirection = isYDiffIsLarger ? new Vector2(longerSideModifier, shorterSideDirection) : new Vector2(shorterSideDirection, longerSideModifier);
                }

                onCollisionCallback.Invoke(currentX, currentY, collisionDirection);

                return;
            };

            currentX = newX;
            currentY = newY;
        }
    }

    private void IterateAndApplyToTargetCellInXAxis(int target, Func<int, bool> iterationCallback, Action<int>? onCollisionCallback)
    {
        // Gets current location.
        int start = GridX;

        // Calculates diff.
        // Edit if target is outside of the world.
        int diff;
        bool isCollisionOccurring;

        if (target < world.Left)
        {
            diff = -start;
            isCollisionOccurring = true;
        }
        else if (target > world.Right)
        {
            diff = (world.Right) - start;
            isCollisionOccurring = true;
        }
        else
        {
            diff = target - start;
            isCollisionOccurring = false;
        }

        // Loop through path.
        int current = start;
        int modifier = diff > 0 ? 1 : -1;
        int diffAbs = Math.Abs(diff);
        for (int i = 1; i <= diffAbs; i++)
        {
            int newLocation = start + i * modifier;

            bool isCollided = !iterationCallback.Invoke(newLocation);
            if (isCollided)
            {
                isCollisionOccurring = true;
                break;
            };

            current = newLocation;
        }

        // Invoke onCollisionCallback if collided.
        if (isCollisionOccurring) onCollisionCallback?.Invoke(current);
    }

    private void IterateAndApplyToTargetCellInYAxis(int target, Func<int, bool> iterationCallback, Action<int>? onCollisionCallback)
    {
        // Gets current location.
        int start = GridY;

        // Calculates diff.
        // Edit if target is outside of the world.
        int diff;
        bool isCollisionOccurring;

        if (target < world.Top)
        {
            diff = -start;
            isCollisionOccurring = true;
        }
        else if (target > world.Bottom)
        {
            diff = (world.Bottom) - start;
            isCollisionOccurring = true;
        }
        else
        {
            diff = target - start;
            isCollisionOccurring = false;
        }

        // Loop through path.
        int current = start;
        int modifier = diff > 0 ? 1 : -1;
        int diffAbs = Math.Abs(diff);
        for (int i = 1; i <= diffAbs; i++)
        {
            int newLocation = start + i * modifier;

            bool isCollided = !iterationCallback.Invoke(newLocation);
            if (isCollided)
            {
                isCollisionOccurring = true;
                break;
            };

            current = newLocation;
        }

        // Invoke onCollisionCallback if collided.
        if (isCollisionOccurring) onCollisionCallback?.Invoke(current);
    }



    private void IterateAndApplyToTargetCell(int targetX, int targetY, Func<int, int, bool> iterationCallback, Action<int, int, Vector2>? onCollisionCallback)
    {
        bool isXSame = GridX == targetX;
        bool isYSame = GridY == targetY;

        if (isXSame && isYSame) return;

        if (isXSame && !isYSame)
        {
            var collisionDirection = new Vector2(0, targetY - GridY > 0 ? 1 : -1);

            IterateAndApplyToTargetCellInYAxis(
                targetY,
                path => iterationCallback.Invoke(GridX, path),
                path => onCollisionCallback?.Invoke(GridX, path, collisionDirection)
                );
        }
        else if (!isXSame && isYSame)
        {
            var collisionDirection = new Vector2(targetX - GridX > 0 ? 1 : -1, 0);

            IterateAndApplyToTargetCellInXAxis(
                targetX,
                path => iterationCallback.Invoke(path, GridY),
                path => onCollisionCallback?.Invoke(path, GridY, collisionDirection)
                );
        }

        else IterateAndApplyToTargetCellInBothAxis(targetX, targetY, iterationCallback, onCollisionCallback);
    }
}
