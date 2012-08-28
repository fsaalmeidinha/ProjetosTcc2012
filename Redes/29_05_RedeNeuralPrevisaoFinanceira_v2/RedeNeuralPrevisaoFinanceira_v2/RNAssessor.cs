using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using DataBaseUtils;
using System.Threading;
using NeuronDotNet.Core;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DataBaseUtils.Model;

namespace RedeNeuralPrevisaoFinanceira_v2
{
    public class RNAssessor
    {
        #region Dados rede principal

        static int jeRNP = 5, nnRNP = 4, ctRNP = 50000, numeroDivisoesCrossValidationRNP = 8, shiftRNP = 7;
        static string taRNP = "0,25";

        #endregion Dados rede principal

        private static int versao = 2;
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

        public List<string> TreinarRedes(string papel = "PETR4")
        {
            //Seta a cultura da thread.
            SetarCultura();

            List<DadosBE> dadosBE = DataBaseUtils.DataBaseUtils.RecuperarCotacoesAtivo(papel);
            //dadosBE = dadosBE.Take(dadosBE.Count - 120).ToList();
            int ciclosTreinamento = 50000;
            List<KeyValuePair<int, int>> listInput_Output = new List<KeyValuePair<int, int>>();
            listInput_Output.Add(new KeyValuePair<int, int>(5, 1));
            //listInput_Output.Add(new KeyValuePair<int, int>(10, 1));
            //listInput_Output.Add(new KeyValuePair<int, int>(20, 1));
            //listInput_Output.Add(new KeyValuePair<int, int>(30, 1));
            //listInput_Output.Add(new KeyValuePair<int, int>(40, 1));
            //listInput_Output.Add(new KeyValuePair<int, int>(50, 1));
            //listInput_Output.Add(new KeyValuePair<int, int>(60, 1));
            List<string> nomeRedes = new List<string>();
            foreach (KeyValuePair<int, int> input_output in listInput_Output)
            {
                //Apagar
                //DateTime dtNow = DateTime.Now;

                //for (int i = 0; i < numeroDivisoesCrossValidation; i++)
                //{
                //    List<double> dadosTreino = dados.Take(dados.Count / numeroDivisoesCrossValidation * i).ToList();
                //    dadosTreino.AddRange(dados.Skip(dados.Count / numeroDivisoesCrossValidation * (i + 1)));
                //    TreinarRedeNeural(papel, input_output.Key, input_output.Value, 4, 0.25, ciclosTreinamento, dadosTreino, i);
                //    Console.WriteLine(i + " -  " + DateTime.Now.Subtract(dtNow).ToString());
                //}
                Thread[] workerThreads = new Thread[numeroDivisoesCrossValidation];//(workerObject.DoWork);
                for (int i = 0; i < numeroDivisoesCrossValidation; i++)
                {
                    //Atribui o valor do i para não ser alterado nas iterações do for antes de ser passado para a thread
                    int iFixThread = i;
                    List<DadosBE> dadosTreino = dadosBE.Take(dadosBE.Count / numeroDivisoesCrossValidation * i).ToList();
                    dadosTreino.AddRange(dadosBE.Skip(dadosBE.Count / numeroDivisoesCrossValidation * (i + 1)));
                    TreinarRedeNeural(papel, input_output.Key, 4, 0.25, ciclosTreinamento, dadosTreino, iFixThread);
                    workerThreads[i] = new Thread(() => TreinarRedeNeural(papel, input_output.Key, 4, 0.25, ciclosTreinamento, dadosTreino, iFixThread));//TreinarRedeNeural(papel, input_output.Key, input_output.Value, 4, 0.25, ciclosTreinamento, dadosTreino, i));
                    workerThreads[i].Start();
                }
                for (int i = 0; i < numeroDivisoesCrossValidation; i++)
                {
                    workerThreads[i].Join();
                }

                //Apagar
                //Console.WriteLine(String.Format("Input: {0}, Output: {1}, Tempo: {2}", input_output.Key, input_output.Value, DateTime.Now.Subtract(dtNow).ToString()));
            }

            return nomeRedes;
        }

        public static List<double[]> PreverCotacao(DateTime dtInicial, int qtdDiasPrevisao, string papel = "PETR4")
        {
            List<DadosBE> dadosBE = DataBaseUtils.DataBaseUtils.RecuperarCotacoesAtivo(papel);
            //Verifica se a data informada não pertence aos shifts anteriores, pois assim a rede estaria prevendo em cima de dados conhecidos...
            DateTime dtLimite = dadosBE.Skip(dadosBE.Count() / 8 * 7).First().DataGeracao;
            if (dtInicial < dtLimite)
            {
                throw new Exception("Data inválida: data deve ser menor do que " + dtLimite.ToShortDateString());
            }
            DateTime dtFinal = dtInicial.Date.AddDays(qtdDiasPrevisao);

            Network network = RecuperarRedeNeural(papel);

            DateTime dtPrevisao = dtInicial.Date;
            for (int diasPrevistos = 0; diasPrevistos < qtdDiasPrevisao; diasPrevistos++)
            {
                DadosBE dadoBEPrever = dadosBE.First(dado => dado.DataGeracao >= dtPrevisao);//Dado que iremos prever...
                List<DadosBE> dadosBERun = DataBaseUtils.DataBaseUtils.SelecionarUltimosNDadosAntesDaDataDaPrevisao(dadosBE, dtPrevisao, jeRNP);

                List<double> input = DataBaseUtils.DataBaseUtils.SelecionarInput_V2(dadosBERun, true);
                double[] output = network.Run(input.ToArray());

                //Atualiza os valores
                dadoBEPrever.ValorNormalizadoPrevisto = output[0];
                dadoBEPrever.CotacaoDolarNormalizadoPrevisto = (decimal)output[1];

                dtPrevisao = dadoBEPrever.DataGeracao.AddDays(1);
            }

            //Retorna os dados solicitados
            return dadosBE.Where(dado => dado.DataGeracao >= dtInicial).Take(qtdDiasPrevisao).Select(dado => new double[] { (double)dado.PrecoAbertura, DataBaseUtils.DataBaseUtils.DesnormalizarDado(dado.ValorNormalizadoPrevisto, papel) }).ToList();
        }

        public void GerarRelatorioCrossValidation()
        {

        }

        private static Network RecuperarRedeNeural(string nomeRede = "PETR4")
        {
            using (Stream stream = File.Open(diretorioRedes + RecuperarNomeRNPrincipal(nomeRede) + ".ndn", FileMode.Open))
            {
                IFormatter formatter = new BinaryFormatter();
                return (Network)formatter.Deserialize(stream);
            }
        }

        private string TreinarRedeNeural(string papel, int je, int nn, double ta, int ct, List<DadosBE> dadosBE, int shift)
        {
            string nomeRede = String.Format("{0}_je{1}_nn{2}_ta{3}_ct{4}_dcv{5}_shift{6}_v{7}", papel, je, nn, ta.ToString().Replace('.', ','), ct, numeroDivisoesCrossValidation, shift, versao);

            RedeNeural_v2.Treinar(papel, nomeRede, dadosBE, je, nn, ta, ct, numeroDivisoesCrossValidation, shift);
            return nomeRede;
        }

        private void SetarCultura()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
        }

        private static string RecuperarNomeRNPrincipal(string papel = "PETR4")
        {
            return String.Format("{0}_je{1}_nn{2}_ta{3}_ct{4}_dcv{5}_shift{6}_v{7}", papel, jeRNP, nnRNP, taRNP, ctRNP, numeroDivisoesCrossValidationRNP, shiftRNP, versao);
        }
    }
}
