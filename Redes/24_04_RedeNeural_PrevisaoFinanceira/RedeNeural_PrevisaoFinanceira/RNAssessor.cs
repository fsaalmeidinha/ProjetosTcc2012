using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuronDotNet.Core;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ConverterTabela;

namespace RedeNeural_PrevisaoFinanceira
{
    public class RNAssessor
    {
        public int JanelaEntrada = 20;
        public int JanelaSaida = 10;
        public int NumeroNeuronios = 8;
        public double TaxaAprendizado = 0.25;
        public int CiclosTreinamento = 5000;
        private int versao = 1;

        private void TreinarRedeNeural(string nomeRede, List<double> dados)
        {
            RedeNeural.Treinar(nomeRede, dados, JanelaEntrada, JanelaSaida, NumeroNeuronios, TaxaAprendizado, CiclosTreinamento);
        }

        public void ExecutarRedeNeural(string papel)
        {
            string nomeRede = String.Format("{0}_je{1}_js{2}_nn{3}_ta{4}_ct{5}_v{6}", papel, JanelaEntrada, JanelaSaida, NumeroNeuronios, TaxaAprendizado, CiclosTreinamento, versao);
            ConverterTabela.Converter c = new ConverterTabela.Converter();

            List<DadosBE> dadosBE = new ConverterTabela.Converter().DePara(papel).ToList().ConvertAll(dado => (DadosBE)dado);
            List<double> dados = dadosBE.ConvertAll(cot => (double)cot.PrecoAbertura);

            double min = dados.Min();
            double max = dados.Max();

            TreinarRedeNeural(nomeRede, dados);
            Network nw = RecuperarRedeNeural(nomeRede);
        }

        private Network RecuperarRedeNeural(string nomeRede)
        {
            using (Stream stream = File.Open(System.IO.Directory.GetCurrentDirectory() + "\\Redes\\" + nomeRede + ".ndn", FileMode.Open))
            {
                IFormatter formatter = new BinaryFormatter();
                return (Network)formatter.Deserialize(stream);
            }
        }

    }
}
