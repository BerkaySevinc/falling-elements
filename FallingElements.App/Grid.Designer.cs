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
            btnDeleteBack = new Panel();
            btnDelete = new Panel();
            pnlSelectedLine1 = new Panel();
            lblFreeFallingParticleCount = new Label();
            btnDirt = new Panel();
            pnlSelected = new Panel();
            pnlSelectedLine8 = new Panel();
            pnlSelectedLine4 = new Panel();
            pnlSelectedLine7 = new Panel();
            pnlSelectedLine6 = new Panel();
            pnlSelectedLine5 = new Panel();
            pnlSelectedLine2 = new Panel();
            pnlSelectedLine3 = new Panel();
            ((System.ComponentModel.ISupportInitialize)trackBarRadius).BeginInit();
            btnDeleteBack.SuspendLayout();
            pnlSelected.SuspendLayout();
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
            lblParticleCount.Visible = false;
            // 
            // btnStone
            // 
            btnStone.BackColor = Color.LightGray;
            btnStone.Cursor = Cursors.Hand;
            btnStone.Location = new Point(159, 17);
            btnStone.Name = "btnStone";
            btnStone.Size = new Size(32, 32);
            btnStone.TabIndex = 2;
            btnStone.Click += btnStone_Click;
            // 
            // btnSand
            // 
            btnSand.BackColor = Color.FromArgb(255, 230, 85);
            btnSand.Cursor = Cursors.Hand;
            btnSand.Location = new Point(209, 17);
            btnSand.Name = "btnSand";
            btnSand.Size = new Size(32, 32);
            btnSand.TabIndex = 3;
            btnSand.Click += btnSand_Click;
            // 
            // btnWater
            // 
            btnWater.BackColor = Color.FromArgb(50, 205, 255);
            btnWater.Cursor = Cursors.Hand;
            btnWater.Location = new Point(307, 17);
            btnWater.Name = "btnWater";
            btnWater.Size = new Size(32, 32);
            btnWater.TabIndex = 3;
            btnWater.Click += btnWater_Click;
            // 
            // trackBarRadius
            // 
            trackBarRadius.Location = new Point(467, 11);
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
            lblRadius.Location = new Point(567, 13);
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
            lblRenderedCellCount.Visible = false;
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
            lblUpdatingParticleCount.Visible = false;
            // 
            // btnDeleteBack
            // 
            btnDeleteBack.BackColor = Color.Silver;
            btnDeleteBack.Controls.Add(btnDelete);
            btnDeleteBack.Cursor = Cursors.Hand;
            btnDeleteBack.Location = new Point(375, 16);
            btnDeleteBack.Name = "btnDeleteBack";
            btnDeleteBack.Size = new Size(34, 34);
            btnDeleteBack.TabIndex = 9;
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
            // pnlSelectedLine1
            // 
            pnlSelectedLine1.BackColor = Color.Silver;
            pnlSelectedLine1.Location = new Point(0, 0);
            pnlSelectedLine1.Name = "pnlSelectedLine1";
            pnlSelectedLine1.Size = new Size(15, 2);
            pnlSelectedLine1.TabIndex = 12;
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
            lblFreeFallingParticleCount.Visible = false;
            // 
            // btnDirt
            // 
            btnDirt.BackColor = Color.SaddleBrown;
            btnDirt.Cursor = Cursors.Hand;
            btnDirt.Location = new Point(258, 17);
            btnDirt.Name = "btnDirt";
            btnDirt.Size = new Size(32, 32);
            btnDirt.TabIndex = 4;
            btnDirt.Click += btnDirt_Click;
            // 
            // pnlSelected
            // 
            pnlSelected.BackColor = Color.FromArgb(10, 12, 13);
            pnlSelected.Controls.Add(pnlSelectedLine8);
            pnlSelected.Controls.Add(pnlSelectedLine4);
            pnlSelected.Controls.Add(pnlSelectedLine7);
            pnlSelected.Controls.Add(pnlSelectedLine6);
            pnlSelected.Controls.Add(pnlSelectedLine5);
            pnlSelected.Controls.Add(pnlSelectedLine2);
            pnlSelected.Controls.Add(pnlSelectedLine3);
            pnlSelected.Controls.Add(pnlSelectedLine1);
            pnlSelected.Location = new Point(73, 14);
            pnlSelected.Name = "pnlSelected";
            pnlSelected.Size = new Size(40, 40);
            pnlSelected.TabIndex = 11;
            // 
            // pnlSelectedLine8
            // 
            pnlSelectedLine8.BackColor = Color.Silver;
            pnlSelectedLine8.Location = new Point(38, 25);
            pnlSelectedLine8.Name = "pnlSelectedLine8";
            pnlSelectedLine8.Size = new Size(2, 15);
            pnlSelectedLine8.TabIndex = 19;
            // 
            // pnlSelectedLine4
            // 
            pnlSelectedLine4.BackColor = Color.Silver;
            pnlSelectedLine4.Location = new Point(38, 0);
            pnlSelectedLine4.Name = "pnlSelectedLine4";
            pnlSelectedLine4.Size = new Size(2, 15);
            pnlSelectedLine4.TabIndex = 15;
            // 
            // pnlSelectedLine7
            // 
            pnlSelectedLine7.BackColor = Color.Silver;
            pnlSelectedLine7.Location = new Point(0, 25);
            pnlSelectedLine7.Name = "pnlSelectedLine7";
            pnlSelectedLine7.Size = new Size(2, 15);
            pnlSelectedLine7.TabIndex = 18;
            // 
            // pnlSelectedLine6
            // 
            pnlSelectedLine6.BackColor = Color.Silver;
            pnlSelectedLine6.Location = new Point(25, 38);
            pnlSelectedLine6.Name = "pnlSelectedLine6";
            pnlSelectedLine6.Size = new Size(15, 2);
            pnlSelectedLine6.TabIndex = 17;
            // 
            // pnlSelectedLine5
            // 
            pnlSelectedLine5.BackColor = Color.Silver;
            pnlSelectedLine5.Location = new Point(0, 38);
            pnlSelectedLine5.Name = "pnlSelectedLine5";
            pnlSelectedLine5.Size = new Size(15, 2);
            pnlSelectedLine5.TabIndex = 16;
            // 
            // pnlSelectedLine2
            // 
            pnlSelectedLine2.BackColor = Color.Silver;
            pnlSelectedLine2.Location = new Point(25, 0);
            pnlSelectedLine2.Name = "pnlSelectedLine2";
            pnlSelectedLine2.Size = new Size(15, 2);
            pnlSelectedLine2.TabIndex = 14;
            // 
            // pnlSelectedLine3
            // 
            pnlSelectedLine3.BackColor = Color.Silver;
            pnlSelectedLine3.Location = new Point(0, 0);
            pnlSelectedLine3.Name = "pnlSelectedLine3";
            pnlSelectedLine3.Size = new Size(2, 15);
            pnlSelectedLine3.TabIndex = 13;
            // 
            // Grid
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(10, 12, 13);
            ClientSize = new Size(1817, 450);
            Controls.Add(btnDirt);
            Controls.Add(lblFreeFallingParticleCount);
            Controls.Add(btnDeleteBack);
            Controls.Add(lblUpdatingParticleCount);
            Controls.Add(lblRenderedCellCount);
            Controls.Add(lblRadius);
            Controls.Add(trackBarRadius);
            Controls.Add(btnWater);
            Controls.Add(btnSand);
            Controls.Add(btnStone);
            Controls.Add(lblParticleCount);
            Controls.Add(pnlSelected);
            Name = "Grid";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Falling Elements";
            WindowState = FormWindowState.Maximized;
            FormClosed += Grid_FormClosed;
            Shown += Grid_Shown;
            ((System.ComponentModel.ISupportInitialize)trackBarRadius).EndInit();
            btnDeleteBack.ResumeLayout(false);
            pnlSelected.ResumeLayout(false);
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
        private Panel btnDeleteBack;
        private Panel btnDelete;
        private Label lblFreeFallingParticleCount;
        private Panel btnDirt;
        private Panel pnlSelected;
        private Panel pnlSelectedLine1;
        private Panel pnlSelectedLine2;
        private Panel pnlSelectedLine4;
        private Panel pnlSelectedLine3;
        private Panel pnlSelectedLine8;
        private Panel pnlSelectedLine7;
        private Panel pnlSelectedLine6;
        private Panel pnlSelectedLine5;
    }
}