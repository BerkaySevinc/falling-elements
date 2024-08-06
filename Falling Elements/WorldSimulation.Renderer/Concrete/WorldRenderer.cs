using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorldSimulation.Renderer;

public class WorldRenderer
{
    public World World { get; }

    public Control Control { get; }


    private System.Drawing.Point _location;
    public System.Drawing.Point Location
    {
        get => _location;
        set
        {
            Clear();

            _location = value;

            RenderAll();
        }
    }


    private float horizontalScale;
    private float verticalScale;
    private Size _size;
    public Size Size
    {
        get => _size;
        set
        {
            Clear();

            _size = value;

            SetScales();

            RenderAll();
        }
    }
    private void SetScales()
    {
        horizontalScale = (float)_size.Width / World.Width;
        verticalScale = (float)_size.Height / World.Height;
    }

    private bool _syncSizeWithControl = true;
    public bool SyncSizeWithControl
    {
        get => _syncSizeWithControl;
        set
        {
            _syncSizeWithControl = value;

            if (value) SyncSize();
        }
    }


    public event EventHandler? Clicked;
    public event EventHandler<MouseClickedEventArgs>? MouseClicked;
    public event EventHandler<MouseDownEventArgs>? MouseDown;
    public event EventHandler<MouseUpEventArgs>? MouseUp;

    private readonly object mouseMoveLock = new();
    private EventHandler<MouseMoveEventArgs>? _mouseMove;
    public event EventHandler<MouseMoveEventArgs>? MouseMove
    {
        add
        {
            lock (mouseMoveLock)
            {
                if (_mouseMove is null) Control.MouseMove += ControlMouseMove;
                _mouseMove += value;
            }
        }
        remove
        {
            lock (mouseMoveLock)
            {
                _mouseMove -= value;
                if (_mouseMove is null) Control.MouseMove -= ControlMouseMove;
            }
        }
    }

    protected virtual void OnClicked(EventArgs e) =>
        Clicked?.Invoke(this, e);

    protected virtual void OnMouseClicked(MouseClickedEventArgs e) =>
        MouseClicked?.Invoke(this, e);

    protected virtual void OnMouseDown(MouseDownEventArgs e) =>
        MouseDown?.Invoke(this, e);

    protected virtual void OnMouseUp(MouseUpEventArgs e) =>
        MouseUp?.Invoke(this, e);

    protected virtual void OnMouseMove(MouseMoveEventArgs e) =>
        _mouseMove?.Invoke(this, e);


    private int marginRight;
    private int marginBottom;
    private Graphics graphics;
    public WorldRenderer(World world, Control control, System.Drawing.Point location, Size size)
    {
        World = world;

        Control = control;

        _location = location;
        _size = size;
        SetScales();

        marginRight = Control.Width - (location.X + size.Width);
        marginBottom = Control.Height - (location.Y + size.Height);

        graphics = control.CreateGraphics();
        cleanerBrush = new SolidBrush(control.BackColor);

        control.Resize += ControlResize;

        control.Click += ControlClick;
        control.MouseClick += ControlMouseClick;
        control.MouseDown += ControlMouseDown;
        control.MouseUp += ControlMouseUp;
    }
    public WorldRenderer(World world, Control control, System.Drawing.Point location) : this(world, control, location, new Size(control.Width - location.X, control.Height - location.Y)) { }
    public WorldRenderer(World world, Control control, Size size) : this(world, control, new(0, 0), size) { }
    public WorldRenderer(World world, Control control) : this(world, control, new(0, 0), control.Size) { }



    public void RenderChanges(RenderingUpdates renderingUpdates)
    {
        // Returns if there is no change.
        if (renderingUpdates.Updates.Count is 0) return;

        // Draws change lines.
        DrawLines(DetectLines(renderingUpdates));
    }

    public void RenderAll()
    {
        var renderingUpdates = new RenderingUpdates();

        foreach (var particle in World.Particles)
            renderingUpdates.Add((particle.GridX, particle.GridY), (null, particle.Color));

        // Clears grid.
        Clear();

        // Renders particles.
        RenderChanges(renderingUpdates);
    }

    public void Clear()
    {
        graphics.FillRectangle(cleanerBrush, new Rectangle(Location, Size));
    }

    private List<(int y, int left, int right, Color? color)> DetectLines(RenderingUpdates renderingUpdates)
    {
        // Orders cells by "y", then "x".
        var orderedChanges = 
            renderingUpdates.Updates
            .OrderBy(c => c.Key.y)
            .ThenBy(c => c.Key.x)
            .ToList();

        // Gets first cell as a line.
        var firstChange = orderedChanges.First();
        (int y, int left, int right, Color? color) targetLine = (firstChange.Key.y, firstChange.Key.x, firstChange.Key.x, firstChange.Value.newColor);

        // Creates list with first line to keep all lines to draw.
        var lines = new List<(int y, int left, int right, Color? color)>();

        // Detects bigger horizontal lines to draw at once.
        for (int i = 1; i < orderedChanges.Count; i++)
        {
            var cell = orderedChanges[i];

            // Gets cell info.
            int x = cell.Key.x;
            int y = cell.Key.y;
            var color = cell.Value.newColor;

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


    private void ControlResize(object? sender, EventArgs e)
    {
        if (!SyncSizeWithControl)
        {
            RenderAll();
            return;
        }

        SyncSize();
    }

    private void SyncSize()
    {
        Size = new(Control.Width - (Location.X + marginRight), Control.Height - (Location.Y + marginBottom));
    }

    private void ControlClick(object? sender, EventArgs e) =>
        OnClicked(EventArgs.Empty);

    private void ControlMouseClick(object? sender, MouseEventArgs e)
    {
        if (MouseClicked is null) return;

        var targetLocation = GetWorldLocation(e.Location);

        if (targetLocation is not System.Drawing.Point worldLocation) return;

        var args = new MouseClickedEventArgs(e.Button, e.Location, worldLocation);
        OnMouseClicked(args);
    }

    private void ControlMouseDown(object? sender, MouseEventArgs e)
    {
        if (MouseDown is null) return;

        var targetLocation = GetWorldLocation(e.Location);

        if (targetLocation is not System.Drawing.Point worldLocation) return;

        var args = new MouseDownEventArgs(e.Button, e.Location, worldLocation);
        OnMouseDown(args);
    }

    private void ControlMouseUp(object? sender, MouseEventArgs e)
    {
        if (MouseUp is null) return;

        var worldLocation = GetWorldLocation(e.Location);

        var args = new MouseUpEventArgs(e.Button, e.Location, worldLocation);
        OnMouseUp(args);
    }

    private void ControlMouseMove(object? sender, MouseEventArgs e)
    {
        var targetLocation = GetWorldLocation(e.Location);

        if (targetLocation is not System.Drawing.Point worldLocation) return;

        var args = new MouseMoveEventArgs(e.Button, e.Location, worldLocation);
        OnMouseMove(args);
    }

    private System.Drawing.Point? GetWorldLocation(System.Drawing.Point location)
    {
        var targetX = location.X - Location.X;
        var targetY = location.Y - Location.Y;

        if (targetX < 0 || targetY < 0) return null;
        if (targetX >= Size.Width || targetY >= Size.Height) return null;

        return new System.Drawing.Point((int)(targetX / horizontalScale), (int)(targetY / verticalScale));
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



