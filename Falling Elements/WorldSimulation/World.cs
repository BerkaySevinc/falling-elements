using Falling_Elements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;




namespace WorldSimulation;

public class World
{
    public int Width { get; }
    public int Height { get; }
    public IParticle?[,] Grid { get; }

    public float Gravity { get; set; } = 10;

    public IEnumerable<IParticle> Particles => particlesByAltitude.SelectMany(l => l);
    public int ParticlesOnGroundCount => particlesByAltitude[Height - 1].Count;
    public int ParticleCount => particlesByAltitude.Sum(l => l.Count);



    public World(int width, int height)
    {
        (Width, Height) = (width, height);

        Grid = new IParticle?[Width, Height];

        // Creates particles by altitude list.
        particlesByAltitude = new List<IParticle>[height];

        for (int i = 0; i < Height; i++)
            particlesByAltitude[i] = new List<IParticle>();
    }
    public World(Size size) : this(size.Width, size.Height) { }
    public World(int size) : this(size, size) { }


    List<IParticle> createdParticles = new();
    public void AddParticle<T>(System.Drawing.Point location, int radius) where T : struct, IParticle
    {
        if (radius <= 0) return;

        radius--;

        int left = location.X - radius;
        int right = location.X + radius;
        int top = location.Y - radius;
        int bottom = location.Y + radius;

        double radiusSquare = Math.Pow(radius, 2);

        for (int floorY = bottom; floorY >= top; floorY--)
            for (int floorX = right; floorX >= left; floorX--)
            {
                double dist = Math.Pow(location.X - floorX, 2) + Math.Pow(location.Y - floorY, 2);
                if (dist > radiusSquare) continue;

                // Returns if outsite of the bounds.
                if (floorX < 0 || floorX >= Width) continue;
                if (floorY < 0 || floorY >= Height) continue;

                // Creates particle.
                var particle = new T
                {
                    Coordinates = new Point(floorX, floorY)
                };

                // Adds to newly created particle list.
                lock (createdParticles)
                    createdParticles.Add(particle);
            }
    }


