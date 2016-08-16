namespace Sensor
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.glControl1 = new OpenTK.GLControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxM11 = new System.Windows.Forms.TextBox();
            this.textBoxM12 = new System.Windows.Forms.TextBox();
            this.textBoxM13 = new System.Windows.Forms.TextBox();
            this.textBoxM14 = new System.Windows.Forms.TextBox();
            this.textBoxM21 = new System.Windows.Forms.TextBox();
            this.textBoxM22 = new System.Windows.Forms.TextBox();
            this.textBoxM23 = new System.Windows.Forms.TextBox();
            this.textBoxM24 = new System.Windows.Forms.TextBox();
            this.textBoxM31 = new System.Windows.Forms.TextBox();
            this.textBoxM32 = new System.Windows.Forms.TextBox();
            this.textBoxM33 = new System.Windows.Forms.TextBox();
            this.textBoxM34 = new System.Windows.Forms.TextBox();
            this.textBoxM41 = new System.Windows.Forms.TextBox();
            this.textBoxM42 = new System.Windows.Forms.TextBox();
            this.textBoxM43 = new System.Windows.Forms.TextBox();
            this.textBoxM44 = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxMV44 = new System.Windows.Forms.TextBox();
            this.textBoxMV43 = new System.Windows.Forms.TextBox();
            this.textBoxMV42 = new System.Windows.Forms.TextBox();
            this.textBoxMV41 = new System.Windows.Forms.TextBox();
            this.textBoxMV34 = new System.Windows.Forms.TextBox();
            this.textBoxMV33 = new System.Windows.Forms.TextBox();
            this.textBoxMV32 = new System.Windows.Forms.TextBox();
            this.textBoxMV31 = new System.Windows.Forms.TextBox();
            this.textBoxMV24 = new System.Windows.Forms.TextBox();
            this.textBoxMV23 = new System.Windows.Forms.TextBox();
            this.textBoxMV22 = new System.Windows.Forms.TextBox();
            this.textBoxMV21 = new System.Windows.Forms.TextBox();
            this.textBoxMV14 = new System.Windows.Forms.TextBox();
            this.textBoxMV13 = new System.Windows.Forms.TextBox();
            this.textBoxMV12 = new System.Windows.Forms.TextBox();
            this.textBoxMV11 = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // glControl1
            // 
            this.glControl1.BackColor = System.Drawing.Color.Black;
            this.glControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl1.Location = new System.Drawing.Point(0, 0);
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(868, 522);
            this.glControl1.TabIndex = 0;
            this.glControl1.VSync = false;
            this.glControl1.Load += new System.EventHandler(this.glControl1_Load);
            this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl1_Paint);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(210, 117);
            this.panel1.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.textBoxM44, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBoxM43, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBoxM42, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBoxM41, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBoxM34, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBoxM33, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBoxM32, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBoxM31, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBoxM24, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxM23, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxM22, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxM21, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxM14, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBoxM13, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBoxM12, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBoxM11, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(7, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(181, 100);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // textBoxM11
            // 
            this.textBoxM11.Location = new System.Drawing.Point(3, 3);
            this.textBoxM11.Name = "textBoxM11";
            this.textBoxM11.Size = new System.Drawing.Size(39, 20);
            this.textBoxM11.TabIndex = 0;
            // 
            // textBoxM12
            // 
            this.textBoxM12.Location = new System.Drawing.Point(48, 3);
            this.textBoxM12.Name = "textBoxM12";
            this.textBoxM12.Size = new System.Drawing.Size(39, 20);
            this.textBoxM12.TabIndex = 1;
            // 
            // textBoxM13
            // 
            this.textBoxM13.Location = new System.Drawing.Point(93, 3);
            this.textBoxM13.Name = "textBoxM13";
            this.textBoxM13.Size = new System.Drawing.Size(39, 20);
            this.textBoxM13.TabIndex = 2;
            // 
            // textBoxM14
            // 
            this.textBoxM14.Location = new System.Drawing.Point(138, 3);
            this.textBoxM14.Name = "textBoxM14";
            this.textBoxM14.Size = new System.Drawing.Size(39, 20);
            this.textBoxM14.TabIndex = 3;
            // 
            // textBoxM21
            // 
            this.textBoxM21.Location = new System.Drawing.Point(3, 28);
            this.textBoxM21.Name = "textBoxM21";
            this.textBoxM21.Size = new System.Drawing.Size(39, 20);
            this.textBoxM21.TabIndex = 4;
            // 
            // textBoxM22
            // 
            this.textBoxM22.Location = new System.Drawing.Point(48, 28);
            this.textBoxM22.Name = "textBoxM22";
            this.textBoxM22.Size = new System.Drawing.Size(39, 20);
            this.textBoxM22.TabIndex = 5;
            // 
            // textBoxM23
            // 
            this.textBoxM23.Location = new System.Drawing.Point(93, 28);
            this.textBoxM23.Name = "textBoxM23";
            this.textBoxM23.Size = new System.Drawing.Size(39, 20);
            this.textBoxM23.TabIndex = 6;
            // 
            // textBoxM24
            // 
            this.textBoxM24.Location = new System.Drawing.Point(138, 28);
            this.textBoxM24.Name = "textBoxM24";
            this.textBoxM24.Size = new System.Drawing.Size(39, 20);
            this.textBoxM24.TabIndex = 7;
            // 
            // textBoxM31
            // 
            this.textBoxM31.Location = new System.Drawing.Point(3, 53);
            this.textBoxM31.Name = "textBoxM31";
            this.textBoxM31.Size = new System.Drawing.Size(39, 20);
            this.textBoxM31.TabIndex = 8;
            // 
            // textBoxM32
            // 
            this.textBoxM32.Location = new System.Drawing.Point(48, 53);
            this.textBoxM32.Name = "textBoxM32";
            this.textBoxM32.Size = new System.Drawing.Size(39, 20);
            this.textBoxM32.TabIndex = 9;
            // 
            // textBoxM33
            // 
            this.textBoxM33.Location = new System.Drawing.Point(93, 53);
            this.textBoxM33.Name = "textBoxM33";
            this.textBoxM33.Size = new System.Drawing.Size(39, 20);
            this.textBoxM33.TabIndex = 10;
            // 
            // textBoxM34
            // 
            this.textBoxM34.Location = new System.Drawing.Point(138, 53);
            this.textBoxM34.Name = "textBoxM34";
            this.textBoxM34.Size = new System.Drawing.Size(39, 20);
            this.textBoxM34.TabIndex = 11;
            // 
            // textBoxM41
            // 
            this.textBoxM41.Location = new System.Drawing.Point(3, 78);
            this.textBoxM41.Name = "textBoxM41";
            this.textBoxM41.Size = new System.Drawing.Size(39, 20);
            this.textBoxM41.TabIndex = 12;
            // 
            // textBoxM42
            // 
            this.textBoxM42.Location = new System.Drawing.Point(48, 78);
            this.textBoxM42.Name = "textBoxM42";
            this.textBoxM42.Size = new System.Drawing.Size(39, 20);
            this.textBoxM42.TabIndex = 13;
            // 
            // textBoxM43
            // 
            this.textBoxM43.Location = new System.Drawing.Point(93, 78);
            this.textBoxM43.Name = "textBoxM43";
            this.textBoxM43.Size = new System.Drawing.Size(39, 20);
            this.textBoxM43.TabIndex = 14;
            // 
            // textBoxM44
            // 
            this.textBoxM44.Location = new System.Drawing.Point(138, 78);
            this.textBoxM44.Name = "textBoxM44";
            this.textBoxM44.Size = new System.Drawing.Size(39, 20);
            this.textBoxM44.TabIndex = 15;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tableLayoutPanel2);
            this.panel2.Location = new System.Drawing.Point(216, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(210, 117);
            this.panel2.TabIndex = 3;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.Controls.Add(this.textBoxMV44, 3, 3);
            this.tableLayoutPanel2.Controls.Add(this.textBoxMV43, 2, 3);
            this.tableLayoutPanel2.Controls.Add(this.textBoxMV42, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.textBoxMV41, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.textBoxMV34, 3, 2);
            this.tableLayoutPanel2.Controls.Add(this.textBoxMV33, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.textBoxMV32, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.textBoxMV31, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.textBoxMV24, 3, 1);
            this.tableLayoutPanel2.Controls.Add(this.textBoxMV23, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.textBoxMV22, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.textBoxMV21, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.textBoxMV14, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.textBoxMV13, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.textBoxMV12, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.textBoxMV11, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(7, 12);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(181, 100);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // textBoxMV44
            // 
            this.textBoxMV44.Location = new System.Drawing.Point(138, 78);
            this.textBoxMV44.Name = "textBoxMV44";
            this.textBoxMV44.Size = new System.Drawing.Size(39, 20);
            this.textBoxMV44.TabIndex = 15;
            // 
            // textBoxMV43
            // 
            this.textBoxMV43.Location = new System.Drawing.Point(93, 78);
            this.textBoxMV43.Name = "textBoxMV43";
            this.textBoxMV43.Size = new System.Drawing.Size(39, 20);
            this.textBoxMV43.TabIndex = 14;
            // 
            // textBoxMV42
            // 
            this.textBoxMV42.Location = new System.Drawing.Point(48, 78);
            this.textBoxMV42.Name = "textBoxMV42";
            this.textBoxMV42.Size = new System.Drawing.Size(39, 20);
            this.textBoxMV42.TabIndex = 13;
            // 
            // textBoxMV41
            // 
            this.textBoxMV41.Location = new System.Drawing.Point(3, 78);
            this.textBoxMV41.Name = "textBoxMV41";
            this.textBoxMV41.Size = new System.Drawing.Size(39, 20);
            this.textBoxMV41.TabIndex = 12;
            // 
            // textBoxMV34
            // 
            this.textBoxMV34.Location = new System.Drawing.Point(138, 53);
            this.textBoxMV34.Name = "textBoxMV34";
            this.textBoxMV34.Size = new System.Drawing.Size(39, 20);
            this.textBoxMV34.TabIndex = 11;
            // 
            // textBoxMV33
            // 
            this.textBoxMV33.Location = new System.Drawing.Point(93, 53);
            this.textBoxMV33.Name = "textBoxMV33";
            this.textBoxMV33.Size = new System.Drawing.Size(39, 20);
            this.textBoxMV33.TabIndex = 10;
            // 
            // textBoxMV32
            // 
            this.textBoxMV32.Location = new System.Drawing.Point(48, 53);
            this.textBoxMV32.Name = "textBoxMV32";
            this.textBoxMV32.Size = new System.Drawing.Size(39, 20);
            this.textBoxMV32.TabIndex = 9;
            // 
            // textBoxMV31
            // 
            this.textBoxMV31.Location = new System.Drawing.Point(3, 53);
            this.textBoxMV31.Name = "textBoxMV31";
            this.textBoxMV31.Size = new System.Drawing.Size(39, 20);
            this.textBoxMV31.TabIndex = 8;
            // 
            // textBoxMV24
            // 
            this.textBoxMV24.Location = new System.Drawing.Point(138, 28);
            this.textBoxMV24.Name = "textBoxMV24";
            this.textBoxMV24.Size = new System.Drawing.Size(39, 20);
            this.textBoxMV24.TabIndex = 7;
            // 
            // textBoxMV23
            // 
            this.textBoxMV23.Location = new System.Drawing.Point(93, 28);
            this.textBoxMV23.Name = "textBoxMV23";
            this.textBoxMV23.Size = new System.Drawing.Size(39, 20);
            this.textBoxMV23.TabIndex = 6;
            // 
            // textBoxMV22
            // 
            this.textBoxMV22.Location = new System.Drawing.Point(48, 28);
            this.textBoxMV22.Name = "textBoxMV22";
            this.textBoxMV22.Size = new System.Drawing.Size(39, 20);
            this.textBoxMV22.TabIndex = 5;
            // 
            // textBoxMV21
            // 
            this.textBoxMV21.Location = new System.Drawing.Point(3, 28);
            this.textBoxMV21.Name = "textBoxMV21";
            this.textBoxMV21.Size = new System.Drawing.Size(39, 20);
            this.textBoxMV21.TabIndex = 4;
            // 
            // textBoxMV14
            // 
            this.textBoxMV14.Location = new System.Drawing.Point(138, 3);
            this.textBoxMV14.Name = "textBoxMV14";
            this.textBoxMV14.Size = new System.Drawing.Size(39, 20);
            this.textBoxMV14.TabIndex = 3;
            // 
            // textBoxMV13
            // 
            this.textBoxMV13.Location = new System.Drawing.Point(93, 3);
            this.textBoxMV13.Name = "textBoxMV13";
            this.textBoxMV13.Size = new System.Drawing.Size(39, 20);
            this.textBoxMV13.TabIndex = 2;
            // 
            // textBoxMV12
            // 
            this.textBoxMV12.Location = new System.Drawing.Point(48, 3);
            this.textBoxMV12.Name = "textBoxMV12";
            this.textBoxMV12.Size = new System.Drawing.Size(39, 20);
            this.textBoxMV12.TabIndex = 1;
            // 
            // textBoxMV11
            // 
            this.textBoxMV11.Location = new System.Drawing.Point(3, 3);
            this.textBoxMV11.Name = "textBoxMV11";
            this.textBoxMV11.Size = new System.Drawing.Size(39, 20);
            this.textBoxMV11.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(868, 522);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.glControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private OpenTK.GLControl glControl1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox textBoxM44;
        private System.Windows.Forms.TextBox textBoxM43;
        private System.Windows.Forms.TextBox textBoxM42;
        private System.Windows.Forms.TextBox textBoxM41;
        private System.Windows.Forms.TextBox textBoxM34;
        private System.Windows.Forms.TextBox textBoxM33;
        private System.Windows.Forms.TextBox textBoxM32;
        private System.Windows.Forms.TextBox textBoxM31;
        private System.Windows.Forms.TextBox textBoxM24;
        private System.Windows.Forms.TextBox textBoxM23;
        private System.Windows.Forms.TextBox textBoxM22;
        private System.Windows.Forms.TextBox textBoxM21;
        private System.Windows.Forms.TextBox textBoxM14;
        private System.Windows.Forms.TextBox textBoxM13;
        private System.Windows.Forms.TextBox textBoxM12;
        private System.Windows.Forms.TextBox textBoxM11;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TextBox textBoxMV44;
        private System.Windows.Forms.TextBox textBoxMV43;
        private System.Windows.Forms.TextBox textBoxMV42;
        private System.Windows.Forms.TextBox textBoxMV41;
        private System.Windows.Forms.TextBox textBoxMV34;
        private System.Windows.Forms.TextBox textBoxMV33;
        private System.Windows.Forms.TextBox textBoxMV32;
        private System.Windows.Forms.TextBox textBoxMV31;
        private System.Windows.Forms.TextBox textBoxMV24;
        private System.Windows.Forms.TextBox textBoxMV23;
        private System.Windows.Forms.TextBox textBoxMV22;
        private System.Windows.Forms.TextBox textBoxMV21;
        private System.Windows.Forms.TextBox textBoxMV14;
        private System.Windows.Forms.TextBox textBoxMV13;
        private System.Windows.Forms.TextBox textBoxMV12;
        private System.Windows.Forms.TextBox textBoxMV11;
    }
}

