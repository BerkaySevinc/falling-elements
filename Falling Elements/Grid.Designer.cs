namespace Falling_Elements
{
    partial class Grid
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblParticleCount = new Label();
            lblParticlesOnGround = new Label();
            btnStone = new Panel();
            btnSand = new Panel();
            btnWater = new Panel();
            trackBarRadius = new TrackBar();
            lblRadius = new Label();
            lblChangedCellCount = new Label();
            ((System.ComponentModel.ISupportInitialize)trackBarRadius).BeginInit();
            SuspendLayout();
            // 
            // lblParticleCount
            // 
            lblParticleCount.AutoSize = true;
            lblParticleCount.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lblParticleCount.ForeColor = SystemColors.ButtonHighlight;
            lblParticleCount.Location = new Point(1066, 16);
            lblParticleCount.Name = "lblParticleCount";
            lblParticleCount.Size = new Size(113, 21);
            lblParticleCount.TabIndex = 0;
            lblParticleCount.Text = "Particle Count: ";
            // 
            // lblParticlesOnGround
            // 
            lblParticlesOnGround.AutoSize = true;
            lblParticlesOnGround.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lblParticlesOnGround.ForeColor = SystemColors.ButtonHighlight;
            lblParticlesOnGround.Location = new Point(828, 16);
            lblParticlesOnGround.Name = "lblParticlesOnGround";
            lblParticlesOnGround.Size = new Size(153, 21);
            lblParticlesOnGround.TabIndex = 1;
            lblParticlesOnGround.Text = "Particles on Ground: ";
            // 
            // btnStone
            // 
            btnStone.BackColor = Color.Silver;
            btnStone.Cursor = Cursors.Hand;
            btnStone.Location = new Point(202, 10);
            btnStone.Name = "btnStone";
            btnStone.Size = new Size(32, 32);
            btnStone.TabIndex = 2;
            btnStone.Click += btnStone_Click;
            // 
            // btnSand
            // 
            btnSand.BackColor = Color.SandyBrown;
            btnSand.Cursor = Cursors.Hand;
            btnSand.Location = new Point(252, 10);
            btnSand.Name = "btnSand";
            btnSand.Size = new Size(32, 32);
            btnSand.TabIndex = 3;
            btnSand.Click += btnSand_Click;
            // 
            // btnWater
            // 
            btnWater.BackColor = Color.DeepSkyBlue;
            btnWater.Cursor = Cursors.Hand;
            btnWater.Location = new Point(303, 10);
            btnWater.Name = "btnWater";
            btnWater.Size = new Size(32, 32);
            btnWater.TabIndex = 3;
            btnWater.Click += btnWater_Click;
            // 
            // trackBarRadius
            // 
            trackBarRadius.Location = new Point(361, 11);
            trackBarRadius.Minimum = 1;
            trackBarRadius.Name = "trackBarRadius";
            trackBarRadius.Size = new Size(100, 45);
            trackBarRadius.TabIndex = 5;
            trackBarRadius.Value = 1;
            trackBarRadius.ValueChanged += trackBarRadius_ValueChanged;
            // 
            // lblRadius
            // 
            lblRadius.AutoSize = true;
            lblRadius.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lblRadius.ForeColor = SystemColors.ButtonHighlight;
            lblRadius.Location = new Point(461, 13);
            lblRadius.Name = "lblRadius";
            lblRadius.Size = new Size(19, 21);
            lblRadius.TabIndex = 6;
            lblRadius.Text = "1";
            // 
            // lblChangedCellCount
            // 
            lblChangedCellCount.AutoSize = true;
            lblChangedCellCount.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lblChangedCellCount.ForeColor = SystemColors.ButtonHighlight;
            lblChangedCellCount.Location = new Point(597, 16);
            lblChangedCellCount.Name = "lblChangedCellCount";
            lblChangedCellCount.Size = new Size(155, 21);
            lblChangedCellCount.TabIndex = 7;
            lblChangedCellCount.Text = "Changed Cell Count: ";
            // 
            // Grid
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(10, 12, 13);
            ClientSize = new Size(1251, 450);
            Controls.Add(lblChangedCellCount);
            Controls.Add(lblRadius);
            Controls.Add(trackBarRadius);
            Controls.Add(btnWater);
            Controls.Add(btnSand);
            Controls.Add(btnStone);
            Controls.Add(lblParticlesOnGround);
            Controls.Add(lblParticleCount);
            Name = "Grid";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Falling Elements";
            WindowState = FormWindowState.Maximized;
            FormClosed += Grid_FormClosed;
            Shown += Grid_Shown;
            MouseDown += Grid_MouseDown;
            MouseMove += Grid_MouseMove;
            MouseUp += Grid_MouseUp;
            ((System.ComponentModel.ISupportInitialize)trackBarRadius).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblParticleCount;
        private Label lblParticlesOnGround;
        private Panel btnStone;
        private Panel btnSand;
        private Panel btnWater;
        private TrackBar trackBarRadius;
        private Label lblRadius;
        private Label lblChangedCellCount;
    }
}