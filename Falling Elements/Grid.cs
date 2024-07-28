using System.Drawing;
using System.Windows.Forms;

using WorldSimulation;
using WorldSimulation.Renderer;




namespace Falling_Elements
{
    public partial class Grid : Form
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Grid()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            InitializeComponent();
        }


        const int scale = 5;
        const int drawSpace = 60;


        // Creates world.
        Graphics graphics;
        World world;
        WorldRenderer renderer;
        private void Grid_Shown(object sender, EventArgs e)
        {
            world = new World(Width / scale, (Height - 39 - drawSpace) / scale)
            {
                Gravity = 10F,
            };

            renderer = new WorldRenderer(world, this, new System.Drawing.Point(0, drawSpace));

            particleAddingMethod = world.AddParticle<Sand>;
            trackBarRadius.Value = 2;

            graphics = CreateGraphics();
            cleanerBrush = new SolidBrush(BackColor);

            Render();
        }

        private SolidBrush cleanerBrush;
        public FpsCounter FpsCounter = new(new(0, 0, 0, 0, 200));
        private void Render()
        {
            Task drawTask = Task.CompletedTask;
            while (true)
            {
                // Update world.
                var changes = world.Update();

                // Wait for initial drawing to finish.
                drawTask.Wait();

                // Display FPS
                int fps = (int)FpsCounter.FpsRender;
                fps = fps > 10000 ? 10000 : fps;
                graphics.FillRectangle(cleanerBrush, 0, 0, 150, drawSpace);
                graphics.DrawString("FPS: " + fps, new Font("Consolas", 12), Brushes.White, 30, 16);

                // Draw updated world.
                drawTask = Task.Run(() => renderer.RenderChanges(changes));

                // Display world info.
                lblChangedCellCount.Text = "Changed Cell Count: " + changes.Count;
                lblParticlesOnGround.Text = "Particles on Ground: " + world.ParticlesOnGroundCount;
                lblParticleCount.Text = "Particle Count: " + world.ParticleCount;

                FpsCounter.StopFrame();
                FpsCounter.StartFrame();

                Application.DoEvents();
            }
        }

        // Adds particle if left mouse button is held down.
        bool isMouseButtonLeftDown = false;
        private void Grid_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button is not MouseButtons.Left) return;

            mouseLocation = new System.Drawing.Point(e.X, e.Y - drawSpace);
            isMouseButtonLeftDown = true;

            Task.Run(() =>
            {
                AddParticles(mouseLocation.X / scale, mouseLocation.Y / scale);

                Thread.Sleep(300);

                while (isMouseButtonLeftDown)
                {
                    Thread.Sleep(20);
                    AddParticles(mouseLocation.X / scale, mouseLocation.Y / scale);
                }
            });
        }
        private void Grid_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button is not MouseButtons.Left) return;

            isMouseButtonLeftDown = false;
        }
        System.Drawing.Point mouseLocation;
        private void Grid_MouseMove(object? sender, MouseEventArgs e)
        {
            if (e.Button is not MouseButtons.Left) return;

            mouseLocation = new System.Drawing.Point(e.X, e.Y - drawSpace);
        }


        Action<System.Drawing.Point, int> particleAddingMethod;
        private void AddParticles(int x, int y)
        {
            particleAddingMethod.Invoke(new(x, y), radius);
        }

        private void btnStone_Click(object sender, EventArgs e) =>
            particleAddingMethod = world.AddParticle<Stone>;

        private void btnSand_Click(object sender, EventArgs e) =>
            particleAddingMethod = world.AddParticle<Sand>;

        private void btnWater_Click(object sender, EventArgs e) =>
            particleAddingMethod = world.AddParticle<Water>;

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
}