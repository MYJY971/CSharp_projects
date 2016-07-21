namespace MY_Aruco
{
    partial class AruForm
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelImage = new System.Windows.Forms.Panel();
            this.glControl1 = new OpenTK.GLControl();
            this.panelInfo = new System.Windows.Forms.Panel();
            this.buttonStop = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonFullSize = new System.Windows.Forms.Button();
            this.buttonAdaptedSize = new System.Windows.Forms.Button();
            this.panelImage.SuspendLayout();
            this.panelInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelImage
            // 
            this.panelImage.BackColor = System.Drawing.Color.DarkRed;
            this.panelImage.Controls.Add(this.glControl1);
            this.panelImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelImage.Location = new System.Drawing.Point(0, 0);
            this.panelImage.Name = "panelImage";
            this.panelImage.Size = new System.Drawing.Size(487, 317);
            this.panelImage.TabIndex = 0;
            // 
            // glControl1
            // 
            this.glControl1.BackColor = System.Drawing.Color.Black;
            this.glControl1.Location = new System.Drawing.Point(143, 119);
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(172, 107);
            this.glControl1.TabIndex = 0;
            this.glControl1.VSync = false;
            this.glControl1.Load += new System.EventHandler(this.glControl1_Load);
            this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl1_Paint);
            // 
            // panelInfo
            // 
            this.panelInfo.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panelInfo.Controls.Add(this.buttonAdaptedSize);
            this.panelInfo.Controls.Add(this.buttonFullSize);
            this.panelInfo.Controls.Add(this.buttonStop);
            this.panelInfo.Controls.Add(this.label2);
            this.panelInfo.Controls.Add(this.label1);
            this.panelInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelInfo.Location = new System.Drawing.Point(0, 317);
            this.panelInfo.Name = "panelInfo";
            this.panelInfo.Size = new System.Drawing.Size(487, 100);
            this.panelInfo.TabIndex = 1;
            // 
            // buttonStop
            // 
            this.buttonStop.BackColor = System.Drawing.Color.DarkRed;
            this.buttonStop.Location = new System.Drawing.Point(412, 74);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 2;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = false;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(151, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "label2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // buttonFullSize
            // 
            this.buttonFullSize.Location = new System.Drawing.Point(222, 36);
            this.buttonFullSize.Name = "buttonFullSize";
            this.buttonFullSize.Size = new System.Drawing.Size(75, 23);
            this.buttonFullSize.TabIndex = 3;
            this.buttonFullSize.Text = "Full";
            this.buttonFullSize.UseVisualStyleBackColor = true;
            this.buttonFullSize.Click += new System.EventHandler(this.buttonFullSize_Click);
            // 
            // buttonAdaptedSize
            // 
            this.buttonAdaptedSize.Location = new System.Drawing.Point(303, 36);
            this.buttonAdaptedSize.Name = "buttonAdaptedSize";
            this.buttonAdaptedSize.Size = new System.Drawing.Size(75, 23);
            this.buttonAdaptedSize.TabIndex = 4;
            this.buttonAdaptedSize.Text = "Adapted";
            this.buttonAdaptedSize.UseVisualStyleBackColor = true;
            this.buttonAdaptedSize.Click += new System.EventHandler(this.buttonAdaptedSize_Click);
            // 
            // AruForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 417);
            this.Controls.Add(this.panelImage);
            this.Controls.Add(this.panelInfo);
            this.Name = "AruForm";
            this.Text = "MY-Aruco";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panelImage.ResumeLayout(false);
            this.panelInfo.ResumeLayout(false);
            this.panelInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelImage;
        private System.Windows.Forms.Panel panelInfo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private OpenTK.GLControl glControl1;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonAdaptedSize;
        private System.Windows.Forms.Button buttonFullSize;
    }
}

