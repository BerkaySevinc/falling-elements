using System.Drawing;
using System.Windows.Forms;
using WorldSimulation;




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
        Graphics g;
        World world;
        private void Grid_Shown(object sender, EventArgs e)
        {
            world = new World(Width / scale, (Height - 39 - drawSpace) / scale)
            {
                Gravity = 10F,
            };

            particleAddingMethod = world.AddParticle<Sand>;
            trackBarRadius.Value = 2;

            g = CreateGraphics();

            Render();
        }

        public FpsCounter FpsCounter = new(new(0, 0, 0, 0, 1000));
        private void Render()
        {
            while (true)
            {
                FpsCounter.StartFrame();

                var changes = world.Update();

                DrawWorld(changes);

                Application.DoEvents();

                int fps = (int)FpsCounter.FpsRender;
                g.FillRectangle(new SolidBrush(BackColor), 0, 0, 100, drawSpace);
                g.DrawString("FPS: " + fps, new Font("Consolas", 12), Brushes.White, 3, 5);

                lblParticleCount.Text = "Particle Count: " + world.ParticleCount;
                lblParticlesOnGround.Text = "Particles on Ground: " + world.ParticlesOnGroundCount;

                FpsCounter.StopFrame();
            }
        }


        public void DrawWorld(Dictionary<WorldSimulation.Point, IParticle?> changes)
        {
            var emptyBrush = new SolidBrush(BackColor);

            foreach (var change in changes)
            {
                var (x, y) = change.Key.Floor();
                IParticle? particle = change.Value;

                var brush = particle is null ? emptyBrush : new SolidBrush(particle.Color);

                g.FillRectangle(brush, x * scale, y * scale + drawSpace, scale, scale);
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