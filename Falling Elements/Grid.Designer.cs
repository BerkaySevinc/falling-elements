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
            btnStone = new Panel();
            btnSand = new Panel();
            btnWater = new Panel();
            trackBarRadius = new TrackBar();
            lblRadius = new Label();
            lblRenderedCellCount = new Label();
            lblUpdatingParticleCount = new Label();
            panel1 = new Panel();
            btnDelete = new Panel();
            lblFreeFallingParticleCount = new Label();
            ((System.ComponentModel.ISupportInitialize)trackBarRadius).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // lblParticleCount
            // 
            lblParticleCount.AutoSize = true;
            lblParticleCount.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lblParticleCount.ForeColor = SystemColors.ButtonHighlight;
            lblParticleCount.Location = new Point(657, 16);
            lblParticleCount.Name = "lblParticleCount";
            lblParticleCount.Size = new Size(113, 21);
            lblParticleCount.TabIndex = 0;
            lblParticleCount.Text = "Particle Count: ";
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
            trackBarRadius.Location = new Point(421, 11);
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
            lblRadius.Location = new Point(521, 13);
            lblRadius.Name = "lblRadius";
            lblRadius.Size = new Size(19, 21);
            lblRadius.TabIndex = 6;
            lblRadius.Text = "1";
            // 
            // lblRenderedCellCount
            // 
            lblRenderedCellCount.AutoSize = true;
            lblRenderedCellCount.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lblRenderedCellCount.ForeColor = SystemColors.ButtonHighlight;
            lblRenderedCellCount.Location = new Point(1455, 16);
            lblRenderedCellCount.Name = "lblRenderedCellCount";
            lblRenderedCellCount.Size = new Size(160, 21);
            lblRenderedCellCount.TabIndex = 7;
            lblRenderedCellCount.Text = "Rendered Cell Count: ";
            // 
            // lblUpdatingParticleCount
            // 
            lblUpdatingParticleCount.AutoSize = true;
            lblUpdatingParticleCount.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lblUpdatingParticleCount.ForeColor = SystemColors.ButtonHighlight;
            lblUpdatingParticleCount.Location = new Point(881, 16);
            lblUpdatingParticleCount.Name = "lblUpdatingParticleCount";
            lblUpdatingParticleCount.Size = new Size(181, 21);
            lblUpdatingParticleCount.TabIndex = 8;
            lblUpdatingParticleCount.Text = "Updating Particle Count: ";
            // 
            // panel1
            // 
            panel1.BackColor = Color.Silver;
            panel1.Controls.Add(btnDelete);
            panel1.Cursor = Cursors.Hand;
            panel1.Location = new Point(353, 9);
            panel1.Name = "panel1";
            panel1.Size = new Size(34, 34);
            panel1.TabIndex = 9;
            // 
            // btnDelete
            // 
            btnDelete.BackColor = Color.FromArgb(10, 12, 13);
            btnDelete.Cursor = Cursors.Hand;
            btnDelete.Location = new Point(1, 1);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(32, 32);
            btnDelete.TabIndex = 10;
            btnDelete.Click += btnDelete_Click;
            // 
            // lblFreeFallingParticleCount
            // 
            lblFreeFallingParticleCount.AutoSize = true;
            lblFreeFallingParticleCount.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lblFreeFallingParticleCount.ForeColor = SystemColors.ButtonHighlight;
            lblFreeFallingParticleCount.Location = new Point(1165, 16);
            lblFreeFallingParticleCount.Name = "lblFreeFallingParticleCount";
            lblFreeFallingParticleCount.Size = new Size(196, 21);
            lblFreeFallingParticleCount.TabIndex = 10;
            lblFreeFallingParticleCount.Text = "Free Falling Particle Count: ";
            // 
            // Grid
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(10, 12, 13);
            ClientSize = new Size(1817, 450);
            Controls.Add(lblFreeFallingParticleCount);
            Controls.Add(panel1);
            Controls.Add(lblUpdatingParticleCount);
            Controls.Add(lblRenderedCellCount);
            Controls.Add(lblRadius);
            Controls.Add(trackBarRadius);
            Controls.Add(btnWater);
            Controls.Add(btnSand);
            Controls.Add(btnStone);
            Controls.Add(lblParticleCount);
            Name = "Grid";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Falling Elements";
            WindowState = FormWindowState.Maximized;
            FormClosed += Grid_FormClosed;
            Shown += Grid_Shown;
            ((System.ComponentModel.ISupportInitialize)trackBarRadius).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblParticleCount;
        private Panel btnStone;
        private Panel btnSand;
        private Panel btnWater;
        private TrackBar trackBarRadius;
        private Label lblRadius;
        private Label lblRenderedCellCount;
        private Label lblUpdatingParticleCount;
        private Panel panel1;
        private Panel btnDelete;
        private Label lblFreeFallingParticleCount;
    }
}