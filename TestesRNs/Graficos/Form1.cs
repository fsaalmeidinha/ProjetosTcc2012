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
        public int espacoAbaixo = 0;
        public int espacoAcima = 0;
        public int shiftItens = 0;
        Dictionary<string, ResumoPrevisao> resumosPrevisao = new Dictionary<string, ResumoPrevisao>();
        //int qtdDiasPrever = 100;
        //DateTime dataInicialPrevisao = new DateTime(2011, 09, 1);
        //Versao versao = Versao.V602;

        public Form1()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("pt-BR");

            InitializeComponent();
            trackBar1.Minimum = 2;
            trackBar1.Maximum = 100;
            trackBar1.Value = 30;
            //PreencherComboBoxVersao();
            DesenharGrafico();
        }

        void DesenharGrafico()
        {
            ResumoPrevisao resumoPrevisao = null;
            if (resumosPrevisao.Keys.Contains(papel))
                resumoPrevisao = resumosPrevisao[papel];
            else
            {
                resumoPrevisao = RNHelper.PreverAtivo(papel);//;
                resumosPrevisao.Add(papel, resumoPrevisao);
            }

            List<TestesRNs.Modelo.ResumoPrevisao.Previsao> previsoes = new List<ResumoPrevisao.Previsao>(resumoPrevisao.Previsoes);
            shiftItens = shiftItens > previsoes.Count - 2 ? previsoes.Count - 2 : shiftItens;
            previsoes = previsoes.Skip(shiftItens).ToList();

            #region Preenchimento do resumo na tela

            trackBar1.Maximum = resumoPrevisao.Previsoes.Count;
            lblDataInicialResumo.Text = resumoPrevisao.DataInicial.ToShortDateString();
            lblDataFinalResumo.Text = resumoPrevisao.DataFinal.ToShortDateString();
            //lblTamanhoTendencia.Text = resumoPrevisao.TamanhoTendencia.ToString();
            lblVersoes.Text = "Versões: " +
                String.Join(", ", resumoPrevisao.Versoes.Select(ver => ((DescriptionAttribute)typeof(Versao).GetMember(ver.ToString())[0].GetCustomAttributes(typeof(DescriptionAttribute), false).First()).Description))
                + " (Tem.Tend: " + resumoPrevisao.TamanhoTendencia + " dias )";
            lblTotalDados.Text = resumoPrevisao.Previsoes.Count.ToString();

            int acertosAlta = resumoPrevisao.Previsoes.Count(prev => prev.ResultadoPrevisao == ResultadoPrevisao.SubirOK);
            int totalTendenciasAlta = resumoPrevisao.Previsoes.Count(prev => prev.ResultadoPrevisao == ResultadoPrevisao.SubirOK || prev.ResultadoPrevisao == ResultadoPrevisao.SubirNOK);
            double percentualAcertoAlta = (double)acertosAlta / (totalTendenciasAlta == 0 ? 1 : totalTendenciasAlta) * 100;
            lblAcertosAlta.Text = String.Format("{0}/{1} ({2}%)", acertosAlta, totalTendenciasAlta, percentualAcertoAlta.ToString("N2"));

            int acertosBaixa = resumoPrevisao.Previsoes.Count(prev => prev.ResultadoPrevisao == ResultadoPrevisao.DescerOK);
            int totalTendenciasBaixa = resumoPrevisao.Previsoes.Count(prev => prev.ResultadoPrevisao == ResultadoPrevisao.DescerOK || prev.ResultadoPrevisao == ResultadoPrevisao.DescerNOK);
            double percentualAcertoBaixa = (double)acertosBaixa / (totalTendenciasBaixa == 0 ? 1 : totalTendenciasBaixa) * 100;
            lblAcertosbaixa.Text = String.Format("{0}/{1} ({2}%)", acertosBaixa, totalTendenciasBaixa, percentualAcertoBaixa.ToString("N2"));

            /*RESUMO PARCIAL*/
            List<TestesRNs.Modelo.ResumoPrevisao.Previsao> previsoesParciais = new List<ResumoPrevisao.Previsao>(previsoes);
            previsoesParciais = previsoesParciais.Take(Convert.ToInt32(lblQtdDias.Text) > previsoesParciais.Count ? previsoesParciais.Count : Convert.ToInt32(lblQtdDias.Text)).ToList();

            lblDataInicialParcial.Text = previsoesParciais.First().Data.ToShortDateString();
            lblDataFinalParcial.Text = previsoesParciais.Last().Data.ToShortDateString();
            int acertosAltaParcial = previsoesParciais.Count(prev => prev.ResultadoPrevisao == ResultadoPrevisao.SubirOK);
            int totalTendenciasAltaParcial = previsoesParciais.Count(prev => prev.ResultadoPrevisao == ResultadoPrevisao.SubirOK || prev.ResultadoPrevisao == ResultadoPrevisao.SubirNOK);
            double percentualAcertoAltaParcial = (double)acertosAltaParcial / (totalTendenciasAltaParcial == 0 ? 1 : totalTendenciasAltaParcial) * 100;
            lblAcertosAltaParcial.Text = String.Format("{0}/{1} ({2}%)", acertosAltaParcial, totalTendenciasAltaParcial, percentualAcertoAltaParcial.ToString("N2"));


            int acertosBaixaParcial = previsoesParciais.Count(prev => prev.ResultadoPrevisao == ResultadoPrevisao.DescerOK);
            int totalTendenciasBaixaParcial = previsoesParciais.Count(prev => prev.ResultadoPrevisao == ResultadoPrevisao.DescerOK || prev.ResultadoPrevisao == ResultadoPrevisao.DescerNOK);
            double percentualAcertoBaixaParcial = (double)acertosBaixaParcial / (totalTendenciasBaixaParcial == 0 ? 1 : totalTendenciasBaixaParcial) * 100;
            lblAcertosBaixaParcial.Text = String.Format("{0}/{1} ({2}%)", acertosBaixaParcial, totalTendenciasBaixaParcial, percentualAcertoBaixaParcial.ToString("N2"));

            #endregion Preenchimento do resumo na tela

            int qtdDiasPrever = previsoes.Count;

            grafico.Image = new Bitmap(grafico.Width, grafico.Height);
            Graphics g = Graphics.FromImage(grafico.Image);

            int minimoEntreOsDois = Convert.ToInt32(previsoes.Min(prev => prev.ValorAtivo)) - espacoAbaixo;
            int maximoEntreOsDois = Convert.ToInt32(previsoes.Max(prev => prev.ValorAtivo)) + espacoAcima;
            if (minimoEntreOsDois < 0)
            {
                minimoEntreOsDois = 0;
                espacoAbaixo = minimoEntreOsDois;
            }
            if (espacoAcima > 2 * (maximoEntreOsDois - espacoAcima))
            {
                maximoEntreOsDois = 2 * (maximoEntreOsDois - espacoAcima);
                espacoAcima = maximoEntreOsDois;
            }

            //A cada 40 pixels, há alteração de 1 real 
            int multiplicadorY = grafico.Height / (maximoEntreOsDois - minimoEntreOsDois);//15;
            //if (multiplicadorY < 10) multiplicadorY = 10;
            //if (multiplicadorY > 50) multiplicadorY = multiplicadorY / 2;
            if (multiplicadorY < 1) multiplicadorY = 1;
            //A cada 10 pixels, há alteração de 1 dia da cotação
            ////////////////int multiplicadorX = Convert.ToInt32(txtEscalaX.Text);
            int multiplicadorX = Convert.ToInt32(grafico.Width / Convert.ToDouble(lblQtdDias.Text));

            //Espacamento em reais antes do inicio do dado de menor valor
            int espacamentoAbaixo = 1;//8;
            DesenharLinharAuxiliaresDoGrafico(g, minimoEntreOsDois - espacamentoAbaixo, maximoEntreOsDois, 1, multiplicadorY, multiplicadorX);

            minimoEntreOsDois *= multiplicadorY;

            Pen pen1 = new Pen(Color.Red) { Width = 3f };
            for (int i = 0; i < qtdDiasPrever - 1; i++)
            {
                Point ponto1Real = new Point(40 + (i * multiplicadorX), grafico.Height - Convert.ToInt32((previsoes[i].ValorAtivo + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);
                Point ponto2Real = new Point(40 + ((i + 1) * multiplicadorX), grafico.Height - Convert.ToInt32((previsoes[i + 1].ValorAtivo + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);

                //Point ponto1Previsto = new Point(40 + (i * multiplicadorX), grafico.Height - Convert.ToInt32((previsao[i][1] + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);
                //Point ponto2Previsto = new Point(40 + ((i + 1) * multiplicadorX), grafico.Height - Convert.ToInt32((previsao[i + 1][1] + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);

                g.DrawLine(pen1, ponto1Real, ponto2Real);


                if (previsoes[i].ResultadoPrevisao != 0 && i + resumoPrevisao.TamanhoTendencia < previsoes.Count)
                {
                    Point ponto1Tendencia = new Point(40 + (i * multiplicadorX), grafico.Height - Convert.ToInt32((previsoes[i].ValorAtivo + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);
                    Point ponto2Tendencia = new Point(40 + ((i + resumoPrevisao.TamanhoTendencia) * multiplicadorX), grafico.Height - Convert.ToInt32((previsoes[i + resumoPrevisao.TamanhoTendencia].ValorAtivo + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);

                    Pen pen2 = null;
                    if (previsoes[i].ResultadoPrevisao == ResultadoPrevisao.SubirOK)//OK - A Rede preveu que iria subir o valor do ativo e subiu..
                        pen2 = new Pen(Color.Green) { Width = 2f };
                    if (previsoes[i].ResultadoPrevisao == ResultadoPrevisao.SubirNOK)//NOK - A Rede preveu que iria subir o valor do ativo e caiu..
                        pen2 = new Pen(Color.Yellow) { Width = 2f };
                    if (previsoes[i].ResultadoPrevisao == ResultadoPrevisao.DescerOK)//OK - A Rede preveu que iria descer o valor do ativo e desceu..
                        pen2 = new Pen(Color.Orange) { Width = 2f };
                    if (previsoes[i].ResultadoPrevisao == ResultadoPrevisao.DescerNOK)//NOK - A Rede preveu que iria descer o valor do ativo e subiu..
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
            espacoAbaixo = 0;
            espacoAcima = 0;
            shiftItens = 0;
            trackBar1.Value = 30;
            DesenharGrafico();
        }

        #region Botoes Manipulação Grafico

        private void btnMaisEspacoAcima_Click(object sender, EventArgs e)
        {
            if (espacoAcima <= 30)
                espacoAcima++;
            DesenharGrafico();
        }

        private void btnMenosEspacoAcima_Click(object sender, EventArgs e)
        {
            espacoAcima--;
            DesenharGrafico();
        }

        private void btnMenosEspacoAbaixo_Click(object sender, EventArgs e)
        {
            espacoAbaixo--;
            DesenharGrafico();
        }

        private void btnMaisEspacoAbaixo_Click(object sender, EventArgs e)
        {
            if (espacoAcima <= 30)
                espacoAbaixo++;
            DesenharGrafico();
        }

        private void btnMaisEsquerda_Click(object sender, EventArgs e)
        {
            shiftItens--;
            DesenharGrafico();
        }

        private void btnMuitoMaisEsquerda_Click(object sender, EventArgs e)
        {
            shiftItens -= 10;
            if (shiftItens < 0)
                shiftItens = 0;
            DesenharGrafico();
        }

        private void btnMaisDireita_Click(object sender, EventArgs e)
        {
            shiftItens++;
            DesenharGrafico();
        }

        private void btnMuitoMaisDireita_Click(object sender, EventArgs e)
        {
            shiftItens += 10;
            DesenharGrafico();
        }

        #endregion Botoes Manipulação Grafico

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            lblQtdDias.Text = trackBar1.Value.ToString();
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
