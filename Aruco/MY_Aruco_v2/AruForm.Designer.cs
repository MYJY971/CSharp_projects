namespace MY_Aruco_v2
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
            this.buttonSegmented = new System.Windows.Forms.Button();
            this.tresh2Label = new System.Windows.Forms.Label();
            this.tresh1Label = new System.Windows.Forms.Label();
            this.buttonTresh2M = new System.Windows.Forms.Button();
            this.buttonTresh2P = new System.Windows.Forms.Button();
            this.buttonTresh1M = new System.Windows.Forms.Button();
            this.buttonTresh1P = new System.Windows.Forms.Button();
            this.buttonAdaptedSize = new System.Windows.Forms.Button();
            this.buttonFullSize = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
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
            this.panelImage.Size = new System.Drawing.Size(635, 317);
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
            this.panelInfo.Controls.Add(this.buttonSegmented);
            this.panelInfo.Controls.Add(this.tresh2Label);
            this.panelInfo.Controls.Add(this.tresh1Label);
            this.panelInfo.Controls.Add(this.buttonTresh2M);
            this.panelInfo.Controls.Add(this.buttonTresh2P);
            this.panelInfo.Controls.Add(this.buttonTresh1M);
            this.panelInfo.Controls.Add(this.buttonTresh1P);
            this.panelInfo.Controls.Add(this.buttonAdaptedSize);
            this.panelInfo.Controls.Add(this.buttonFullSize);
            this.panelInfo.Controls.Add(this.buttonStop);
            this.panelInfo.Controls.Add(this.label2);
            this.panelInfo.Controls.Add(this.label1);
            this.panelInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelInfo.Location = new System.Drawing.Point(0, 317);
            this.panelInfo.Name = "panelInfo";
            this.panelInfo.Size = new System.Drawing.Size(635, 100);
            this.panelInfo.TabIndex = 1;
            // 
            // buttonSegmented
            // 
            this.buttonSegmented.Location = new System.Drawing.Point(174, 71);
            this.buttonSegmented.Name = "buttonSegmented";
            this.buttonSegmented.Size = new System.Drawing.Size(75, 23);
            this.buttonSegmented.TabIndex = 11;
            this.buttonSegmented.Text = "Segmented";
            this.buttonSegmented.UseVisualStyleBackColor = true;
            this.buttonSegmented.Click += new System.EventHandler(this.buttonSegmented_Click);
            // 
            // tresh2Label
            // 
            this.tresh2Label.AutoSize = true;
            this.tresh2Label.Location = new System.Drawing.Point(362, 49);
            this.tresh2Label.Name = "tresh2Label";
            this.tresh2Label.Size = new System.Drawing.Size(40, 13);
            this.tresh2Label.TabIndex = 10;
            this.tresh2Label.Text = "Tresh2";
            // 
            // tresh1Label
            // 
            this.tresh1Label.AutoSize = true;
            this.tresh1Label.Location = new System.Drawing.Point(263, 49);
            this.tresh1Label.Name = "tresh1Label";
            this.tresh1Label.Size = new System.Drawing.Size(40, 13);
            this.tresh1Label.TabIndex = 9;
            this.tresh1Label.Text = "Tresh1";
            // 
            // buttonTresh2M
            // 
            this.buttonTresh2M.Location = new System.Drawing.Point(407, 65);
            this.buttonTresh2M.Name = "buttonTresh2M";
            this.buttonTresh2M.Size = new System.Drawing.Size(38, 35);
            this.buttonTresh2M.TabIndex = 8;
            this.buttonTresh2M.Text = "-";
            this.buttonTresh2M.UseVisualStyleBackColor = true;
            this.buttonTresh2M.Click += new System.EventHandler(this.buttonTresh2M_Click);
            // 
            // buttonTresh2P
            // 
            this.buttonTresh2P.Location = new System.Drawing.Point(365, 65);
            this.buttonTresh2P.Name = "buttonTresh2P";
            this.buttonTresh2P.Size = new System.Drawing.Size(36, 35);
            this.buttonTresh2P.TabIndex = 7;
            this.buttonTresh2P.Text = "+";
            this.buttonTresh2P.UseVisualStyleBackColor = true;
            this.buttonTresh2P.Click += new System.EventHandler(this.buttonTresh2P_Click);
            // 
            // buttonTresh1M
            // 
            this.buttonTresh1M.Location = new System.Drawing.Point(308, 65);
            this.buttonTresh1M.Name = "buttonTresh1M";
            this.buttonTresh1M.Size = new System.Drawing.Size(38, 35);
            this.buttonTresh1M.TabIndex = 6;
            this.buttonTresh1M.Text = "-";
            this.buttonTresh1M.UseVisualStyleBackColor = true;
            this.buttonTresh1M.Click += new System.EventHandler(this.buttonTresh1M_Click);
            // 
            // buttonTresh1P
            // 
            this.buttonTresh1P.Location = new System.Drawing.Point(266, 65);
            this.buttonTresh1P.Name = "buttonTresh1P";
            this.buttonTresh1P.Size = new System.Drawing.Size(36, 35);
            this.buttonTresh1P.TabIndex = 5;
            this.buttonTresh1P.Text = "+";
            this.buttonTresh1P.UseVisualStyleBackColor = true;
            this.buttonTresh1P.Click += new System.EventHandler(this.buttonTresh1P_Click);
            // 
            // buttonAdaptedSize
            // 
            this.buttonAdaptedSize.Location = new System.Drawing.Point(93, 71);
            this.buttonAdaptedSize.Name = "buttonAdaptedSize";
            this.buttonAdaptedSize.Size = new System.Drawing.Size(75, 23);
            this.buttonAdaptedSize.TabIndex = 4;
            this.buttonAdaptedSize.Text = "Real size";
            this.buttonAdaptedSize.UseVisualStyleBackColor = true;
            this.buttonAdaptedSize.Click += new System.EventHandler(this.buttonAdaptedSize_Click);
            // 
            // buttonFullSize
            // 
            this.buttonFullSize.Location = new System.Drawing.Point(12, 71);
            this.buttonFullSize.Name = "buttonFullSize";
            this.buttonFullSize.Size = new System.Drawing.Size(75, 23);
            this.buttonFullSize.TabIndex = 3;
            this.buttonFullSize.Text = "Full";
            this.buttonFullSize.UseVisualStyleBackColor = true;
            this.buttonFullSize.Click += new System.EventHandler(this.buttonFullSize_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.BackColor = System.Drawing.Color.DarkRed;
            this.buttonStop.Location = new System.Drawing.Point(557, 71);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 2;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = false;
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
            // AruForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 417);
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
        private System.Windows.Forms.Label tresh2Label;
        private System.Windows.Forms.Label tresh1Label;
        private System.Windows.Forms.Button buttonTresh2M;
        private System.Windows.Forms.Button buttonTresh2P;
        private System.Windows.Forms.Button buttonTresh1M;
        private System.Windows.Forms.Button buttonTresh1P;
        private System.Windows.Forms.Button buttonSegmented;
    }
}