    Random random = new();
    Stopwatch deltaTimer = new();
    List<IParticle>[] particlesByAltitude;
    public Dictionary<System.Drawing.Point, IParticle?> Update()
    {
        // Calculates delta time.
        var deltaTime = (float)deltaTimer.Elapsed.TotalSeconds;
        var deltaGravitation = Gravity * deltaTime;
        deltaTimer.Restart();

        var gridChanges = new Dictionary<System.Drawing.Point, IParticle?>();

        // Loops through all altitudes.
        for (int floorY = Height - 1; floorY >= 0; floorY--)
        {
            var altitudeParticleList = particlesByAltitude[floorY];

            // Loops through all particles at the altitude.
            for (int i = altitudeParticleList.Count - 1; i >= 0; i--)
            {
                // Gets particle.
                var particle = altitudeParticleList[i];

                // Checks if particle is immovable solid.
                if (particle is IImmovableSolid) continue;

                // Gets particle coordinates info.
                var (x, y) = particle.Coordinates;
                var floorX = (int)x;

                // Checks if particle is movable solid.
                if (particle is IMovableParticle movableParticle)
                {
                    // Continues if it is already on the ground.
                    if (floorY == Height - 1) continue;

                    // Calculates the remaining move.
                    float remainingMove = movableParticle.Mass * deltaGravitation;

                    // Gets is particle solid.
                    bool isParticleSolid = particle is ISolid;

                    // Checks path and makes move.
                    bool positionChanged = false;
                    Point currentCoordinates = movableParticle.Coordinates;
                    for (; remainingMove > 0; remainingMove--)
                    {
                        // Calculates move.
                        float move = remainingMove < 1 ? remainingMove : 1;

                        // Gets current coordinates info.
                        var (currentX, currentY) = currentCoordinates;
                        var (floorCurrentX, floorCurrentY) = currentCoordinates.Floor();

                        // Gets target coordinates info.
                        float targetY = currentY + move;
                        int floorTargetY = (int)targetY;

                        // Breaks if particle is still on the same cell.
                        if (floorTargetY == floorCurrentY)
                        {
                            // Sets new coordinates.
                            movableParticle.Coordinates = new Point(currentX, targetY);
                            break;
                        }

                        // Sets exactly on the ground if particle gets under it.
                        if (floorTargetY == Height - 1)
                        {
                            targetY = floorTargetY;
                            remainingMove = 0;
                        }

                        bool positionReplaced = false;
                        IParticle? replacedParticle = null;

                        // Moves down if below cell is available.
                        IParticle? belowParticle = Grid[floorCurrentX, floorTargetY];
                        if (belowParticle is null || (isParticleSolid && belowParticle is not ISolid))
                        {
                            positionChanged = true;
                            if (belowParticle is not null)
                            {
                                positionReplaced = true;
                                replacedParticle = belowParticle;
                            }

                            // Sets current coordinates and continue loop.
                            currentCoordinates = new Point(currentX, targetY);
                        }

                        // Checks diagonal cells if below cell is not empty.
                        else
                        {
                            // Checks left diagonal cell.
                            IParticle? leftDiagonalParticle = null;
                            bool isLeftDiagonalAvailable = false;

                            if (floorCurrentX != 0)
                            {
                                leftDiagonalParticle = Grid[floorCurrentX - 1, floorTargetY];

                                isLeftDiagonalAvailable =
                                    leftDiagonalParticle is null
                                    || (isParticleSolid && leftDiagonalParticle is not ISolid);
                                //&& Grid[floorCurrentX - 1, floorCurrentY] is null;
                            }

                            // Checks right diagonal cell.
                            IParticle? rightDiagonalParticle = null;
                            bool isRightDiagonalAvailable = false;

                            if (floorCurrentX != Width - 1)
                            {
                                rightDiagonalParticle = Grid[floorCurrentX + 1, floorTargetY];

                                isRightDiagonalAvailable =
                                    rightDiagonalParticle is null
                                    || (isParticleSolid && rightDiagonalParticle is not ISolid);
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
                                currentCoordinates = new Point(currentX + direction, targetY);
                            }

                            // Moves to side cells if diagonal cells are not available and particle is liquid.
                            else if (movableParticle is ILiquid)
                            {
                                // Checks side cells.
                                bool isLeftAvailable =
                                    floorCurrentX != 0
                                    && Grid[floorCurrentX - 1, floorCurrentY] is null;

                                bool isRightAvailable =
                                    floorCurrentX != Width - 1
                                    && Grid[floorCurrentX + 1, floorCurrentY] is null;

                                // Moves to available side cell if at least one is available.
                                if (isLeftAvailable || isRightAvailable)
                                {
                                    // Gets direction according to available one or randomly.
                                    int direction =
                                        isLeftAvailable && isRightAvailable
                                        ? random.Next(2) is 0 ? 1 : -1
                                        : isLeftAvailable ? -1 : 1;

                                    positionChanged = true;
                                    
                                    // Sets current coordinates and continue loop.
                                    currentCoordinates = new Point(currentX + direction, currentY);

                                    continue;
                                }
                            }
                        }

                        // Breaks if no cell is available.
                        if (!positionChanged) break;

                        // Replace particle.
                        if (positionReplaced)
                        {
                            // Sets replaced particle coordinates.
                            replacedParticle!.Coordinates = new Point(currentX, currentY);

                            // Moves replaced particle to location in the particle list.
                            particlesByAltitude[floorTargetY].Remove(replacedParticle);
                            particlesByAltitude[floorCurrentY].Add(replacedParticle);

                            // Adds replaced particle to new cell location and applies to changes.
                            gridChanges[new System.Drawing.Point(floorCurrentX, floorCurrentY)] = replacedParticle;
                            Grid[floorCurrentX, floorCurrentY] = replacedParticle;
                        }
                    }

                    if (!positionChanged) continue;

                    // Gets particle coordinates info.
                    var (floorResultX, floorResultY) = currentCoordinates.Floor();

                    // Sets new coordinates.
                    movableParticle.Coordinates = currentCoordinates;

                    // Moves to new location in the particle list.
                    altitudeParticleList.RemoveAt(i);
                    particlesByAltitude[floorResultY].Add(particle);

                    // Removes from old cell location and applies to changes.
                    bool isInitialCellCleared = gridChanges.TryAdd(new System.Drawing.Point(floorX, floorY), null);
                    if (isInitialCellCleared) Grid[floorX, floorY] = null;

                    // Adds to new cell location and applies to changes.
                    Grid[floorResultX, floorResultY] = movableParticle;
                    gridChanges[new System.Drawing.Point(floorResultX, floorResultY)] = movableParticle;
                }
            }
        }

        // Adds newly created particles to the grid and changes.
        lock (createdParticles)
            if (createdParticles.Count > 0)
            {
                foreach (var particle in createdParticles)
                {
                    // Gets particle coordinates info.
                    var (floorX, floorY) = particle.Coordinates.Floor();
                    var location = new System.Drawing.Point(floorX, floorY);

                    var altitudeParticleList = particlesByAltitude[floorY];

                    // Checks coordinates if particle exists.
                    int index = altitudeParticleList.FindIndex(p => (int)p.Coordinates.X == floorX && (int)p.Coordinates.Y == floorY);
                    if (index != -1)
                    {
                        var existingParticle = altitudeParticleList[index];

                        // Modify coordinates if the particle type is same.
                        if (existingParticle.GetType() == particle.GetType())
                        {
                            existingParticle.Coordinates = location;
                            continue;
                        }
                        // Remove existing particle from list.
                        else altitudeParticleList.RemoveAt(index);
                    }

                    // Adds particle to list.
                    altitudeParticleList.Add(particle);

                    // Adds particle to grid.
                    Grid[floorX, floorY] = particle;

                    // Adds particle to grid changes.
                    gridChanges[location] = particle;
                }

                createdParticles.Clear();
            }

        // Returns all changes.
        return gridChanges;
    }


}

