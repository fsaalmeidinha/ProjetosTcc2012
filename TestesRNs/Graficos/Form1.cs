using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TestesRNs.Modelo;
using TestesRNs.RedeNeural;

namespace Graficos
{
    public partial class Form1 : Form
    {
        private int janelaVerticalEmReais = 1;
        string papel = "ETER3";//PETR4,ETER3,GOLL4,VALE5,NATU3
        //int qtdDiasPrever = 100;
        //DateTime dataInicialPrevisao = new DateTime(2011, 09, 1);
        //Versao versao = Versao.V602;

        public Form1()
        {
            InitializeComponent();
            //PreencherComboBoxVersao();
            DesenharGrafico();
        }

        void DesenharGrafico()
        {
            DateTime dtInicial;
            DateTime dtFinal;
            int tamanhoTendencia;
            List<double[]> previsao = RNHelper.PreverAtivo(out dtInicial, out dtFinal, out tamanhoTendencia, papel);//;
            int qtdDiasPrever = previsao.Count;

            grafico.Image = new Bitmap(grafico.Width, grafico.Height);
            Graphics g = Graphics.FromImage(grafico.Image);

            int minimoEntreOsDois = Convert.ToInt32(previsao.Min(prev => prev[0]));
            int maximoEntreOsDois = Convert.ToInt32(previsao.Max(prev => prev[0]));

            //A cada 40 pixels, há alteração de 1 real 
            int multiplicadorY = grafico.Height / (maximoEntreOsDois - minimoEntreOsDois);//15;
            //if (multiplicadorY < 10) multiplicadorY = 10;
            if (multiplicadorY > 50) multiplicadorY = multiplicadorY / 2;
            //A cada 10 pixels, há alteração de 1 dia da cotação
            int multiplicadorX = Convert.ToInt32(txtEscalaX.Text);

            //Espacamento em reais antes do inicio do dado de menor valor
            int espacamentoAbaixo = 1;//8;
            DesenharLinharAuxiliaresDoGrafico(g, minimoEntreOsDois - espacamentoAbaixo, maximoEntreOsDois, 1, multiplicadorY, multiplicadorX);

            minimoEntreOsDois *= multiplicadorY;

            Pen pen1 = new Pen(Color.Red) { Width = 3f };
            for (int i = 0; i < qtdDiasPrever - 1; i++)
            {
                Point ponto1Real = new Point(40 + (i * multiplicadorX), grafico.Height - Convert.ToInt32((previsao[i][0] + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);
                Point ponto2Real = new Point(40 + ((i + 1) * multiplicadorX), grafico.Height - Convert.ToInt32((previsao[i + 1][0] + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);

                //Point ponto1Previsto = new Point(40 + (i * multiplicadorX), grafico.Height - Convert.ToInt32((previsao[i][1] + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);
                //Point ponto2Previsto = new Point(40 + ((i + 1) * multiplicadorX), grafico.Height - Convert.ToInt32((previsao[i + 1][1] + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);

                g.DrawLine(pen1, ponto1Real, ponto2Real);


                if (previsao[i][1] != 0 && i + tamanhoTendencia < previsao.Count)
                {
                    Point ponto1Tendencia = new Point(40 + (i * multiplicadorX), grafico.Height - Convert.ToInt32((previsao[i][0] + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);
                    Point ponto2Tendencia = new Point(40 + ((i + tamanhoTendencia) * multiplicadorX), grafico.Height - Convert.ToInt32((previsao[i + tamanhoTendencia][0] + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);

                    Pen pen2 = null;
                    if (previsao[i][1] == 1)//OK - A Rede preveu que iria subir o valor do ativo e subiu..
                        pen2 = new Pen(Color.Green) { Width = 2f };
                    if (previsao[i][1] == 2)//NOK - A Rede preveu que iria subir o valor do ativo e caiu..
                        pen2 = new Pen(Color.Yellow) { Width = 2f };
                    if (previsao[i][1] == 3)//OK - A Rede preveu que iria descer o valor do ativo e desceu..
                        pen2 = new Pen(Color.Orange) { Width = 2f };
                    if (previsao[i][1] == 4)//NOK - A Rede preveu que iria descer o valor do ativo e subiu..
                        pen2 = new Pen(Color.Blue) { Width = 2f };

                    g.DrawLine(pen2, ponto1Tendencia, ponto2Tendencia);
                }
                //g.DrawLine(pen2, ponto1Previsto, ponto2Previsto);

                ////Seguiu a tendencia
                //if ((ponto2Real.Y > ponto1Real.Y && ponto2Previsto.Y > ponto1Real.Y)
                // || (ponto2Real.Y < ponto1Real.Y && ponto2Previsto.Y < ponto1Real.Y))
                //    g.DrawLine(pen3, ponto1Real, ponto2Previsto);
                //else//Não seguiu a tendencia
                //    g.DrawLine(pen4, ponto1Real, ponto2Previsto);
            }
        }

        /*
        void DesenharBollinger()
        {
            List<DadoBE> dadosBE = DadoBE.PegarTodos(papel).Where(cot => cot.DataGeracao >= new DateTime(2012, 4, 1) && cot.DataGeracao <= new DateTime(2012, 10, 1)).ToList();

            grafico.Image = new Bitmap(grafico.Width, grafico.Height);
            Graphics g = Graphics.FromImage(grafico.Image);

            int minimoEntreOsDois = Convert.ToInt32(Math.Min(dadosBE.Min(prev => prev.PrecoFechamento), dadosBE.Min(prev => prev.BandaInferior)));
            int maximoEntreOsDois = Convert.ToInt32(Math.Max(dadosBE.Max(prev => prev.PrecoFechamento), dadosBE.Max(prev => prev.BandaSuperior)));

            //A cada 40 pixels, há alteração de 1 real 
            int multiplicadorY = grafico.Height / (maximoEntreOsDois - minimoEntreOsDois);//15;
            //if (multiplicadorY < 10) multiplicadorY = 10;
            if (multiplicadorY > 50) multiplicadorY = multiplicadorY / 2;
            //A cada 10 pixels, há alteração de 1 dia da cotação
            int multiplicadorX = Convert.ToInt32(txtEscalaX.Text);

            //Espacamento em reais antes do inicio do dado de menor valor
            int espacamentoAbaixo = 1;//8;
            DesenharLinharAuxiliaresDoGrafico(g, minimoEntreOsDois - espacamentoAbaixo, maximoEntreOsDois, 1, multiplicadorY, multiplicadorX);

            minimoEntreOsDois *= multiplicadorY;

            Pen pen1 = new Pen(Color.Red) { Width = 3f };
            Pen pen2 = new Pen(Color.DarkBlue) { Width = 2f };
            Pen pen3 = new Pen(Color.DarkGreen) { Width = 2f };
            Pen pen4 = new Pen(Color.DarkOrange) { Width = 2f };
            for (int i = 0; i < qtdDiasPrever - 1; i++)
            {
                Point ponto1Real = new Point(40 + (i * multiplicadorX), grafico.Height - Convert.ToInt32((dadosBE[i].PrecoFechamento + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);
                Point ponto2Real = new Point(40 + ((i + 1) * multiplicadorX), grafico.Height - Convert.ToInt32((dadosBE[i + 1].PrecoFechamento + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);

                Point ponto1Central = new Point(40 + (i * multiplicadorX), grafico.Height - Convert.ToInt32((dadosBE[i].BandaCentral + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);
                Point ponto2Central = new Point(40 + ((i + 1) * multiplicadorX), grafico.Height - Convert.ToInt32((dadosBE[i + 1].BandaCentral + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);

                Point ponto1Inferior = new Point(40 + (i * multiplicadorX), grafico.Height - Convert.ToInt32((dadosBE[i].BandaInferior + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);
                Point ponto2Inferior = new Point(40 + ((i + 1) * multiplicadorX), grafico.Height - Convert.ToInt32((dadosBE[i + 1].BandaInferior + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);

                Point ponto1Superior = new Point(40 + (i * multiplicadorX), grafico.Height - Convert.ToInt32((dadosBE[i].BandaSuperior + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);
                Point ponto2Superior = new Point(40 + ((i + 1) * multiplicadorX), grafico.Height - Convert.ToInt32((dadosBE[i + 1].BandaSuperior + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);



                g.DrawLine(pen1, ponto1Real, ponto2Real);
                //g.DrawLine(pen2, ponto1Previsto, ponto2Previsto);


                g.DrawLine(pen3, ponto1Central, ponto2Central);
                g.DrawLine(pen2, ponto1Inferior, ponto2Inferior);
                g.DrawLine(pen2, ponto1Superior, ponto2Superior);
            }
        }
        */

        void DesenharLinharAuxiliaresDoGrafico(Graphics g, int minimo, int maximo, int unidade, int multiplicadorEscalaY, int multiplicadorEscalaX)
        {
            Pen pen = new Pen(Color.LightGray);

            //HORIZONTAL
            for (int i = grafico.Height - 1; i > 1 - 1; i -= multiplicadorEscalaY * janelaVerticalEmReais)
            {
                Brush b = pen.Color == Color.LightGray ? Brushes.LightGray : (pen.Color == Color.LightSkyBlue ? Brushes.LightSkyBlue : Brushes.LightGreen);

                g.DrawString("R$ " + minimo.ToString(), new Font("Arial", 8), b, (float)0, (float)i);
                minimo += janelaVerticalEmReais;

                Point pontoReal1 = new Point(0, i);
                Point pontoReal2 = new Point(grafico.Width, i);

                g.DrawLine(pen, pontoReal1, pontoReal2);

                //alterna a cor da linha
                if (pen.Color == Color.LightGray)
                    pen.Color = Color.LightSkyBlue;
                else if (pen.Color == Color.LightSkyBlue)
                    pen.Color = Color.LightGreen;
                else
                    pen.Color = Color.LightGray;
            }

            //VERTICAL
            for (int i = 40; i < grafico.Width; i += multiplicadorEscalaX)
            {
                Point pontoReal1 = new Point(i, 0);
                Point pontoReal2 = new Point(i, grafico.Height);

                g.DrawLine(pen, pontoReal1, pontoReal2);

                //alterna a cor da linha
                if (pen.Color == Color.LightGray)
                    pen.Color = Color.LightSkyBlue;
                else
                    pen.Color = Color.LightGray;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //DesenharBollinger();
            DesenharGrafico();
        }

        private void ddlPapel_SelectedIndexChanged(object sender, EventArgs e)
        {
            papel = ((ComboBox)sender).SelectedItem.ToString();
            DesenharGrafico();
        }

        //private void PreencherComboBoxVersao()
        //{
        //    ddlVersao.DataSource = Enum.GetValues(typeof(Versao));
        //    ddlVersao.SelectedIndex = 0;
        //}

        //private void ddlVersao_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    versao = ((Versao)Convert.ToInt32(((ComboBox)sender).SelectedItem));
        //    DesenharGrafico();
        //}
    }
}
