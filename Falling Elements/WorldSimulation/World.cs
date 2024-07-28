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

                // Returns if particle already exists in the pixel.
                if (Grid[floorX, floorY]?.GetType() == typeof(T)) continue;

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
        deltaTimer.Restart();

        var gridChanges = new Dictionary<System.Drawing.Point, IParticle?>();

        // Adds newly created particles to changes.
        lock (createdParticles)
            if (createdParticles.Count > 0)
            {
                foreach (var particle in createdParticles)
                {
                    // Gets particle coordinates info.
                    var (floorX, floorY) = particle.Coordinates.Floor();

                    var altitudeParticleList = particlesByAltitude[floorY];

                    // Checks coordinates and removes particle if already exists there.
                    int index = altitudeParticleList.FindIndex(p => (int)p.Coordinates.X == floorX && (int)p.Coordinates.Y == floorY);
                    if (index != -1) altitudeParticleList.RemoveAt(index);

                    // Adds to particle list.
                    altitudeParticleList.Add(particle);

                    // Adds particle to grid.
                    Grid[floorX, floorY] = particle;

                    // Adds to changes.
                    gridChanges[(System.Drawing.Point)particle.Coordinates] = particle;
                }
                createdParticles.Clear();
            }

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
                    var remainingMove = movableParticle.Mass * Gravity * deltaTime;

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

                        // Moves down if below cell is empty.
                        if (Grid[floorCurrentX, floorTargetY] is null)
                        {
                            // Sets current coordinates and continue loop.
                            currentCoordinates = new Point(currentX, targetY);
                            positionChanged = true;
                            continue;
                        }

                        // Checks diagonal cells if below cell is not empty.
                        else
                        {
                            // Checks diagonal cells.
                            bool isLeftDiagonalAvailable =
                                floorCurrentX != 0
                                && Grid[floorCurrentX - 1, floorTargetY] is null;
                            //&& Grid[floorCurrentX - 1, floorCurrentY] is null;

                            bool isRightDiagonalAvailable =
                                floorCurrentX != Width - 1
                                && Grid[floorCurrentX + 1, floorTargetY] is null;
                            //&& Grid[floorCurrentX + 1, floorTargetY] is null;

                            // Moves to available diagonal cell if at least one cell is available.
                            if (isLeftDiagonalAvailable || isRightDiagonalAvailable)
                            {
                                // Gets diagonal cell direction according to available one or randomly.
                                int direction =
                                    isLeftDiagonalAvailable && isRightDiagonalAvailable
                                    ? random.Next(2) is 0 ? 1 : -1
                                    : isLeftDiagonalAvailable ? -1 : 1;

                                // Sets current coordinates and continue loop.
                                currentCoordinates = new Point(currentX + direction, targetY);
                                positionChanged = true;

                                continue;
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

                                    // Sets current coordinates and continue loop.
                                    currentCoordinates = new Point(currentX + direction, currentY);
                                    positionChanged = true;

                                    continue;
                                }
                            }

                            // Breaks if no cell is available.
                            else break;
                        }
                    }

                    if (!positionChanged) continue;

                    // Gets particle coordinates info.
                    var (floorResultX, floorResultY) = currentCoordinates.Floor();

                    // Moves to new location in the particle list.
                    altitudeParticleList.RemoveAt(i);
                    particlesByAltitude[floorResultY].Add(particle);

                    // Sets new coordinates.
                    movableParticle.Coordinates = currentCoordinates;

                    // Removes from old cell location and applies to changes.
                    Grid[floorX, floorY] = null;
                    gridChanges[new System.Drawing.Point(floorX, floorY)] = null;

                    // Adds to new cell location and applies to changes.
                    gridChanges[new System.Drawing.Point(floorResultX, floorResultY)] = movableParticle;
                    Grid[floorResultX, floorResultY] = movableParticle;
                }
            }
        }

        // Returns all changes.
        return gridChanges;
    }

}

