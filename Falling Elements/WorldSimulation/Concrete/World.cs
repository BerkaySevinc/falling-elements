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

    public int Left { get; } = 0;
    public int Right { get; }
    public int Top { get; } = 0;
    public int Bottom { get; }

    public IParticle?[,] Grid { get; }

    public float Gravity { get; set; } = 10;

    public IEnumerable<IParticle> Particles => particlesByAltitude.SelectMany(l => l);
    public int ParticlesOnGroundCount => particlesByAltitude[Bottom].Count;
    public int ParticleCount => particlesByAltitude.Sum(l => l.Count);



    public World(int width, int height)
    {
        (Width, Height) = (width, height);
        (Right, Bottom) = (Width - 1, Height - 1);

        Grid = new IParticle?[Width, Height];

        // Creates particles by altitude list.
        particlesByAltitude = new List<IParticle>[height];

        for (int i = 0; i < Height; i++)
            particlesByAltitude[i] = new List<IParticle>();
    }
    public World(Size size) : this(size.Width, size.Height) { }
    public World(int size) : this(size, size) { }


    Dictionary<(int gridX, int gridY), Type> particlesToCreate = new();
    public void AddParticle<T>(System.Drawing.Point location, int radius) where T : class, IParticle
    {
        if (radius <= 0) return;

        radius--;

        int left = location.X - radius;
        int right = location.X + radius;
        int top = location.Y - radius;
        int bottom = location.Y + radius;

        double radiusSquare = Math.Pow(radius, 2);

        var type = typeof(T);

        lock (particlesToCreate)
            for (int gridY = bottom; gridY >= top; gridY--)
                for (int gridX = right; gridX >= left; gridX--)
                {
                    double dist = Math.Pow(location.X - gridX, 2) + Math.Pow(location.Y - gridY, 2);
                    if (dist > radiusSquare) continue;

                    // Returns if outsite of the bounds.
                    if (gridX < 0 || gridX >= Width) continue;
                    if (gridY < 0 || gridY >= Height) continue;

                    // Adds to newly created particle list.
                    particlesToCreate[(gridX, gridY)] = type;
                }
    }

    List<(int gridX, int gridY)> particlesToDelete = new();
    public void DeleteParticle(System.Drawing.Point location, int radius)
    {
        if (radius <= 0) return;

        radius--;

        int left = location.X - radius;
        int right = location.X + radius;
        int top = location.Y - radius;
        int bottom = location.Y + radius;

        double radiusSquare = Math.Pow(radius, 2);

        lock (particlesToDelete)
            for (int gridY = bottom; gridY >= top; gridY--)
                for (int gridX = right; gridX >= left; gridX--)
                {
                    double dist = Math.Pow(location.X - gridX, 2) + Math.Pow(location.Y - gridY, 2);
                    if (dist > radiusSquare) continue;

                    // Returns if outsite of the bounds.
                    if (gridX < 0 || gridX >= Width) continue;
                    if (gridY < 0 || gridY >= Height) continue;

                    // Adds to particles to delete list.
                    particlesToDelete.Add((gridX, gridY));
                }
    }

    Stopwatch deltaTimer = new();
    public List<IParticle>[] particlesByAltitude;
    public RenderingUpdates Update()
    {
        // Calculates delta time.
        var deltaTime = (float)deltaTimer.Elapsed.TotalSeconds;
        deltaTimer.Restart();

        var renderingUpdates = new RenderingUpdates();

        // Loops through all altitudes.
        for (int floorY = Bottom; floorY >= 0; floorY--)
        {
            var altitudeParticleList = particlesByAltitude[floorY];

            // Loops through all particles at the altitude.
            for (int i = altitudeParticleList.Count - 1; i >= 0; i--)
            {
                // Gets particle.
                var particle = altitudeParticleList[i];

                // Continue if particle cant move.
                if (!particle.IsUpdating) continue;

                // Step particle.
                var changes = particle.Step(deltaTime);

                if (changes is null) continue;
                renderingUpdates.Add(changes);
            }
        }

        // Adds newly created particles to the grid and changes.
        lock (particlesToCreate)
            if (particlesToCreate.Count > 0)
            {
                foreach (var particleInfo in particlesToCreate)
                {
                    // Gets particle info.
                    int gridX = particleInfo.Key.gridX;
                    int gridY = particleInfo.Key.gridY;
                    Type type = particleInfo.Value;

                    // Continue if particle exists.
                    if (Grid[gridX, gridY] is not null) continue;

                    // Creates new particle.
                    IParticle particle = (IParticle)Activator.CreateInstance(type, this, gridX, gridY)!;

                    // Adds particle to grid changes.
                    renderingUpdates.Add((gridX, gridY), (null, particle.Color));
                }

                particlesToCreate.Clear();
            }

        // Removes newly deleted particles from the grid and render changes.
        lock (particlesToDelete)
            if (particlesToDelete.Count > 0)
            {
                foreach (var (gridX, gridY) in particlesToDelete)
                {
                    var existingParticle = Grid[gridX, gridY];

                    if (existingParticle is null) continue;

                    // Adds particle to grid changes.
                    renderingUpdates.Add((gridX, gridY), (existingParticle.Color, null));

                    existingParticle?.Dispose();
                }

                particlesToDelete.Clear();
            }

        // Returns all changes.
        return renderingUpdates;
    }


}

