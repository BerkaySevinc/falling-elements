using Falling_Elements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;




namespace WorldSimulation;

public class World
{
    public int Width { get; }
    public int Height { get; }
    public float Gravity { get; set; } = 10;

    public IParticle?[,] Grid { get; private set; }


    public World(int width, int height)
    {
        (Width, Height) = (width, height);

        Grid = new IParticle?[Width, Height];
    }
    public World(Size size) : this(size.Width, size.Height) { }
    public World(int size) : this(size, size) { }


    List<IParticle> createdParticles = new();
    public void AddParticle<T>(T particle) where T : struct, IParticle
    {
        var (floorX, floorY) = particle.Coordinates.Floor();

        // Returns if outsite of the bounds.
        if (floorX < 0 || floorX >= Width) return;
        if (floorY < 0 || floorY >= Height) return;

        // Returns if particle already exists in the pixel.
        if (Grid[floorX, floorY]?.GetType() == particle.GetType()) return;

        // Adds to newly created particle list.
        lock (createdParticles)
            createdParticles.Add(particle);
    }


    Random random = new();
    Stopwatch deltaTimer = new();
    public Dictionary<Point, IParticle?> Update()
    {
        // Calculates delta time.
        var deltaTime = (float)deltaTimer.Elapsed.TotalSeconds;
        deltaTimer.Restart();

        var allGridChanges = new Dictionary<Point, IParticle?>();

        // Adds newly created particles to changes.
        lock (createdParticles)
            if (createdParticles.Count > 0)
            {
                foreach (var particle in createdParticles)
                {
                    // Gets particle coordinates info.
                    var (floorX, floorY) = particle.Coordinates.Floor();

                    // Adds particle to grid.
                    Grid[floorX, floorY] = particle;

                    // Adds to changes.
                    allGridChanges[particle.Coordinates] = particle;
                }
                createdParticles.Clear();
            }

        // Loops through all pixels.
        for (int floorY = Height - 1; floorY >= 0; floorY--)
            for (int floorX = Width - 1; floorX >= 0; floorX--)
            {
                // Gets particle.
                var particle = Grid[floorX, floorY];

                // Continues if empty.
                if (particle is null) continue;

                // Gets particle coordinates info.
                var (x, y) = particle.Coordinates;

                // Checks if particle is movable solid.
                if (particle is IMovableSolid movableSolid)
                {
                    // Continues if it is already on the ground.
                    if (floorY == Height - 1) continue;

                    // Calculates the target altitude.
                    var targetY = y + movableSolid.Mass * Gravity * deltaTime;
                    var floorTargetY = (int)targetY;

                    // Continues if particle is still on the same pixel.
                    if (floorTargetY == floorY)
                    {
                        // Moves to target.
                        movableSolid.Coordinates = new Point(x, targetY);
                        continue;
                    }

                    // Checks if path is available.
                    float pathY = Math.Min(y + 1, targetY);
                    int floorPathY = floorY + 1;
                    bool positionChanged = false;
                    Point tempCoordinates = movableSolid.Coordinates;
                    for (;
                        floorPathY <= floorTargetY && floorPathY < Height;
                        pathY = Math.Min(pathY + 1, targetY), floorPathY++
                        )
                    {
                        var (tempX, _) = tempCoordinates;
                        var (floorTempX, _) = tempCoordinates.Floor();

                        // Moves down if its below is empty.
                        if (Grid[floorTempX, floorPathY] is null)
                        {
                            // Stops if on the ground.
                            if (floorPathY == Height - 1)
                            {
                                // Sets exactly on the ground.
                                tempCoordinates = new Point(tempX, floorPathY);
                                positionChanged = true;
                                break;
                            }

                            // Sets temp coordinates and continue loop.
                            tempCoordinates = new Point(tempX, pathY);
                            positionChanged = true;
                            continue;
                        }

                        // Checks diagonals if below is not empty.
                        else
                        {
                            //Checks diagonals.
                            bool isLeftDiagonalAvailable =
                                floorTempX != 0
                                && Grid[floorTempX - 1, floorPathY] is null;
                            //&& Grid[floorTempX - 1, floorTempY] is null;

                            bool isRightDiagonalAvailable =
                                floorTempX != Width - 1
                                && Grid[floorTempX + 1, floorPathY] is null;
                            //&& Grid[floorTempX + 1, floorTempY] is null;

                            // Continues if nowhere is not available.
                            if (!isLeftDiagonalAvailable && !isRightDiagonalAvailable)
                                break;

                            // Moves to available diagonal.
                            else
                            {
                                // Gets diagonal direction according to available one or randomly.
                                int direction =
                                    isLeftDiagonalAvailable && isRightDiagonalAvailable
                                    ? random.Next(2) is 0 ? 1 : -1
                                    : isLeftDiagonalAvailable ? -1 : 1;

                                // Sets temp coordinates and continue loop.
                                tempCoordinates = new Point(tempX + direction, pathY);
                                positionChanged = true;

                                continue;
                            }
                        }
                    }

                    if (!positionChanged) continue;

                    // Gets particle coordinates info.
                    var (floorResultX, floorResultY) = tempCoordinates.Floor();

                    // Moves to target.
                    movableSolid.Coordinates = tempCoordinates;

                    // Applies grid changes.
                    Grid[floorX, floorY] = null;
                    Grid[floorResultX, floorResultY] = movableSolid;

                    // Saves grid changes.
                    allGridChanges[new Point(floorX, floorY)] = null;
                    allGridChanges[tempCoordinates] = movableSolid;
                }
            }

        // Returns all changes.
        return allGridChanges;
    }

    public bool IsOnGround(IParticle particle) => particle.Coordinates.Y == Height - 1;
    public bool IsEmptyBelow(IParticle particle)
    {
        var (floorX, floorY) = particle.Coordinates.Floor();

        return Grid[floorX, floorY + 1] is null;
    }


}

