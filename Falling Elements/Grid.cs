using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

using WorldSimulation;
using WorldSimulation.Renderer;





namespace Falling_Elements;


public partial class Grid : Form
{
    public Grid()
    {
        InitializeComponent();

        graphics = CreateGraphics();
        cleanerBrush = new SolidBrush(BackColor);
    }


    const int scale = 5;

    static int fpsFix = 60;
    static bool isFpsFixerEnabled = false;

    const int drawSpace = 60;

    // Creates world.
    Graphics graphics;
    World? world;
    WorldRenderer? renderer;
    [MemberNotNull(nameof(world))]
    private void Grid_Shown(object sender, EventArgs e)
    {
        world = new World(Width / scale, (Height - 39 - drawSpace) / scale)
        {
            Gravity = 10F,
        };

        renderer = new WorldRenderer(world, this, new(0, drawSpace), new(Width - 16, Height - 39 - drawSpace))
        {

        };

        renderer.MouseDown += RendererMouseDown;
        renderer.MouseUp += RendererMouseUp;

        btnSand_Click(btnSand, EventArgs.Empty);
        trackBarRadius.Value = 2;

        Render();
    }

    private SolidBrush cleanerBrush;
    int expectedFrameTimeoutMilliSeconds = 1000 / fpsFix;
    public FpsCounter FpsCounter = new(new(0, 0, 0, 0, 50));
    private void Render()
    {
        Task drawTask = Task.CompletedTask;
        while (true)
        {
            // Update world.
            var renderingUpdates = world!.Update();

            // Wait for initial drawing to finish.
            drawTask.Wait();

            // Display FPS
            int fps = (int)FpsCounter.FpsRender;
            fps = fps > 10000 ? 10000 : fps;
            graphics.FillRectangle(cleanerBrush, 0, 0, 200, drawSpace);
            graphics.DrawString(
                "FPS: " + fps,
                new Font("Consolas", 12),
                fps > 60 ? Brushes.White : fps > 30 ? Brushes.Yellow : Brushes.Red,
                30, 20);

            // Draw updated world.
            drawTask = Task.Run(() => renderer!.RenderChanges(renderingUpdates));

            // Display world info.
            //lblParticleCount.Text = "Particle Count: " + world.ParticleCount;
            //lblUpdatingParticleCount.Text = "Updating Particle Count: " + world.UpdatingParticlesByAltitude.Sum(l => l.Count);
            //lblFreeFallingParticleCount.Text = "Free Falling Particle Count: " + world.UpdatingParticlesByAltitude.Sum(l => l.Count(p => p is MovableParticle movableParticle && movableParticle.IsFreeFalling));
            //lblRenderedCellCount.Text = "Rendered Cell Count: " + renderingUpdates.Updates.Count;

            var elapsed = FpsCounter.RestartFrame();

            // FPS fixer
            if (isFpsFixerEnabled && elapsed.Milliseconds < expectedFrameTimeoutMilliSeconds)
            {
                int timeout = expectedFrameTimeoutMilliSeconds - elapsed.Milliseconds;
                Thread.Sleep(timeout);
            }

            Application.DoEvents();
        }
    }


    // Adds particle if left mouse button is held down.
    bool isMouseButtonLeftDown = false;
    private void RendererMouseDown(object? sender, MouseDownEventArgs e)
    {
        if (e.Button is not MouseButtons.Left) return;

        mouseWorldLocation = e.WorldLocation;
        isMouseButtonLeftDown = true;

        renderer!.MouseMove += RendererMouseMove;

        Task.Run(() =>
        {
            AddParticles(mouseWorldLocation);

            Thread.Sleep(300);

            while (isMouseButtonLeftDown)
            {
                Thread.Sleep(20);
                AddParticles(mouseWorldLocation);
            }
        });
    }

    private void RendererMouseUp(object? sender, MouseUpEventArgs e)
    {
        if (e.Button is not MouseButtons.Left) return;

        isMouseButtonLeftDown = false;

        renderer!.MouseMove -= RendererMouseMove;
    }

    System.Drawing.Point mouseWorldLocation;
    private void RendererMouseMove(object? sender, MouseMoveEventArgs e)
    {
        if (e.Button is not MouseButtons.Left) return;

        mouseWorldLocation = e.WorldLocation;
    }


    Action<System.Drawing.Point, int>? particleAddingMethod;
    private void AddParticles(System.Drawing.Point worldLocation)
        => particleAddingMethod!.Invoke(worldLocation, radius);


    private void btnStone_Click(object sender, EventArgs e)
    {
        particleAddingMethod = world!.AddParticle<Stone>;
        DisplaySelection(sender);
    }

    private void btnSand_Click(object sender, EventArgs e)
    {
        particleAddingMethod = world!.AddParticle<Sand>;
        DisplaySelection(sender);
    }

    private void btnDirt_Click(object sender, EventArgs e)
    {
        particleAddingMethod = world!.AddParticle<Dirt>;
        DisplaySelection(sender);
    }

    private void btnWater_Click(object sender, EventArgs e)
    {
        particleAddingMethod = world!.AddParticle<Water>;
        DisplaySelection(sender);
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
        particleAddingMethod = world!.DeleteParticle;
        DisplaySelection(btnDeleteBack);
    }

    private void DisplaySelection(object sender)
    {
        Control button = (Control)sender;

        int offset = button != btnDeleteBack ? 4 : 2;

        pnlSelected.Location = new System.Drawing.Point(button.Location.X - offset, button.Location.Y - offset);

        btnDeleteBack.Visible = button != btnDeleteBack;
    }


    int radius = 1;
    private void trackBarRadius_ValueChanged(object sender, EventArgs e)
    {
        radius = trackBarRadius.Value;
        lblRadius.Text = radius.ToString();
    }


    private void Grid_FormClosed(object sender, FormClosedEventArgs e)
    {
        Environment.Exit(0);
    }
}