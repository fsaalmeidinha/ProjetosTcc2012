using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading;
using DataBaseUtils;
using NeuronDotNet.Core;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace RedeNeuralPrevisaoFinanceira_v3
{
    public class RNAssessor
    {
        #region Dados rede principal

        static int nnRNP = 4, numeroDivisoesCrossValidationRNP = 8, shiftRNP = 7;
        static int ctRNP
        {
            get
            {
                return versao == 3 ? 200000 : 50000;
            }
        }
        static string taRNP = "0,25";

        #endregion Dados rede principal

        //private static double versao = 3;
        //private static double versao = 3.2;
        //private static double versao = 3.3;
        private static double versao = 3.4;
        //private static double versao = 3.5;
        //private static double versao = 3.6;
        //private static double versao = 3.7;
        //private static double versao = 3.8;
        //private static double versao = 3.9;
        //private static double versao = 4.01;
        //private static double versao = 4.02;
        //private static double versao = 4.03;
        //private static double versao = 4.04;
        static int numeroDivisoesCrossValidation = 8;
        private static string diretorioRedes
        {
            get
            {
                return ConfigurationManager.AppSettings["DiretorioRedes_v3"] + "\\RedesPrevisaoFinanceira\\";
            }
        }
        private static string diretorioCrossValidation
        {
            get
            {
                return ConfigurationManager.AppSettings["DiretorioRedes_v3"] + "\\RelatorioCrossValidation\\";
            }
        }

        public void TreinarRedes(string papel = "PETR4")
        {
            //Seta a cultura da thread.
            SetarCultura();

            List<DadosBE> dadosBE = DataBaseUtils.DataBaseUtils.RecuperarCotacoesAtivo(papel);
            //dadosBE = dadosBE.Take(dadosBE.Count - 120).ToList();
            int ciclosTreinamento = 50000;

            Thread[] workerThreads = new Thread[numeroDivisoesCrossValidation];//(workerObject.DoWork);
            for (int i = 0; i < numeroDivisoesCrossValidation; i++)
            {
                //Atribui o valor do i para não ser alterado nas iterações do for antes de ser passado para a thread
                int iFixThread = i;
                List<DadosBE> dadosTreino = dadosBE.Take(dadosBE.Count / numeroDivisoesCrossValidation * i).ToList();
                dadosTreino.AddRange(dadosBE.Skip(dadosBE.Count / numeroDivisoesCrossValidation * (i + 1)));
                //TreinarRedeNeural(papel, 4, 0.25, ciclosTreinamento, dadosTreino, iFixThread);
                workerThreads[i] = new Thread(() => TreinarRedeNeural(papel, 4, 0.25, ciclosTreinamento, dadosTreino, iFixThread));
                workerThreads[i].Start();
            }

            for (int i = 0; i < numeroDivisoesCrossValidation; i++)
            {
                workerThreads[i].Join();
            }
        }

        private string TreinarRedeNeural(string papel, int nn, double ta, int ct, List<DadosBE> dadosBE, int shift)
        {
            string nomeRede = String.Format("{0}_nn{1}_ta{2}_ct{3}_dcv{4}_shift{5}_v{6}", papel, nn, ta.ToString().Replace('.', ','), ct, numeroDivisoesCrossValidation, shift, versao.ToString().Replace('.', ','));

            RedeNeural_v3.Treinar(papel, nomeRede, dadosBE, nn, ta, ct, numeroDivisoesCrossValidation, shift, versao);
            return nomeRede;
        }

        /// <summary>
        /// Prever n dados da versao informada
        /// </summary>
        /// <param name="dtPrevisao"></param>
        /// <param name="versaoRN">{3, 3.2, 3.3, 3.4, 3.5}</param>
        /// <param name="papel"></param>
        /// <returns></returns>
        public static List<double[]> PreverCotacao(DateTime dtPrevisao, double versaoRN, int qtdDiasPrevisao, string papel = "PETR4")
        {
            versao = versaoRN;
            List<DadosBE> dadosBE = DataBaseUtils.DataBaseUtils.RecuperarCotacoesAtivo(papel);
            //Preenche os indices da RN_V3
            DataBaseUtils.DataBaseUtils.PreencherIndicesRN_V3(dadosBE, versao);

            //Verifica se a data informada não pertence aos shifts anteriores, pois assim a rede estaria prevendo em cima de dados conhecidos...
            DateTime dtLimite = dadosBE.Skip(dadosBE.Count() / numeroDivisoesCrossValidation * shiftRNP).First().DataGeracao;
            if (dtPrevisao < dtLimite)
            {
                throw new Exception("Data inválida: data deve ser menor do que " + dtLimite.ToShortDateString());
            }

            Network network = RecuperarRedeNeural(papel);

            //Primeiro dado para a previsao
            int dadoBEPrevisoesSkip = dadosBE.IndexOf(dadosBE.Last(dado => dado.DataGeracao < dtPrevisao));
            //dadosBE = dadosBE.Skip(dadoBEPrevisoesSkip).ToList();

            List<double[]> previsoes = new List<double[]>();
            for (int indPrevisao = 0; indPrevisao < qtdDiasPrevisao; indPrevisao++)
            {
                //A versão 3.5, 3.6, 3.7, 3.8 é a unica que permite previsao em cima de previsao, pois não utiliza nenhum dado que não é previsto
                //3, 3.2, 3.3, 3.4,
                //3.5

                //DadoBE com os dados do dia anterior a previsao
                DadosBE dadoBE = dadosBE.Skip(dadoBEPrevisoesSkip + indPrevisao).First();
                //////////////////PREVISAO EM CIMA DE PREVISAO////////////
                //////////////////if ((versaoRN == 3.5 || versaoRN == 3.6 || versaoRN == 3.7 || versaoRN == 3.8 || versaoRN == 3.9 || versaoRN = 4.01|| versaoRN = 4.02) && previsoes.Count > 0)
                //////////////////{
                //////////////////    //Utiliza os dados previstos para a nova previsao
                //////////////////    dadoBE.ValorNormalizado = previsoes.Last()[1];
                //////////////////    dadoBE.PrecoAbertura = Convert.ToDecimal(DataBaseUtils.DataBaseUtils.DesnormalizarDado(dadoBE.ValorNormalizado, papel));
                //////////////////    //Reatribui os indices com os valores das previsoes anteriores
                //////////////////    DataBaseUtils.DataBaseUtils.PreencherIndicesRN_V3(dadosBE, versao);
                //////////////////}

                List<double> input = DataBaseUtils.DataBaseUtils.TransformarDadoBE_Em_Treinamento_RNV3(dadoBE, versao).Input;
                double previsao = network.Run(input.ToArray())[0];
                //double previsaoDesnormalizada = DataBaseUtils.DataBaseUtils.DesnormalizarDado(previsao, papel);

                previsoes.Add(new double[] { Convert.ToDouble(dadosBE[dadoBEPrevisoesSkip + indPrevisao + 1].PrecoAbertura), previsao });
                ////Elimina o elemento ja usado na previsao
                //dadosBE = dadosBE.Skip(1).ToList();
            }

            //Retorna os dados solicitados
            return previsoes.Select(prev => new double[] { prev[0], DataBaseUtils.DataBaseUtils.DesnormalizarDado(prev[1], papel) }).ToList();//new double[] { dadoBE.PrecoFechamentoDiaSeguinte, DataBaseUtils.DataBaseUtils.DesnormalizarDado(previsao, papel.ToUpper()) };
        }

        private static Network RecuperarRedeNeural(string nomeRede = "PETR4")
        {
            using (Stream stream = File.Open(diretorioRedes + RecuperarNomeRNPrincipal(nomeRede) + ".ndn", FileMode.Open))
            {
                IFormatter formatter = new BinaryFormatter();
                return (Network)formatter.Deserialize(stream);
            }
        }

        private static string RecuperarNomeRNPrincipal(string papel = "PETR4")
        {
            return String.Format("{0}_nn{1}_ta{2}_ct{3}_dcv{4}_shift{5}_v{6}", papel, nnRNP, taRNP, ctRNP, numeroDivisoesCrossValidationRNP, shiftRNP, versao.ToString().Replace('.', ','));
        }

        private void SetarCultura()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
        }
    }
}
