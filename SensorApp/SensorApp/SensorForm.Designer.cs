namespace SensorApp
{
    partial class SensorForm
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
            this.glControl1 = new OpenTK.GLControl();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.afficherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.masquerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelMenu = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelTexture = new System.Windows.Forms.Label();
            this.radioButtonTextured = new System.Windows.Forms.RadioButton();
            this.radioButtonNoTextured = new System.Windows.Forms.RadioButton();
            this.menuStrip1.SuspendLayout();
            this.panelMenu.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // glControl1
            // 
            this.glControl1.BackColor = System.Drawing.Color.Black;
            this.glControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl1.Location = new System.Drawing.Point(0, 24);
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(682, 515);
            this.glControl1.TabIndex = 0;
            this.glControl1.VSync = false;
            this.glControl1.Load += new System.EventHandler(this.glControl1_Load);
            this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl1_Paint);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(682, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.afficherToolStripMenuItem,
            this.masquerToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // afficherToolStripMenuItem
            // 
            this.afficherToolStripMenuItem.Name = "afficherToolStripMenuItem";
            this.afficherToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.afficherToolStripMenuItem.Text = "Afficher";
            this.afficherToolStripMenuItem.Click += new System.EventHandler(this.afficherToolStripMenuItem_Click);
            // 
            // masquerToolStripMenuItem
            // 
            this.masquerToolStripMenuItem.Name = "masquerToolStripMenuItem";
            this.masquerToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.masquerToolStripMenuItem.Text = "Masquer";
            this.masquerToolStripMenuItem.Click += new System.EventHandler(this.masquerToolStripMenuItem_Click);
            // 
            // panelMenu
            // 
            this.panelMenu.Controls.Add(this.panel1);
            this.panelMenu.Location = new System.Drawing.Point(0, 24);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(682, 75);
            this.panelMenu.TabIndex = 2;
            this.panelMenu.Visible = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radioButtonNoTextured);
            this.panel1.Controls.Add(this.radioButtonTextured);
            this.panel1.Controls.Add(this.labelTexture);
            this.panel1.Location = new System.Drawing.Point(346, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(111, 69);
            this.panel1.TabIndex = 0;
            // 
            // labelTexture
            // 
            this.labelTexture.AutoSize = true;
            this.labelTexture.Location = new System.Drawing.Point(4, 4);
            this.labelTexture.Name = "labelTexture";
            this.labelTexture.Size = new System.Drawing.Size(43, 13);
            this.labelTexture.TabIndex = 0;
            this.labelTexture.Text = "Texture";
            // 
            // radioButtonTextured
            // 
            this.radioButtonTextured.AutoSize = true;
            this.radioButtonTextured.Checked = true;
            this.radioButtonTextured.Location = new System.Drawing.Point(7, 21);
            this.radioButtonTextured.Name = "radioButtonTextured";
            this.radioButtonTextured.Size = new System.Drawing.Size(41, 17);
            this.radioButtonTextured.TabIndex = 1;
            this.radioButtonTextured.TabStop = true;
            this.radioButtonTextured.Text = "Oui";
            this.radioButtonTextured.UseVisualStyleBackColor = true;
            this.radioButtonTextured.CheckedChanged += new System.EventHandler(this.radioButtonTextured_CheckedChanged);
            // 
            // radioButtonNoTextured
            // 
            this.radioButtonNoTextured.AutoSize = true;
            this.radioButtonNoTextured.Location = new System.Drawing.Point(7, 44);
            this.radioButtonNoTextured.Name = "radioButtonNoTextured";
            this.radioButtonNoTextured.Size = new System.Drawing.Size(45, 17);
            this.radioButtonNoTextured.TabIndex = 2;
            this.radioButtonNoTextured.TabStop = true;
            this.radioButtonNoTextured.Text = "Non";
            this.radioButtonNoTextured.UseVisualStyleBackColor = true;
            this.radioButtonNoTextured.CheckedChanged += new System.EventHandler(this.radioButtonNoTextured_CheckedChanged);
            // 
            // SensorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 539);
            this.Controls.Add(this.panelMenu);
            this.Controls.Add(this.glControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SensorForm";
            this.Text = "Prise en main des capteurs";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelMenu.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenTK.GLControl glControl1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem afficherToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem masquerToolStripMenuItem;
        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioButtonNoTextured;
        private System.Windows.Forms.RadioButton radioButtonTextured;
        private System.Windows.Forms.Label labelTexture;
    }
}

