using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Graficos
{
    public partial class Form1 : Form
    {
        //De quantos em quantos reais o gráfico será gerado em Y
        int janelaVerticalEmReais = 1;
        List<double[]> previsao;

        public Form1()
        {
            InitializeComponent();
            previsao = PrevisaoFinanceiraHelper.PrevisaoFinanceira.PreverCotacaoAtivo("PETR4", 120);
            DesenharGrafico();
            AlimentarGridTaxasAcerto();
        }

        private void AlimentarGridTaxasAcerto()
        {
            DataTable dt = new DataTable();
            DataRow drReal = dt.NewRow();
            dt.Rows.Add(drReal);
            DataRow drPrevisto = dt.NewRow();
            dt.Rows.Add(drPrevisto);
            DataRow drTaxaAcerto = dt.NewRow();
            dt.Rows.Add(drTaxaAcerto);
            DataColumn dcDescricao = new DataColumn("Descrição");
            dt.Columns.Add(dcDescricao);
            drReal[0] = "Real";
            drPrevisto[0] = "Previsto";
            drTaxaAcerto[0] = "Taxa Acerto";

            for (int dia = 1; dia <= previsao.Count; dia++)
            {
                DataColumn dc = new DataColumn("D+" + dia);
                dt.Columns.Add(dc);
                double ta = Math.Min(previsao[dia - 1][0], previsao[dia - 1][1]) / Math.Max(previsao[dia - 1][0], previsao[dia - 1][1]);
                drReal[dc] = previsao[dia - 1][0];
                drPrevisto[dc] = previsao[dia - 1][1];
                drTaxaAcerto[dc] = ta;
            }

            //gvTaxaAcerto.AutoGenerateColumns = true;
            BindingSource binding = new BindingSource();
            binding.DataSource = dt;// previsao.Select(prev => new { Real = prev[0], Previsto = prev[1] });
            gvTaxaAcerto.DataSource = binding;
            gvTaxaAcerto.Rows[0].Tag = "Teste";
        }

        void DesenharGrafico()
        {
            List<double> real = previsao.Select(prev => prev[0]).ToList();
            List<double> previsto = previsao.Select(prev => prev[1]).ToList();

            grafico.Image = new Bitmap(grafico.Width, grafico.Height);
            Graphics g = Graphics.FromImage(grafico.Image);
            Pen pen1 = new Pen(Color.Red);
            pen1.Width = 2;
            Pen pen2 = new Pen(Color.Yellow);
            pen2.Width = 2;

            //A cada 40 pixels, há alteração de 1 real 
            int multiplicadorY = 20;
            //A cada 10 pixels, há alteração de 1 dia da cotação
            int multiplicadorX = Convert.ToInt32(txtEscalaX.Text);

            int minimoEntreOsDois = Convert.ToInt32(Math.Min(real.Min(), previsto.Min()));

            //Espacamento em reais antes do inicio do dado de menor valor
            int espacamentoAbaixo = 8;
            DesenharLinharAuxiliaresDoGrafico(g, minimoEntreOsDois - espacamentoAbaixo, Convert.ToInt32(Math.Max(real.Max(), previsto.Max())), 1, multiplicadorY, multiplicadorX);

            minimoEntreOsDois *= multiplicadorY;

            for (int i = 0; i < previsto.Count() - 1; i++)
            {
                Point pontoReal1 = new Point(40 + (i * multiplicadorX), grafico.Height - Convert.ToInt32((real[i] + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);
                Point pontoReal2 = new Point(40 + ((i + 1) * multiplicadorX), grafico.Height - Convert.ToInt32((real[i + 1] + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);

                Point pontoPrevisao1 = new Point(40 + (i * multiplicadorX), grafico.Height - Convert.ToInt32((previsto[i] + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);
                Point pontoPrevisao2 = new Point(40 + ((i + 1) * multiplicadorX), grafico.Height - Convert.ToInt32((previsto[i + 1] + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);

                g.DrawLine(pen1, pontoReal1, pontoReal2);
                g.DrawLine(pen2, pontoPrevisao1, pontoPrevisao2);
            }
        }

        void DesenharLinharAuxiliaresDoGrafico(Graphics g, int minimo, int maximo, int unidade, int multiplicadorEscalaY, int multiplicadorEscalaX)
        {
            ////Numero de divisoes do grafico em Y
            //int divisoes = grafico.Height / (multiplicadorEscalaY * janelaVerticalEmReais);
            ////Quantas divisoes serão necessárias para apresentar os dados do valor minimo até o valor máximo(todo o espaço da linha real e prevista em Y)
            //int divisoesNecessariasApresentacaoDados = ((maximo - minimo) / janelaVerticalEmReais) + 1;//Adiciona 1 por causa do arredondamento
            ////Quantos espaçamentos serao dados até chegar no valor minimo dos dados
            //int espacamentoInferior = divisoes - divisoesNecessariasApresentacaoDados / 2;
            //if (espacamentoInferior < 0)
            //    espacamentoInferior = 0;

            ////Espaçaremos a base do grafico para melhorar sua visualização
            //minimo -= espacamentoInferior;

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
            DesenharGrafico();
        }
    }
}
