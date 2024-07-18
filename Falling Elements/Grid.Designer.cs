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
            lblMovableSolidCount = new Label();
            lblParticlesOnGround = new Label();
            SuspendLayout();
            // 
            // lblMovableSolidCount
            // 
            lblMovableSolidCount.AutoSize = true;
            lblMovableSolidCount.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lblMovableSolidCount.ForeColor = SystemColors.ButtonHighlight;
            lblMovableSolidCount.Location = new Point(556, 9);
            lblMovableSolidCount.Name = "lblMovableSolidCount";
            lblMovableSolidCount.Size = new Size(158, 21);
            lblMovableSolidCount.TabIndex = 0;
            lblMovableSolidCount.Text = "Movable Solid Count:";
            // 
            // lblParticlesOnGround
            // 
            lblParticlesOnGround.AutoSize = true;
            lblParticlesOnGround.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lblParticlesOnGround.ForeColor = SystemColors.ButtonHighlight;
            lblParticlesOnGround.Location = new Point(273, 9);
            lblParticlesOnGround.Name = "lblParticlesOnGround";
            lblParticlesOnGround.Size = new Size(153, 21);
            lblParticlesOnGround.TabIndex = 1;
            lblParticlesOnGround.Text = "Particles on Ground: ";
            // 
            // Grid
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(10, 12, 13);
            ClientSize = new Size(760, 450);
            Controls.Add(lblParticlesOnGround);
            Controls.Add(lblMovableSolidCount);
            Name = "Grid";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Falling Elements";
            WindowState = FormWindowState.Maximized;
            FormClosed += Grid_FormClosed;
            Shown += Grid_Shown;
            MouseDown += Grid_MouseDown;
            MouseMove += Grid_MouseMove;
            MouseUp += Grid_MouseUp;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblMovableSolidCount;
        private Label lblParticlesOnGround;
    }
}