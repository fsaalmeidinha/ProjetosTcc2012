using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Graficos
{
    public partial class Form1 : Form
    {
        //De quantos em quantos reais o gráfico será gerado em Y
        int janelaVerticalEmReais = 1;

        //Previsoes por RN
        Dictionary<string, List<double>> previsoesPorRN = new Dictionary<string, List<double>>();
        Dictionary<string, Color> previsoesEscolhidasPorRN = new Dictionary<string, Color>();

        List<double[]> previsao;
        DateTime dtInicialPrevisao = new DateTime(2011, 10, 1);
        int qtdDiasPrever = 30;

        public Form1()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            InitializeComponent();

            //V1
            previsao = PrevisaoFinanceiraHelper.PrevisaoFinanceira.PreverCotacaoAtivo("PETR4", dtInicialPrevisao, qtdDiasPrever);

            //Utiliza a previsao da RN1 para popular os valores reais
            previsoesPorRN.Add("PETR4_Real", previsao.Select(prev => prev[0]).ToList());

            //Preenche as previsoes da RN_V1
            previsoesPorRN.Add("PETR4_RN_V1", previsao.Select(prev => prev[1]).ToList());

            //V2
            previsao = RedeNeuralPrevisaoFinanceira_v2.RNAssessor.PreverCotacao(dtInicialPrevisao, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V2", previsao.Select(prev => prev[1]).ToList());

            //V3
            previsao = RedeNeuralPrevisaoFinanceira_v3.RNAssessor.PreverCotacao(dtInicialPrevisao, 3, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V3", previsao.Select(prev => prev[1]).ToList());

            //V3.2
            previsao = RedeNeuralPrevisaoFinanceira_v3.RNAssessor.PreverCotacao(dtInicialPrevisao, 3.2, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V3_2", previsao.Select(prev => prev[1]).ToList());

            //V3.3
            previsao = RedeNeuralPrevisaoFinanceira_v3.RNAssessor.PreverCotacao(dtInicialPrevisao, 3.3, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V3_3", previsao.Select(prev => prev[1]).ToList());

            //V3.4
            previsao = RedeNeuralPrevisaoFinanceira_v3.RNAssessor.PreverCotacao(dtInicialPrevisao, 3.4, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V3_4", previsao.Select(prev => prev[1]).ToList());

            previsao = RedeNeuralPrevisaoFinanceira_v3.RNAssessor.PreverCotacao(dtInicialPrevisao, 3.4, qtdDiasPrever, "ETER3");
            previsoesPorRN.Add("ETER3_Real", previsao.Select(prev => prev[0]).ToList());
            previsoesPorRN.Add("ETER3_RN_V3_4", previsao.Select(prev => prev[1]).ToList());

            previsao = RedeNeuralPrevisaoFinanceira_v3.RNAssessor.PreverCotacao(dtInicialPrevisao, 3.4, qtdDiasPrever, "GOLL4");
            previsoesPorRN.Add("GOLL4_Real", previsao.Select(prev => prev[0]).ToList());
            previsoesPorRN.Add("GOLL4_RN_V3_4", previsao.Select(prev => prev[1]).ToList());

            previsao = RedeNeuralPrevisaoFinanceira_v3.RNAssessor.PreverCotacao(dtInicialPrevisao, 3.4, qtdDiasPrever, "NATU3");
            previsoesPorRN.Add("NATU3_Real", previsao.Select(prev => prev[0]).ToList());
            previsoesPorRN.Add("NATU3_RN_V3_4", previsao.Select(prev => prev[1]).ToList());

            previsao = RedeNeuralPrevisaoFinanceira_v3.RNAssessor.PreverCotacao(dtInicialPrevisao, 3.4, qtdDiasPrever, "VALE5");
            previsoesPorRN.Add("VALE5_Real", previsao.Select(prev => prev[0]).ToList());
            previsoesPorRN.Add("VALE5_RN_V3_4", previsao.Select(prev => prev[1]).ToList());

            //V3.5
            previsao = RedeNeuralPrevisaoFinanceira_v3.RNAssessor.PreverCotacao(dtInicialPrevisao, 3.5, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V3_5", previsao.Select(prev => prev[1]).ToList());

            //V3.6
            previsao = RedeNeuralPrevisaoFinanceira_v3.RNAssessor.PreverCotacao(dtInicialPrevisao, 3.6, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V3_6", previsao.Select(prev => prev[1]).ToList());

            //V3.7
            previsao = RedeNeuralPrevisaoFinanceira_v3.RNAssessor.PreverCotacao(dtInicialPrevisao, 3.7, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V3_7", previsao.Select(prev => prev[1]).ToList());

            //V3.8
            previsao = RedeNeuralPrevisaoFinanceira_v3.RNAssessor.PreverCotacao(dtInicialPrevisao, 3.8, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V3_8", previsao.Select(prev => prev[1]).ToList());

            //V3.9
            previsao = RedeNeuralPrevisaoFinanceira_v3.RNAssessor.PreverCotacao(dtInicialPrevisao, 3.9, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V3_9", previsao.Select(prev => prev[1]).ToList());

            //V4.01
            previsao = RedeNeuralPrevisaoFinanceira_v3.RNAssessor.PreverCotacao(dtInicialPrevisao, 4.01, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V4_01", previsao.Select(prev => prev[1]).ToList());

            //V4.02
            previsao = RedeNeuralPrevisaoFinanceira_v3.RNAssessor.PreverCotacao(dtInicialPrevisao, 4.02, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V4_02", previsao.Select(prev => prev[1]).ToList());

            //V4.03
            previsao = RedeNeuralPrevisaoFinanceira_v3.RNAssessor.PreverCotacao(dtInicialPrevisao, 4.03, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V4_03", previsao.Select(prev => prev[1]).ToList());

            //V4.04
            previsao = RedeNeuralPrevisaoFinanceira_v3.RNAssessor.PreverCotacao(dtInicialPrevisao, 4.04, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V4_04", previsao.Select(prev => prev[1]).ToList());

            #region V5
            //for (double v = 5.01; v <= 5.10; v = v + 0.01)
            //{
            //    previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, v, qtdDiasPrever);
            //    string nomeRN = "RN_V5_" + v.ToString().Split(new string[] { ".", "," }, StringSplitOptions.RemoveEmptyEntries)[1];
            //    previsoesPorRN.Add(nomeRN, previsao.Select(prev => prev[1]).ToList());
            //}
            //V5.01
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.01, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V5_01", previsao.Select(prev => prev[1]).ToList());

            //V5.02
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.02, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V5_02", previsao.Select(prev => prev[1]).ToList());

            //V5.03
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.03, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V5_03", previsao.Select(prev => prev[1]).ToList());

            //V5.04
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.04, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V5_04", previsao.Select(prev => prev[1]).ToList());

            //V5.05
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.05, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V5_05", previsao.Select(prev => prev[1]).ToList());

            //V5.06
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.06, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V5_06", previsao.Select(prev => prev[1]).ToList());

            //V5.07
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.07, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V5_07", previsao.Select(prev => prev[1]).ToList());

            //V5.08
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.08, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V5_08", previsao.Select(prev => prev[1]).ToList());

            //V5.09
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.09, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V5_09", previsao.Select(prev => prev[1]).ToList());

            //V5.10
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.10, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V5_10", previsao.Select(prev => prev[1]).ToList());

            //V5.11
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.11, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V5_11", previsao.Select(prev => prev[1]).ToList());

            //V5.12
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.12, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V5_12", previsao.Select(prev => prev[1]).ToList());

            //V5.13
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.13, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V5_13", previsao.Select(prev => prev[1]).ToList());

            //V5.14
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.14, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V5_14", previsao.Select(prev => prev[1]).ToList());

            //V5.15
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.15, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V5_15", previsao.Select(prev => prev[1]).ToList());

            //V5.16
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.16, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V5_16", previsao.Select(prev => prev[1]).ToList());

            //V5.17
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.17, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V5_17", previsao.Select(prev => prev[1]/*-3.45*/).ToList());
            #endregion V5

            //Media ponderada
            previsao = MediaPonderada.MetodoMediaPonderada.PreverMediasPonderada("PETR4", dtInicialPrevisao, qtdDiasPrever, false);
            previsoesPorRN.Add("PETR4_Med_Ponderada", previsao.Select(prev => prev[1]).ToList());

            //Media aritmetica
            previsao = MediaAritmetica.MetodoMediaAritmetica.PreverMediasAritmetica("PETR4", dtInicialPrevisao, qtdDiasPrever, false);
            previsoesPorRN.Add("PETR4_Med_Aritmetica", previsao.Select(prev => prev[1]).ToList());

            //AlisamentoExponencialSimples
            previsao = AlisamentoExponencialSimples.MetodoAlisamentoExpSimples.PreverAlisamentoExponencialSimples("PETR4", dtInicialPrevisao, qtdDiasPrever, false);
            previsoesPorRN.Add("PETR4_Med_Exp_Simpl", previsao.Select(prev => prev[1]).ToList());

            previsoesEscolhidasPorRN.Add(cbAtivo.SelectedItem.ToString() + "_" + pbReal.Name.Substring(2), pbReal.BackColor);
            DesenharGrafico();
            AlimentarGridTaxasAcerto();
        }

        private void AlimentarGridTaxasAcerto()
        {
            DataTable dt = new DataTable();
            DataRow drReal = dt.NewRow();
            dt.Rows.Add(drReal);
            DataColumn dcDescricao = new DataColumn("Descrição");
            dt.Columns.Add(dcDescricao);
            drReal[0] = "Real";
            string nomeAtivoEscolhido = cbAtivo.SelectedItem.ToString();

            List<double> valoresReais = previsoesPorRN[nomeAtivoEscolhido + "_Real"];
            //Preenche os valores reais
            for (int dia = 1; dia <= qtdDiasPrever; dia++)
            {
                DataColumn dc = new DataColumn("D+" + dia);
                dt.Columns.Add(dc);
                drReal[dc] = valoresReais[dia - 1];
            }

            foreach (KeyValuePair<string, Color> previsaoSelecionada in previsoesEscolhidasPorRN.Where(prev => prev.Key != nomeAtivoEscolhido + "_Real"))
            {
                DataRow drPrevisto = dt.NewRow();
                dt.Rows.Add(drPrevisto);
                drPrevisto[0] = previsaoSelecionada.Key;
                List<double> previsoes = previsoesPorRN[previsaoSelecionada.Key];

                for (int dia = 1; dia <= qtdDiasPrever; dia++)
                {
                    double ta = Math.Min(valoresReais[dia - 1], previsoes[dia - 1]) / Math.Max(valoresReais[dia - 1], previsoes[dia - 1]);
                    drPrevisto[dia] = ta.ToString().Substring(0, 5) + "(" + previsoes[dia - 1].ToString().Substring(0, 4) + ")";
                }
            }

            //gvTaxaAcerto.AutoGenerateColumns = true;
            BindingSource binding = new BindingSource();
            binding.DataSource = dt;// previsao.Select(prev => new { Real = prev[0], Previsto = prev[1] });
            gvTaxaAcerto.DataSource = binding;
            //gvTaxaAcerto.Rows[0].Tag = "Teste";
        }

        void DesenharGrafico()
        {
            //List<double> real = previsao.Select(prev => prev[0]).ToList();
            //List<double> previsto = previsao.Select(prev => prev[1]).ToList();
            Dictionary<Color, List<double>> previsoesEscolhidas = RecuperarPrevisoesEscolhidas();
            if (previsoesEscolhidas.Count == 0)
                return;

            grafico.Image = new Bitmap(grafico.Width, grafico.Height);
            Graphics g = Graphics.FromImage(grafico.Image);

            int minimoEntreOsDois = Convert.ToInt32(previsoesEscolhidas.First().Value.Min());
            int maximoEntreOsDois = Convert.ToInt32(previsoesEscolhidas.First().Value.Max());

            //A cada 40 pixels, há alteração de 1 real 
            int multiplicadorY = grafico.Height / (maximoEntreOsDois - minimoEntreOsDois) / 3;//15;
            if (multiplicadorY < 15) multiplicadorY = 15;
            //A cada 10 pixels, há alteração de 1 dia da cotação
            int multiplicadorX = Convert.ToInt32(txtEscalaX.Text);

            foreach (List<double> previsao in previsoesEscolhidas.Select(prevEsc => prevEsc.Value).ToList())
            {
                minimoEntreOsDois = Convert.ToInt32(Math.Min(minimoEntreOsDois, previsao.Min()));
                maximoEntreOsDois = Convert.ToInt32(Math.Min(maximoEntreOsDois, previsao.Max()));
            }

            //Espacamento em reais antes do inicio do dado de menor valor
            int espacamentoAbaixo = 2;//8;
            DesenharLinharAuxiliaresDoGrafico(g, minimoEntreOsDois - espacamentoAbaixo, maximoEntreOsDois, 1, multiplicadorY, multiplicadorX);

            minimoEntreOsDois *= multiplicadorY;

            foreach (KeyValuePair<Color, List<double>> previsao in previsoesEscolhidas)
            {
                Pen pen1 = new Pen(previsao.Key);
                pen1.Width = 2;
                for (int i = 0; i < qtdDiasPrever - 1; i++)
                {
                    Point ponto1 = new Point(40 + (i * multiplicadorX), grafico.Height - Convert.ToInt32((previsao.Value[i] + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);
                    Point ponto2 = new Point(40 + ((i + 1) * multiplicadorX), grafico.Height - Convert.ToInt32((previsao.Value[i + 1] + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);

                    g.DrawLine(pen1, ponto1, ponto2);
                }
            }
            //for (int i = 0; i < qtdDiasPrever - 1; i++)
            //{
            //    Point pontoReal1 = new Point(40 + (i * multiplicadorX), grafico.Height - Convert.ToInt32((real[i] + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);
            //    Point pontoReal2 = new Point(40 + ((i + 1) * multiplicadorX), grafico.Height - Convert.ToInt32((real[i + 1] + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);

            //    Point pontoPrevisao1 = new Point(40 + (i * multiplicadorX), grafico.Height - Convert.ToInt32((previsto[i] + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);
            //    Point pontoPrevisao2 = new Point(40 + ((i + 1) * multiplicadorX), grafico.Height - Convert.ToInt32((previsto[i + 1] + espacamentoAbaixo) * multiplicadorY) + minimoEntreOsDois);

            //    g.DrawLine(pen1, pontoReal1, pontoReal2);
            //    g.DrawLine(pen2, pontoPrevisao1, pontoPrevisao2);
            //}
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
            AlimentarGridTaxasAcerto();
        }

        private void pbRns_Click(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            string nomeAtivo = cbAtivo.SelectedItem.ToString();
            string rnName = pb.Name.Substring(2);
            SetarPrevisaoEscolhida(nomeAtivo + "_" + rnName, pb.BackColor);

            DesenharGrafico();
            AlimentarGridTaxasAcerto();
        }

        private void SetarPrevisaoEscolhida(string rnName, Color color)
        {
            if (previsoesEscolhidasPorRN.Keys.Contains(rnName))
                previsoesEscolhidasPorRN.Remove(rnName);
            else
                previsoesEscolhidasPorRN.Add(rnName, color);
        }

        private Dictionary<Color, List<double>> RecuperarPrevisoesEscolhidas()
        {
            Dictionary<Color, List<double>> previsoesEscolhidas = new Dictionary<Color, List<double>>();
            foreach (KeyValuePair<string, Color> rnEscolhida in previsoesEscolhidasPorRN)
            {
                previsoesEscolhidas.Add(rnEscolhida.Value, previsoesPorRN[rnEscolhida.Key]);
            }

            return previsoesEscolhidas;
        }

        private void cbAtivo_SelectedIndexChanged(object sender, EventArgs e)
        {
            grafico.Image = new Bitmap(grafico.Width, grafico.Height);
            gvTaxaAcerto.DataSource = new BindingSource();
            previsoesEscolhidasPorRN.Clear();
            previsoesEscolhidasPorRN.Add(cbAtivo.SelectedItem.ToString() + "_" + pbReal.Name.Substring(2), pbReal.BackColor);

            DesenharGrafico();
            AlimentarGridTaxasAcerto();
        }
    }
}
