using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Configuration;
using System.Numerics;
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

    const int drawSpace = 66;

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
            MaxColorVariationCount = 20
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
    private int expectedFrameTimeoutMilliSeconds = 1000 / fpsFix;
    private FpsCounter fpsCounter = new(new(0, 0, 0, 0, 50));
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
            int fps = (int)fpsCounter.FpsRender;
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

            var elapsed = fpsCounter.RestartFrame();

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
    private bool drawBetweenMouseMove = false;
    private bool isMouseButtonLeftDown = false;
    private System.Drawing.Point initialMouseWorldLocation;
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

            if (!isMouseButtonLeftDown) return;

            AddParticles(mouseWorldLocation);
            initialMouseWorldLocation = mouseWorldLocation;

            while (isMouseButtonLeftDown)
            {
                Thread.Sleep(1);

                if (!drawBetweenMouseMove) AddParticles(mouseWorldLocation);
                else
                {
                    IterateBetweenTwoPoints(initialMouseWorldLocation, mouseWorldLocation, AddParticles);
                    initialMouseWorldLocation = mouseWorldLocation;
                }
            }
        });
    }

    private static void IterateBetweenTwoPoints(System.Drawing.Point start, System.Drawing.Point end, Action<System.Drawing.Point> iterationCallback)
    {
        // Calculates diffs.
        int xDiff = end.X - start.X;
        int yDiff = end.Y - start.Y;

        // Returns if points are same.
        if (xDiff is 0 && yDiff is 0)
        {
            iterationCallback?.Invoke(start);
            return;
        }

        // Gets which is larger.
        bool isYDiffIsLarger = Math.Abs(xDiff) > Math.Abs(yDiff);

        // Get longer & shorter sides.
        (int longerSide, int shorterSide) = isYDiffIsLarger ? (xDiff, yDiff) : (yDiff, xDiff);

        // Calculates slope.
        float slope = (float)shorterSide / longerSide;


        int longerSideAbs = Math.Abs(longerSide);
        int longerSideModifier = longerSide > 0 ? 1 : -1;

        for (int i = 0; i <= longerSideAbs; i++)
        {
            int longerSideIncrease = i * longerSideModifier;
            int shorterSideIncrease = (int)Math.Round(longerSideIncrease * slope, MidpointRounding.AwayFromZero);

            int newX, newY;
            if (isYDiffIsLarger)
            {
                newX = start.X + longerSideIncrease;
                newY = start.Y + shorterSideIncrease;
            }
            else
            {
                newY = start.Y + longerSideIncrease;
                newX = start.X + shorterSideIncrease;
            }

            iterationCallback?.Invoke(new System.Drawing.Point(newX, newY));
        }
    }



    private void RendererMouseUp(object? sender, MouseUpEventArgs e)
    {
        if (e.Button is not MouseButtons.Left) return;

        isMouseButtonLeftDown = false;

        renderer!.MouseMove -= RendererMouseMove;
    }

    private System.Drawing.Point mouseWorldLocation;
    private void RendererMouseMove(object? sender, MouseMoveEventArgs e)
    {
        if (e.Button is not MouseButtons.Left) return;

        mouseWorldLocation = e.WorldLocation;
    }


    private Action<System.Drawing.Point, int>? particleAddingMethod;
    private void AddParticles(System.Drawing.Point worldLocation)
        => particleAddingMethod!.Invoke(worldLocation, brushRadius);


    private void btnStone_Click(object sender, EventArgs e)
    {
        particleAddingMethod = world!.AddParticle<Stone>;
        ParticleSelectionChanged(sender, true);
    }

    private void btnSand_Click(object sender, EventArgs e)
    {
        particleAddingMethod = world!.AddParticle<Sand>;
        ParticleSelectionChanged(sender, false);
    }

    private void btnDirt_Click(object sender, EventArgs e)
    {
        particleAddingMethod = world!.AddParticle<Dirt>;
        ParticleSelectionChanged(sender, false);
    }

    private void btnWater_Click(object sender, EventArgs e)
    {
        particleAddingMethod = world!.AddParticle<Water>;
        ParticleSelectionChanged(sender, false);
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
        particleAddingMethod = world!.DeleteParticle;
        ParticleSelectionChanged(btnDeleteBack, true);
    }

    private void ParticleSelectionChanged(object sender, bool drawBetween)
    {
        drawBetweenMouseMove = drawBetween;

        var button = (Control)sender;

        int offset = button != btnDeleteBack ? 4 : 2;

        pnlSelected.Location = new System.Drawing.Point(button.Location.X - offset, button.Location.Y - offset);

        btnDeleteBack.Visible = button != btnDeleteBack;
    }


    private int brushRadius = 1;
    private void trackBarRadius_ValueChanged(object sender, EventArgs e)
    {
        brushRadius = trackBarRadius.Value;
        lblRadius.Text = brushRadius.ToString();
    }


    private void Grid_FormClosed(object sender, FormClosedEventArgs e)
    {
        Environment.Exit(0);
    }
}