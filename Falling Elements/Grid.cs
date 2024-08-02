using System.Drawing;
using System.Windows.Forms;

using WorldSimulation;
using WorldSimulation.Renderer;



//! PARTICLE SAYISI NORMAL�N �ST�NE �IKIYO BAZILARI YOK OLUYO GR�DDEN AMA L�STTE KALIYO (sular�n replace mekani�iyle ilgili san�r�m) AYRICA SULAR WORLDUN �ST�NE DE �IKAB�L�O WORLD DOLUNCA FLN ORDAN DA SAYISI ARTIYO



//! GUI & Demo
// TODO : scale & reset settings ui
// TODO : grid kodlar�n� iyile�tir, se�ilen particl� belirt arkas�na panel koyarak vs.
// TODO : son 1 saniyede fps droplar�n� g�steren bi indicat�r koy ekrana
// TODO : particllar� silme se�ene�i

//! MEKAN�KLER
// TODO : kumun batmas�
// TODO : water h�z�n� artt�r tek seferde 5 blok vs. gidebilsin, algoritma �o�u �eyi y'ye g�re hesapl�yo
// TODO : havada yeri de�i�en particle sorununu ��z
// �aprazdan replace edince �apraza deil �ste ��kmal�
// a��adan replace edincede yukar�s� yerine nullda yanlara ��kmas� daha mant�kl� gibi

//! OPT�M�ZASYON
// UNDONE : DRAW�NG OPT�M�ZASYONU OLARAK SADECE L�NE OLARAK EN B�Y��� ALIYO, ONUN YER�NE EN B�Y�K D�RTGEN� SE�MEL� VE ��ZMEL�
// TODO : �ok emin de�ilim ama update k�sm�nda optimizasyon olarak t�m particlelar� de�ilde, sadece movingleri looplaman�n bi yolu varm�?
// TODO : multithreading ekle
// TODO : isStopping/Stopped gibi bi�ey eklenebilir, alt�ndaki duruyosa o da duruyo olur, hareket etmeye ba�lay�nca �stteki particle�n da isstopping ini false yapar, (b�ylece sadece hareket edenlere loop atabiliriz)

//! D��ER
// TODO : herhangi bi altitude daki listteki elemanlar� rasgele almak daha m� iyi, daha iyisi �nce dikey olarak inenleri ay�r�p hareket ettirmek sonra diagonaller
// TODO : Update de�i�im e�i�i gibi bi�ey olmal�, ��nk� scale b�y�k oldu�unda, bir pixel 0.5 ilerledi�inde gridchanges e girmio bu y�zden ekranda smooth ilerleyemio



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

            renderer = new WorldRenderer(world, this, new(0, drawSpace), new(Width, Height - 39 - drawSpace))
            {

            };

            renderer.MouseDown += RendererMouseDown;
            renderer.MouseUp += RendererMouseUp;

            particleAddingMethod = world.AddParticle<Sand>;
            trackBarRadius.Value = 2;

            graphics = CreateGraphics();
            cleanerBrush = new SolidBrush(BackColor);

            Render();
        }

        private SolidBrush cleanerBrush;
        public FpsCounter FpsCounter = new(new(0, 0, 0, 0, 50));
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
                graphics.FillRectangle(cleanerBrush, 0, 0, 200, drawSpace);
                graphics.DrawString(
                    "FPS: " + fps, 
                    new Font("Consolas", 12),
                    fps > 60 ? Brushes.White : fps > 30 ? Brushes.Yellow : Brushes.Red,
                    30, 16);

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
        private void RendererMouseDown(object? sender, MouseDownEventArgs e)
        {
            if (e.Button is not MouseButtons.Left) return;

            mouseWorldLocation = e.WorldLocation;
            isMouseButtonLeftDown = true;

            renderer.MouseMove += RendererMouseMove;

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

            renderer.MouseMove -= RendererMouseMove;
        }

        System.Drawing.Point mouseWorldLocation;
        private void RendererMouseMove(object? sender, MouseMoveEventArgs e)
        {
            if (e.Button is not MouseButtons.Left) return;

            mouseWorldLocation = e.WorldLocation;
        }


        Action<System.Drawing.Point, int> particleAddingMethod;
        private void AddParticles(System.Drawing.Point worldLocation)
        {
            particleAddingMethod.Invoke(worldLocation, radius);
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