using System.Drawing;
using System.Windows.Forms;

using WorldSimulation;
using WorldSimulation.Renderer;



//! PARTICLE SAYISI NORMALÝN ÜSTÜNE ÇIKIYO BAZILARI YOK OLUYO GRÝDDEN AMA LÝSTTE KALIYO (sularýn replace mekaniðiyle ilgili sanýrým) AYRICA SULAR WORLDUN ÜSTÜNE DE ÇIKABÝLÝO WORLD DOLUNCA FLN ORDAN DA SAYISI ARTIYO



//! GUI & Demo
// TODO : scale & reset settings ui
// TODO : grid kodlarýný iyileþtir, seçilen particlý belirt arkasýna panel koyarak vs.
// TODO : son 1 saniyede fps droplarýný gösteren bi indicatör koy ekrana
// TODO : particllarý silme seçeneði

//! MEKANÝKLER
// TODO : kumun batmasý
// TODO : water hýzýný arttýr tek seferde 5 blok vs. gidebilsin, algoritma çoðu þeyi y'ye göre hesaplýyo
// TODO : havada yeri deðiþen particle sorununu çöz
// çaprazdan replace edince çapraza deil üste çýkmalý
// aþþadan replace edincede yukarýsý yerine nullda yanlara çýkmasý daha mantýklý gibi

//! OPTÝMÝZASYON
// UNDONE : DRAWÝNG OPTÝMÝZASYONU OLARAK SADECE LÝNE OLARAK EN BÜYÜÐÜ ALIYO, ONUN YERÝNE EN BÜYÜK DÖRTGENÝ SEÇMELÝ VE ÇÝZMELÝ
// TODO : çok emin deðilim ama update kýsmýnda optimizasyon olarak tüm particlelarý deðilde, sadece movingleri looplamanýn bi yolu varmý?
// TODO : multithreading ekle
// TODO : isStopping/Stopped gibi biþey eklenebilir, altýndaki duruyosa o da duruyo olur, hareket etmeye baþlayýnca üstteki particleýn da isstopping ini false yapar, (böylece sadece hareket edenlere loop atabiliriz)

//! DÝÐER
// TODO : herhangi bi altitude daki listteki elemanlarý rasgele almak daha mý iyi, daha iyisi önce dikey olarak inenleri ayýrýp hareket ettirmek sonra diagonaller
// TODO : Update deðiþim eþiði gibi biþey olmalý, çünkü scale büyük olduðunda, bir pixel 0.5 ilerlediðinde gridchanges e girmio bu yüzden ekranda smooth ilerleyemio



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