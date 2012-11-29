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
            this.button1 = new System.Windows.Forms.Button();
            this.ddlPapel = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnMaisEspacoAcima = new System.Windows.Forms.Button();
            this.btnMaisEspacoAbaixo = new System.Windows.Forms.Button();
            this.btnMenosEspacoAcima = new System.Windows.Forms.Button();
            this.btnMenosEspacoAbaixo = new System.Windows.Forms.Button();
            this.btnMaisEsquerda = new System.Windows.Forms.Button();
            this.btnMuitoMaisEsquerda = new System.Windows.Forms.Button();
            this.btnMuitoMaisDireita = new System.Windows.Forms.Button();
            this.btnMaisDireita = new System.Windows.Forms.Button();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.lblQtdDias = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblVersoes = new System.Windows.Forms.Label();
            this.lblDataFinalResumo = new System.Windows.Forms.Label();
            this.lblDataInicialResumo = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblTotalDados = new System.Windows.Forms.Label();
            this.lblAcertosAlta = new System.Windows.Forms.Label();
            this.lblAcertosbaixa = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblAcertosBaixaParcial = new System.Windows.Forms.Label();
            this.lblAcertosAltaParcial = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lblDataFinalParcial = new System.Windows.Forms.Label();
            this.lblDataInicialParcial = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.grafico)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // grafico
            // 
            this.grafico.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.grafico.Location = new System.Drawing.Point(12, 68);
            this.grafico.Name = "grafico";
            this.grafico.Size = new System.Drawing.Size(1272, 586);
            this.grafico.TabIndex = 0;
            this.grafico.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(635, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Quantidade de dias:";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(15, 26);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(98, 36);
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
            this.ddlPapel.Location = new System.Drawing.Point(52, 0);
            this.ddlPapel.Name = "ddlPapel";
            this.ddlPapel.Size = new System.Drawing.Size(61, 21);
            this.ddlPapel.TabIndex = 102;
            this.ddlPapel.Text = "ETER3";
            this.ddlPapel.SelectedIndexChanged += new System.EventHandler(this.ddlPapel_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Papel";
            // 
            // btnMaisEspacoAcima
            // 
            this.btnMaisEspacoAcima.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMaisEspacoAcima.Location = new System.Drawing.Point(1290, 68);
            this.btnMaisEspacoAcima.Name = "btnMaisEspacoAcima";
            this.btnMaisEspacoAcima.Size = new System.Drawing.Size(33, 31);
            this.btnMaisEspacoAcima.TabIndex = 103;
            this.btnMaisEspacoAcima.Text = "+";
            this.btnMaisEspacoAcima.UseVisualStyleBackColor = true;
            this.btnMaisEspacoAcima.Click += new System.EventHandler(this.btnMaisEspacoAcima_Click);
            // 
            // btnMaisEspacoAbaixo
            // 
            this.btnMaisEspacoAbaixo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMaisEspacoAbaixo.Location = new System.Drawing.Point(1290, 623);
            this.btnMaisEspacoAbaixo.Name = "btnMaisEspacoAbaixo";
            this.btnMaisEspacoAbaixo.Size = new System.Drawing.Size(33, 31);
            this.btnMaisEspacoAbaixo.TabIndex = 104;
            this.btnMaisEspacoAbaixo.Text = "+";
            this.btnMaisEspacoAbaixo.UseVisualStyleBackColor = true;
            this.btnMaisEspacoAbaixo.Click += new System.EventHandler(this.btnMaisEspacoAbaixo_Click);
            // 
            // btnMenosEspacoAcima
            // 
            this.btnMenosEspacoAcima.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMenosEspacoAcima.Location = new System.Drawing.Point(1290, 105);
            this.btnMenosEspacoAcima.Name = "btnMenosEspacoAcima";
            this.btnMenosEspacoAcima.Size = new System.Drawing.Size(33, 31);
            this.btnMenosEspacoAcima.TabIndex = 105;
            this.btnMenosEspacoAcima.Text = "-";
            this.btnMenosEspacoAcima.UseVisualStyleBackColor = true;
            this.btnMenosEspacoAcima.Click += new System.EventHandler(this.btnMenosEspacoAcima_Click);
            // 
            // btnMenosEspacoAbaixo
            // 
            this.btnMenosEspacoAbaixo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMenosEspacoAbaixo.Location = new System.Drawing.Point(1290, 586);
            this.btnMenosEspacoAbaixo.Name = "btnMenosEspacoAbaixo";
            this.btnMenosEspacoAbaixo.Size = new System.Drawing.Size(33, 31);
            this.btnMenosEspacoAbaixo.TabIndex = 106;
            this.btnMenosEspacoAbaixo.Text = "-";
            this.btnMenosEspacoAbaixo.UseVisualStyleBackColor = true;
            this.btnMenosEspacoAbaixo.Click += new System.EventHandler(this.btnMenosEspacoAbaixo_Click);
            // 
            // btnMaisEsquerda
            // 
            this.btnMaisEsquerda.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMaisEsquerda.Location = new System.Drawing.Point(12, 665);
            this.btnMaisEsquerda.Name = "btnMaisEsquerda";
            this.btnMaisEsquerda.Size = new System.Drawing.Size(18, 31);
            this.btnMaisEsquerda.TabIndex = 107;
            this.btnMaisEsquerda.Text = "<";
            this.btnMaisEsquerda.UseVisualStyleBackColor = true;
            this.btnMaisEsquerda.Click += new System.EventHandler(this.btnMaisEsquerda_Click);
            // 
            // btnMuitoMaisEsquerda
            // 
            this.btnMuitoMaisEsquerda.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMuitoMaisEsquerda.Location = new System.Drawing.Point(31, 665);
            this.btnMuitoMaisEsquerda.Name = "btnMuitoMaisEsquerda";
            this.btnMuitoMaisEsquerda.Size = new System.Drawing.Size(40, 31);
            this.btnMuitoMaisEsquerda.TabIndex = 108;
            this.btnMuitoMaisEsquerda.Text = "<<";
            this.btnMuitoMaisEsquerda.UseVisualStyleBackColor = true;
            this.btnMuitoMaisEsquerda.Click += new System.EventHandler(this.btnMuitoMaisEsquerda_Click);
            // 
            // btnMuitoMaisDireita
            // 
            this.btnMuitoMaisDireita.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMuitoMaisDireita.Location = new System.Drawing.Point(1225, 665);
            this.btnMuitoMaisDireita.Name = "btnMuitoMaisDireita";
            this.btnMuitoMaisDireita.Size = new System.Drawing.Size(40, 31);
            this.btnMuitoMaisDireita.TabIndex = 112;
            this.btnMuitoMaisDireita.Text = ">>\r\n";
            this.btnMuitoMaisDireita.UseVisualStyleBackColor = true;
            this.btnMuitoMaisDireita.Click += new System.EventHandler(this.btnMuitoMaisDireita_Click);
            // 
            // btnMaisDireita
            // 
            this.btnMaisDireita.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMaisDireita.Location = new System.Drawing.Point(1266, 665);
            this.btnMaisDireita.Name = "btnMaisDireita";
            this.btnMaisDireita.Size = new System.Drawing.Size(18, 31);
            this.btnMaisDireita.TabIndex = 111;
            this.btnMaisDireita.Text = ">";
            this.btnMaisDireita.UseVisualStyleBackColor = true;
            this.btnMaisDireita.Click += new System.EventHandler(this.btnMaisDireita_Click);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(298, 24);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(818, 42);
            this.trackBar1.TabIndex = 113;
            this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            // 
            // lblQtdDias
            // 
            this.lblQtdDias.AutoSize = true;
            this.lblQtdDias.Location = new System.Drawing.Point(732, 9);
            this.lblQtdDias.Name = "lblQtdDias";
            this.lblQtdDias.Size = new System.Drawing.Size(19, 13);
            this.lblQtdDias.TabIndex = 114;
            this.lblQtdDias.Text = "30";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblAcertosbaixa);
            this.groupBox1.Controls.Add(this.lblAcertosAlta);
            this.groupBox1.Controls.Add(this.lblTotalDados);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.lblVersoes);
            this.groupBox1.Controls.Add(this.lblDataFinalResumo);
            this.groupBox1.Controls.Add(this.lblDataInicialResumo);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(147, 657);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(590, 62);
            this.groupBox1.TabIndex = 115;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Resumo";
            // 
            // lblVersoes
            // 
            this.lblVersoes.AutoSize = true;
            this.lblVersoes.Location = new System.Drawing.Point(13, 13);
            this.lblVersoes.Name = "lblVersoes";
            this.lblVersoes.Size = new System.Drawing.Size(35, 13);
            this.lblVersoes.TabIndex = 4;
            this.lblVersoes.Text = "label5";
            // 
            // lblDataFinalResumo
            // 
            this.lblDataFinalResumo.AutoSize = true;
            this.lblDataFinalResumo.Location = new System.Drawing.Point(84, 44);
            this.lblDataFinalResumo.Name = "lblDataFinalResumo";
            this.lblDataFinalResumo.Size = new System.Drawing.Size(35, 13);
            this.lblDataFinalResumo.TabIndex = 3;
            this.lblDataFinalResumo.Text = "label6";
            // 
            // lblDataInicialResumo
            // 
            this.lblDataInicialResumo.AutoSize = true;
            this.lblDataInicialResumo.Location = new System.Drawing.Point(8, 44);
            this.lblDataInicialResumo.Name = "lblDataInicialResumo";
            this.lblDataInicialResumo.Size = new System.Drawing.Size(35, 13);
            this.lblDataInicialResumo.TabIndex = 2;
            this.lblDataInicialResumo.Text = "label5";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(91, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Data final";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Data inicial";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(243, 29);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Acertos Tend.Alta";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(349, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Acertos Tend.Baixa";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(161, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Total Dados";
            // 
            // lblTotalDados
            // 
            this.lblTotalDados.AutoSize = true;
            this.lblTotalDados.Location = new System.Drawing.Point(176, 43);
            this.lblTotalDados.Name = "lblTotalDados";
            this.lblTotalDados.Size = new System.Drawing.Size(35, 13);
            this.lblTotalDados.TabIndex = 7;
            this.lblTotalDados.Text = "label8";
            // 
            // lblAcertosAlta
            // 
            this.lblAcertosAlta.AutoSize = true;
            this.lblAcertosAlta.Location = new System.Drawing.Point(254, 44);
            this.lblAcertosAlta.Name = "lblAcertosAlta";
            this.lblAcertosAlta.Size = new System.Drawing.Size(35, 13);
            this.lblAcertosAlta.TabIndex = 7;
            this.lblAcertosAlta.Text = "label8";
            // 
            // lblAcertosbaixa
            // 
            this.lblAcertosbaixa.AutoSize = true;
            this.lblAcertosbaixa.Location = new System.Drawing.Point(359, 44);
            this.lblAcertosbaixa.Name = "lblAcertosbaixa";
            this.lblAcertosbaixa.Size = new System.Drawing.Size(35, 13);
            this.lblAcertosbaixa.TabIndex = 7;
            this.lblAcertosbaixa.Text = "label8";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblAcertosBaixaParcial);
            this.groupBox2.Controls.Add(this.lblAcertosAltaParcial);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.lblDataFinalParcial);
            this.groupBox2.Controls.Add(this.lblDataInicialParcial);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Location = new System.Drawing.Point(749, 657);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(400, 62);
            this.groupBox2.TabIndex = 116;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Resumo Parcial";
            // 
            // lblAcertosBaixaParcial
            // 
            this.lblAcertosBaixaParcial.AutoSize = true;
            this.lblAcertosBaixaParcial.Location = new System.Drawing.Point(291, 35);
            this.lblAcertosBaixaParcial.Name = "lblAcertosBaixaParcial";
            this.lblAcertosBaixaParcial.Size = new System.Drawing.Size(35, 13);
            this.lblAcertosBaixaParcial.TabIndex = 7;
            this.lblAcertosBaixaParcial.Text = "label8";
            // 
            // lblAcertosAltaParcial
            // 
            this.lblAcertosAltaParcial.AutoSize = true;
            this.lblAcertosAltaParcial.Location = new System.Drawing.Point(186, 35);
            this.lblAcertosAltaParcial.Name = "lblAcertosAltaParcial";
            this.lblAcertosAltaParcial.Size = new System.Drawing.Size(35, 13);
            this.lblAcertosAltaParcial.TabIndex = 7;
            this.lblAcertosAltaParcial.Text = "label8";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(281, 20);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(100, 13);
            this.label12.TabIndex = 5;
            this.label12.Text = "Acertos Tend.Baixa";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(175, 20);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(92, 13);
            this.label13.TabIndex = 5;
            this.label13.Text = "Acertos Tend.Alta";
            // 
            // lblDataFinalParcial
            // 
            this.lblDataFinalParcial.AutoSize = true;
            this.lblDataFinalParcial.Location = new System.Drawing.Point(93, 35);
            this.lblDataFinalParcial.Name = "lblDataFinalParcial";
            this.lblDataFinalParcial.Size = new System.Drawing.Size(35, 13);
            this.lblDataFinalParcial.TabIndex = 3;
            this.lblDataFinalParcial.Text = "label6";
            // 
            // lblDataInicialParcial
            // 
            this.lblDataInicialParcial.AutoSize = true;
            this.lblDataInicialParcial.Location = new System.Drawing.Point(17, 35);
            this.lblDataInicialParcial.Name = "lblDataInicialParcial";
            this.lblDataInicialParcial.Size = new System.Drawing.Size(35, 13);
            this.lblDataInicialParcial.TabIndex = 2;
            this.lblDataInicialParcial.Text = "label5";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(100, 20);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(52, 13);
            this.label17.TabIndex = 1;
            this.label17.Text = "Data final";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(19, 20);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(59, 13);
            this.label18.TabIndex = 0;
            this.label18.Text = "Data inicial";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1325, 728);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblQtdDias);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.btnMuitoMaisDireita);
            this.Controls.Add(this.btnMaisDireita);
            this.Controls.Add(this.btnMuitoMaisEsquerda);
            this.Controls.Add(this.btnMaisEsquerda);
            this.Controls.Add(this.btnMenosEspacoAbaixo);
            this.Controls.Add(this.btnMenosEspacoAcima);
            this.Controls.Add(this.btnMaisEspacoAbaixo);
            this.Controls.Add(this.btnMaisEspacoAcima);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ddlPapel);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.grafico);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.grafico)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox grafico;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox ddlPapel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnMaisEspacoAcima;
        private System.Windows.Forms.Button btnMaisEspacoAbaixo;
        private System.Windows.Forms.Button btnMenosEspacoAcima;
        private System.Windows.Forms.Button btnMenosEspacoAbaixo;
        private System.Windows.Forms.Button btnMaisEsquerda;
        private System.Windows.Forms.Button btnMuitoMaisEsquerda;
        private System.Windows.Forms.Button btnMuitoMaisDireita;
        private System.Windows.Forms.Button btnMaisDireita;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label lblQtdDias;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblDataFinalResumo;
        private System.Windows.Forms.Label lblDataInicialResumo;
        private System.Windows.Forms.Label lblVersoes;
        private System.Windows.Forms.Label lblAcertosbaixa;
        private System.Windows.Forms.Label lblAcertosAlta;
        private System.Windows.Forms.Label lblTotalDados;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblAcertosBaixaParcial;
        private System.Windows.Forms.Label lblAcertosAltaParcial;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblDataFinalParcial;
        private System.Windows.Forms.Label lblDataInicialParcial;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;


    }
}

