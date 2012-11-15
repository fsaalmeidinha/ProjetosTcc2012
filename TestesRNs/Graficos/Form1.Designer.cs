namespace Graficos
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grafico = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtEscalaX = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.ddlPapel = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.grafico)).BeginInit();
            this.SuspendLayout();
            // 
            // grafico
            // 
            this.grafico.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.grafico.Location = new System.Drawing.Point(12, 47);
            this.grafico.Name = "grafico";
            this.grafico.Size = new System.Drawing.Size(1247, 514);
            this.grafico.TabIndex = 0;
            this.grafico.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Escala em X:";
            // 
            // txtEscalaX
            // 
            this.txtEscalaX.Location = new System.Drawing.Point(80, 3);
            this.txtEscalaX.Name = "txtEscalaX";
            this.txtEscalaX.Size = new System.Drawing.Size(24, 20);
            this.txtEscalaX.TabIndex = 100;
            this.txtEscalaX.Text = "10";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 25);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 19);
            this.button1.TabIndex = 101;
            this.button1.Text = "Gerar Grafico";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ddlPapel
            // 
            this.ddlPapel.FormattingEnabled = true;
            this.ddlPapel.Items.AddRange(new object[] {
            "BVSP",
            "ETER3",
            "GOLL4",
            "NATU3",
            "PETR4",
            "VALE5"});
            this.ddlPapel.Location = new System.Drawing.Point(124, 23);
            this.ddlPapel.Name = "ddlPapel";
            this.ddlPapel.Size = new System.Drawing.Size(61, 21);
            this.ddlPapel.TabIndex = 102;
            this.ddlPapel.Text = "ETER3";
            this.ddlPapel.SelectedIndexChanged += new System.EventHandler(this.ddlPapel_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(121, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Papel";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 573);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ddlPapel);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtEscalaX);
            this.Controls.Add(this.grafico);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.grafico)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox grafico;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtEscalaX;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox ddlPapel;
        private System.Windows.Forms.Label label1;


    }
}

