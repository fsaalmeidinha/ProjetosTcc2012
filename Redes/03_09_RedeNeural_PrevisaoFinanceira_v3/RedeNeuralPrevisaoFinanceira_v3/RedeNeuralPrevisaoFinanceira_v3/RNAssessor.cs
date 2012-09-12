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

        static int nnRNP = 4, ctRNP = 50000, numeroDivisoesCrossValidationRNP = 8, shiftRNP = 7;
        static string taRNP = "0,25";

        #endregion Dados rede principal

        private static int versao = 3;
        static int numeroDivisoesCrossValidation = 8;
        private static string diretorioRedes
        {
            get
            {
                return ConfigurationManager.AppSettings["DiretorioRedes"] + "\\RedesPrevisaoFinanceira\\";
            }
        }
        private static string diretorioCrossValidation
        {
            get
            {
                return ConfigurationManager.AppSettings["DiretorioRedes"] + "\\RelatorioCrossValidation\\";
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
            string nomeRede = String.Format("{0}_nn{1}_ta{2}_ct{3}_dcv{4}_shift{5}_v{6}", papel, nn, ta.ToString().Replace('.', ','), ct, numeroDivisoesCrossValidation, shift, versao);

            RedeNeural_v3.Treinar(papel, nomeRede, dadosBE, nn, ta, ct, numeroDivisoesCrossValidation, shift);
            return nomeRede;
        }

        public static double[] PreverCotacao(DateTime dtPrevisao, string papel = "PETR4")
        {
            List<DadosBE> dadosBE = DataBaseUtils.DataBaseUtils.RecuperarCotacoesAtivo(papel);
            //Preenche os indices da RN_V3
            DataBaseUtils.DataBaseUtils.PreencherIndicesRN_V3(dadosBE);

            //Verifica se a data informada não pertence aos shifts anteriores, pois assim a rede estaria prevendo em cima de dados conhecidos...
            DateTime dtLimite = dadosBE.Skip(dadosBE.Count() / numeroDivisoesCrossValidation * numeroDivisoesCrossValidationRNP).First().DataGeracao;
            if (dtPrevisao < dtLimite)
            {
                throw new Exception("Data inválida: data deve ser menor do que " + dtLimite.ToShortDateString());
            }

            Network network = RecuperarRedeNeural(papel);

            //DadoBE com os dados do dia anterior a previsao
            DadosBE dadoBE = dadosBE.Last(dado => dado.DataGeracao < dtPrevisao);

            List<double> input = DataBaseUtils.DataBaseUtils.TransformarDadoBE_Em_Treinamento_RNV3(dadoBE).Input;
            double previsao = network.Run(input.ToArray())[0];

            //Retorna os dados solicitados
            return new double[] { dadoBE.PrecoFechamentoDiaSeguinte, DataBaseUtils.DataBaseUtils.DesnormalizarDado(previsao, papel.ToUpper()) };
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
            return String.Format("{0}_nn{1}_ta{2}_ct{3}_dcv{4}_shift{5}_v{6}", papel, nnRNP, taRNP, ctRNP, numeroDivisoesCrossValidationRNP, shiftRNP, versao);
        }

        private void SetarCultura()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
        }
    }
}
