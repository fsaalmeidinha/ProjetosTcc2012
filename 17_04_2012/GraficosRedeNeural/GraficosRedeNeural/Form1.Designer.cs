namespace GraficosRedeNeural
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
            this.grafico1 = new System.Windows.Forms.PictureBox();
            this.txtX = new System.Windows.Forms.TextBox();
            this.txtY = new System.Windows.Forms.TextBox();
            this.btnDesenhar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.grafico1)).BeginInit();
            this.SuspendLayout();
            // 
            // grafico1
            // 
            this.grafico1.Location = new System.Drawing.Point(12, 12);
            this.grafico1.Name = "grafico1";
            this.grafico1.Size = new System.Drawing.Size(1420, 748);
            this.grafico1.TabIndex = 0;
            this.grafico1.TabStop = false;
            // 
            // txtX
            // 
            this.txtX.Location = new System.Drawing.Point(1359, 12);
            this.txtX.Name = "txtX";
            this.txtX.Size = new System.Drawing.Size(73, 20);
            this.txtX.TabIndex = 1;
            this.txtX.Text = "4";
            // 
            // txtY
            // 
            this.txtY.Location = new System.Drawing.Point(1359, 47);
            this.txtY.Name = "txtY";
            this.txtY.Size = new System.Drawing.Size(73, 20);
            this.txtY.TabIndex = 1;
            this.txtY.Text = "5";
            // 
            // btnDesenhar
            // 
            this.btnDesenhar.Location = new System.Drawing.Point(1359, 74);
            this.btnDesenhar.Name = "btnDesenhar";
            this.btnDesenhar.Size = new System.Drawing.Size(75, 23);
            this.btnDesenhar.TabIndex = 2;
            this.btnDesenhar.Text = "Desenhar";
            this.btnDesenhar.UseVisualStyleBackColor = true;
            this.btnDesenhar.Click += new System.EventHandler(this.btnDesenhar_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1444, 683);
            this.Controls.Add(this.btnDesenhar);
            this.Controls.Add(this.txtY);
            this.Controls.Add(this.txtX);
            this.Controls.Add(this.grafico1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.grafico1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox grafico1;
        private System.Windows.Forms.TextBox txtX;
        private System.Windows.Forms.TextBox txtY;
        private System.Windows.Forms.Button btnDesenhar;
    }
}

