using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuronDotNet.Core;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DataBaseUtils;
using System.Configuration;
using System.Threading;

namespace RedeNeural_PrevisaoFinanceira
{
    public class RNAssessor
    {
        //public int JanelaEntrada = 20;
        //public int JanelaSaida = 10;
        //public int NumeroNeuronios = 8;
        //public double TaxaAprendizado = 0.25;
        //public int CiclosTreinamento = 5000;
        private int versao = 1;
        static int numeroDivisoesCrossValidation = 8;
        private static string diretorioRedes
        {
            get
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["DiretorioRedes"]))
                    return ConfigurationManager.AppSettings["DiretorioRedes"];
                else
                    return System.IO.Directory.GetCurrentDirectory() + "\\Redes\\";

            }
        }

        private string TreinarRedeNeural(string papel, int je, int js, int nn, double ta, int ct, List<double> dados, int shift)
        {
            string nomeRede = String.Format("{0}_je{1}_js{2}_nn{3}_ta{4}_ct{5}_dcv{6}_shift{7}_v{8}", papel, je, js, nn, ta.ToString().Replace('.', ','), ct, numeroDivisoesCrossValidation, shift, versao);

            double min = dados.Min();
            double max = dados.Max();

            RedeNeural.Treinar(papel, nomeRede, dados, je, js, nn, ta, ct, numeroDivisoesCrossValidation, shift);
            return nomeRede;
            //TreinarRedeNeural(nomeRede, dados);
            //Network nw = RecuperarRedeNeural(nomeRede);
        }

        public List<string> TreinarRedes()
        {
            string papel = "PETR4";
            List<DadosBE> dadosBE = DataBaseUtils.DataBaseUtils.RecuperarCotacoesAtivo(papel);
            //dadosBE = dadosBE.Take(dadosBE.Count - 120).ToList();
            List<double> dados = dadosBE.ConvertAll(cot => (double)cot.PrecoAbertura);
            List<int> listNumeroNeuronios = new List<int>() { 2, 4, 8, 12 };
            List<double> listTaxasAprendizado = new List<double>() { 0.1, 0.25, 0.5 };
            int ciclosTreinamento = 10000;
            List<KeyValuePair<int, int>> listInput_Output = new List<KeyValuePair<int, int>>();
            listInput_Output.Add(new KeyValuePair<int, int>(5, 1));
            listInput_Output.Add(new KeyValuePair<int, int>(10, 1));
            listInput_Output.Add(new KeyValuePair<int, int>(20, 1));
            listInput_Output.Add(new KeyValuePair<int, int>(30, 1));
            listInput_Output.Add(new KeyValuePair<int, int>(40, 1));
            listInput_Output.Add(new KeyValuePair<int, int>(50, 1));
            listInput_Output.Add(new KeyValuePair<int, int>(60, 1));
            listInput_Output.Add(new KeyValuePair<int, int>(10, 2));
            listInput_Output.Add(new KeyValuePair<int, int>(20, 2));
            listInput_Output.Add(new KeyValuePair<int, int>(30, 2));
            listInput_Output.Add(new KeyValuePair<int, int>(40, 2));
            listInput_Output.Add(new KeyValuePair<int, int>(50, 2));
            listInput_Output.Add(new KeyValuePair<int, int>(60, 2));
            listInput_Output.Add(new KeyValuePair<int, int>(10, 5));
            listInput_Output.Add(new KeyValuePair<int, int>(20, 5));
            listInput_Output.Add(new KeyValuePair<int, int>(30, 5));
            listInput_Output.Add(new KeyValuePair<int, int>(40, 5));
            listInput_Output.Add(new KeyValuePair<int, int>(50, 5));
            listInput_Output.Add(new KeyValuePair<int, int>(60, 5));
            listInput_Output.Add(new KeyValuePair<int, int>(20, 10));
            listInput_Output.Add(new KeyValuePair<int, int>(30, 10));
            listInput_Output.Add(new KeyValuePair<int, int>(40, 10));
            listInput_Output.Add(new KeyValuePair<int, int>(50, 10));
            listInput_Output.Add(new KeyValuePair<int, int>(60, 10));
            listInput_Output.Add(new KeyValuePair<int, int>(30, 15));
            listInput_Output.Add(new KeyValuePair<int, int>(40, 15));
            listInput_Output.Add(new KeyValuePair<int, int>(50, 15));
            listInput_Output.Add(new KeyValuePair<int, int>(60, 15));
            listInput_Output.Add(new KeyValuePair<int, int>(30, 20));
            listInput_Output.Add(new KeyValuePair<int, int>(40, 20));
            listInput_Output.Add(new KeyValuePair<int, int>(50, 20));
            listInput_Output.Add(new KeyValuePair<int, int>(60, 20));
            listInput_Output.Add(new KeyValuePair<int, int>(40, 25));
            listInput_Output.Add(new KeyValuePair<int, int>(50, 25));
            listInput_Output.Add(new KeyValuePair<int, int>(60, 25));
            listInput_Output.Add(new KeyValuePair<int, int>(50, 30));
            listInput_Output.Add(new KeyValuePair<int, int>(60, 30));
            List<string> nomeRedes = new List<string>();
            //foreach (int numeroNeuronios in listNumeroNeuronios)
            //{
            //    foreach (double taxaAprendizado in listTaxasAprendizado)
            //    {
            foreach (KeyValuePair<int, int> input_output in listInput_Output)
            {
                //Apagar
                DateTime dtNow = DateTime.Now;

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
                    List<double> dadosTreino = dados.Take(dados.Count / numeroDivisoesCrossValidation * i).ToList();
                    dadosTreino.AddRange(dados.Skip(dados.Count / numeroDivisoesCrossValidation * (i + 1)));
                    workerThreads[i] = new Thread(() => TreinarRedeNeural(papel, input_output.Key, input_output.Value, 4, 0.25, ciclosTreinamento, dadosTreino, iFixThread));//TreinarRedeNeural(papel, input_output.Key, input_output.Value, 4, 0.25, ciclosTreinamento, dadosTreino, i));
                    workerThreads[i].Start();
                    //nomeRedes.Add(TreinarRedeNeural(papel, input_output.Key, input_output.Value, 4, 0.25, ciclosTreinamento, dadosTreino, i));
                }
                for (int i = 0; i < numeroDivisoesCrossValidation; i++)
                {
                    workerThreads[i].Join();
                }

                //Apagar
                Console.WriteLine(String.Format("Input: {0}, Output: {1}, Tempo: {2}", input_output.Key, input_output.Value, DateTime.Now.Subtract(dtNow).ToString()));
            }
            //    }
            //}

            return nomeRedes;
        }

        public static List<string> ListarRedes(string papel)
        {
            //Lista os nomes das redes com seu diretorio completo
            List<string> nomesRedes = System.IO.Directory.GetFiles(diretorioRedes, "*.ndn").ToList();
            //Elimina o diretorio completo deixando apenas o nome sem a extensao
            nomesRedes = nomesRedes.ConvertAll(rede => rede = rede.Split('\\').Last().Replace(".ndn", ""));
            //Filtra apenas as redes do papel solicitado
            nomesRedes = nomesRedes.Where(n => n.ToUpper().StartsWith(papel.ToUpper() + "_")).OrderBy(nome => nome).ToList();
            //Remove o shift e o numero de divisoes  cross validation do nome da rede
            nomesRedes = nomesRedes.Select(nmRede => nmRede.Remove(nmRede.IndexOf("_v") - 12, 12)).Distinct().ToList();
            return nomesRedes;
            //return System.IO.Directory.GetFiles(diretorioRedes, "*.ndn").ToList().ConvertAll(rede => rede = rede.Split('\\').Last().Replace(".ndn", "")).Where(n => n.ToUpper().StartsWith(papel.ToUpper() + "_")).OrderBy(nome => nome).ToList();
        }

        public static Network RecuperarRedeNeural(string nomeRede)
        {
            using (Stream stream = File.Open(diretorioRedes + nomeRede + ".ndn", FileMode.Open))
            {
                IFormatter formatter = new BinaryFormatter();
                return (Network)formatter.Deserialize(stream);
            }
        }

        public static Dictionary<int, Network> RecuperarRedesNeuraisAgrupadasPorConfiguracao(string nomeRede)
        {
            Dictionary<int, Network> redes = new Dictionary<int, Network>();
            IFormatter formatter = new BinaryFormatter();
            for (int numShift = 0; numShift < numeroDivisoesCrossValidation; numShift++)
            {
                string nomeCompletoRede = nomeRede.Insert(nomeRede.IndexOf("_v"), String.Format("_dcv{0}_shift{1}", numeroDivisoesCrossValidation, numShift));
                using (Stream stream = File.Open(diretorioRedes + nomeCompletoRede + ".ndn", FileMode.Open))
                {
                    redes.Add(numShift, (Network)formatter.Deserialize(stream));
                }
            }

            return redes;
        }
    }
}
