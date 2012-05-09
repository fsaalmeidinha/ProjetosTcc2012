using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuronDotNet.Core;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ConverterTabela;
using System.Configuration;

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

        private string TreinarRedeNeural(string papel, int je, int js, int nn, double ta, int ct, List<double> dados)
        {
            string nomeRede = String.Format("{0}_je{1}_js{2}_nn{3}_ta{4}_ct{5}_v{6}", papel, je, js, nn, ta.ToString().Replace('.', ','), ct, versao);
            ConverterTabela.Converter c = new ConverterTabela.Converter();

            double min = dados.Min();
            double max = dados.Max();

            RedeNeural.Treinar(nomeRede, dados, je, js, nn, ta, ct);
            return nomeRede;
            //TreinarRedeNeural(nomeRede, dados);
            //Network nw = RecuperarRedeNeural(nomeRede);
        }

        public List<string> TreinarRedes()
        {
            string papel = "PETR4";
            List<DadosBE> dadosBE = new ConverterTabela.Converter().DePara(papel).ToList().ConvertAll(dado => (DadosBE)dado);
            List<double> dados = dadosBE.ConvertAll(cot => (double)cot.PrecoAbertura);
            List<int> listNumeroNeuronios = new List<int>() { 2, 4, 8, 12 };
            List<double> listTaxasAprendizado = new List<double>() { 0.1, 0.25, 0.5 };
            int ciclosTreinamento = 10000;
            List<KeyValuePair<int, int>> listInput_Output = new List<KeyValuePair<int, int>>();
            listInput_Output.Add(new KeyValuePair<int, int>(5, 1));
            listInput_Output.Add(new KeyValuePair<int, int>(10, 2));
            listInput_Output.Add(new KeyValuePair<int, int>(10, 5));
            listInput_Output.Add(new KeyValuePair<int, int>(15, 5));
            listInput_Output.Add(new KeyValuePair<int, int>(15, 10));
            listInput_Output.Add(new KeyValuePair<int, int>(20, 10));
            listInput_Output.Add(new KeyValuePair<int, int>(20, 15));
            listInput_Output.Add(new KeyValuePair<int, int>(30, 15));
            listInput_Output.Add(new KeyValuePair<int, int>(30, 20));
            listInput_Output.Add(new KeyValuePair<int, int>(40, 20));
            listInput_Output.Add(new KeyValuePair<int, int>(40, 25));
            listInput_Output.Add(new KeyValuePair<int, int>(50, 25));
            listInput_Output.Add(new KeyValuePair<int, int>(50, 30));
            listInput_Output.Add(new KeyValuePair<int, int>(60, 30));

            List<string> nomeRedes = new List<string>();
            foreach (int numeroNeuronios in listNumeroNeuronios)
            {
                foreach (double taxaAprendizado in listTaxasAprendizado)
                {
                    foreach (KeyValuePair<int, int> input_output in listInput_Output)
                    {
                        nomeRedes.Add(TreinarRedeNeural(papel, input_output.Key, input_output.Value, numeroNeuronios, taxaAprendizado, ciclosTreinamento, dados));
                    }
                }
            }

            return nomeRedes;
        }

        public static List<string> ListarRedes()
        {
            return System.IO.Directory.GetFiles(diretorioRedes, "*.ndn").ToList();
        }

        private Network RecuperarRedeNeural(string nomeRede)
        {
            using (Stream stream = File.Open(diretorioRedes + nomeRede + ".ndn", FileMode.Open))
            {
                IFormatter formatter = new BinaryFormatter();
                return (Network)formatter.Deserialize(stream);
            }
        }
    }
}
