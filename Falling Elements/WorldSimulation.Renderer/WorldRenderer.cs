using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace WorldSimulation.Renderer;

public class WorldRenderer
{
    public World World { get; }

    public System.Drawing.Point Location { get; }
    public Size Size { get; }


    private Graphics graphics;
    private int horizontalScale;
    private int verticalScale;
    public WorldRenderer(World world, Control control, System.Drawing.Point location, Size size)
    {
        World = world;

        Location = location;
        Size = size;

        horizontalScale = size.Width / world.Width; 
        verticalScale = size.Height / world.Height;

        graphics = control.CreateGraphics();
        cleanerBrush = new SolidBrush(control.BackColor);
    }
    public WorldRenderer(World world, Control control, System.Drawing.Point location) : this(world, control, location, new Size(control.Width - location.X, control.Height - location.Y)) { }
    public WorldRenderer(World world, Control control, Size size) : this(world, control, new(0, 0), size) { }
    public WorldRenderer(World world, Control control) : this(world, control, new(0, 0), control.Size) { }


    public void RenderChanges(Dictionary<System.Drawing.Point, IParticle?> cells)
    {
        // Returns if there is no change.
        if (cells.Count is 0) return;

        // Draws change lines.
        DrawLines(DetectLines(cells));
    }

    public void RenderAll()
    {
        var cells = new Dictionary<System.Drawing.Point, IParticle?>();

        foreach (var particle in World.Particles)
        {
            var (floorX, floorY) = particle.Coordinates.Floor();
            cells.Add(new System.Drawing.Point(floorX, floorY), particle);
        }

        // Clears grid.
        graphics.FillRectangle(cleanerBrush, new Rectangle(Location, Size));

        // Renders particles.
        RenderChanges(cells);
    }

    private List<(int y, int left, int right, Color? color)> DetectLines(Dictionary<System.Drawing.Point, IParticle?> cells)
    {
        // Orders cells by "y", then "x".
        var orderedCells = cells.OrderBy(c => c.Key.Y).ThenBy(c => c.Key.X).ToList();

        // Gets first cell as a line.
        var firstChange = orderedCells.First();
        (int y, int left, int right, Color? color) targetLine = (firstChange.Key.Y, firstChange.Key.X, firstChange.Key.X, firstChange.Value?.Color);

        // Creates list with first line to keep all lines to draw.
        List<(int y, int left, int right, Color? color)> lines = new() { targetLine };

        // Detects bigger horizontal lines to draw at once.
        for (int i = 1; i < orderedCells.Count; i++)
        {
            var cell = orderedCells[i];

            // Gets cell info.
            int x = cell.Key.X;
            int y = cell.Key.Y;
            var color = cell.Value?.Color;

            // Checks if the cell is continuation of the line.
            if (x == (targetLine.right + 1) && y == targetLine.y && color == targetLine.color)
                targetLine.right++;

            // Else create new line.
            else
            {
                lines.Add(targetLine);
                targetLine = (y, x, x, color);
            }
        }
        lines.Add(targetLine);

        return lines;
    }

    private void DrawLines(List<(int y, int left, int right, Color? color)> lines)
    {
        foreach (var rectangle in lines)
        {
            // Gets brush for line.
            var brush = GetBrushByColor(rectangle.color);

            // Draws line.
            graphics.FillRectangle(brush, rectangle.left * horizontalScale + Location.X, rectangle.y * verticalScale + Location.Y, (rectangle.right - rectangle.left + 1) * horizontalScale, verticalScale);
        }
    }


    private SolidBrush cleanerBrush;
    private Dictionary<Color, SolidBrush> particleBrushes = new();
    private SolidBrush GetBrushByColor(Color? color)
    {
        SolidBrush brush;
        if (color is Color c)
        {
            if (!particleBrushes.TryGetValue(c, out brush!))
            {
                brush = new SolidBrush(c);
                particleBrushes.Add(c, brush);
            }
        }
        else brush = cleanerBrush;

        return brush;
    }

}
