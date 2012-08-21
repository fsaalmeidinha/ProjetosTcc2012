using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using DataBaseUtils;
using System.Threading;

namespace RedeNeuralPrevisaoFinanceira_v2
{
    public class RNAssessor
    {
        private int versao = 2;
        static int numeroDivisoesCrossValidation = 8;
        private static string diretorioRedes
        {
            get
            {
                return ConfigurationManager.AppSettings["DiretorioRedes"] + "\\RedesPrevisaoFinanceira\\";
            }
        }

        private string TreinarRedeNeural(string papel, int je, int nn, double ta, int ct, List<DadosBE> dadosBE, int shift)
        {
            string nomeRede = String.Format("{0}_je{1}_nn{2}_ta{3}_ct{4}_dcv{5}_shift{6}_v{7}", papel, je, nn, ta.ToString().Replace('.', ','), ct, numeroDivisoesCrossValidation, shift, versao);

            RedeNeural_v2.Treinar(papel, nomeRede, dadosBE, je, nn, ta, ct, numeroDivisoesCrossValidation, shift);
            return nomeRede;
        }

        public List<string> TreinarRedes()
        {
            //Seta a cultura da thread.
            SetarCultura();

            string papel = "PETR4";
            List<DadosBE> dadosBE = DataBaseUtils.DataBaseUtils.RecuperarCotacoesAtivo(papel);
            //dadosBE = dadosBE.Take(dadosBE.Count - 120).ToList();
            List<int> listNumeroNeuronios = new List<int>() { 2, 4, 8, 12 };
            List<double> listTaxasAprendizado = new List<double>() { 0.1, 0.25, 0.5 };
            int ciclosTreinamento = 50000;
            List<KeyValuePair<int, int>> listInput_Output = new List<KeyValuePair<int, int>>();
            listInput_Output.Add(new KeyValuePair<int, int>(5, 1));
            listInput_Output.Add(new KeyValuePair<int, int>(10, 1));
            listInput_Output.Add(new KeyValuePair<int, int>(20, 1));
            listInput_Output.Add(new KeyValuePair<int, int>(30, 1));
            listInput_Output.Add(new KeyValuePair<int, int>(40, 1));
            listInput_Output.Add(new KeyValuePair<int, int>(50, 1));
            listInput_Output.Add(new KeyValuePair<int, int>(60, 1));
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

        private void SetarCultura()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
        }
    }
}
