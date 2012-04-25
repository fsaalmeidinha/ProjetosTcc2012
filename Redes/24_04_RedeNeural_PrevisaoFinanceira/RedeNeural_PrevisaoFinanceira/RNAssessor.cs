using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuronDotNet.Core;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace RedeNeural_PrevisaoFinanceira
{
    public class RNAssessor
    {
        int janelaEntrada = 20;
        int janelaSaida = 10;
        int numeroNeuronios = 8;
        double taxaAprendizado = 0.25;
        int ciclosTreinamento = 5000;

        private void TreinarRedeNeural(string nomeRede, List<double> dados)
        {
            RedeNeural.Treinar(nomeRede, dados, janelaEntrada, janelaSaida, numeroNeuronios, taxaAprendizado, ciclosTreinamento);
        }

        public void ExecutarRedeNeural(string papel)
        {
            string nomeRede = String.Format("{0}_je{1}_js{2}_nn{3}_ta{4}_ct{5}", papel, janelaEntrada, janelaSaida, numeroNeuronios, taxaAprendizado, ciclosTreinamento);
            List<HelloWorld.Model.Cotacao> cotacoes = HelloWorld.RN.CotacaoBovespaRN.LerCotacoesPorPapel("PETR4T ");
            List<double> dados = cotacoes.ConvertAll(cot => (double)cot.Valor);

            double min = dados.Min();
            double max = dados.Max();

            TreinarRedeNeural(nomeRede, dados);
            Network nw = GetNetwork(nomeRede);
        }

        private Network GetNetwork(string nomeRede)
        {
            using (Stream stream = File.Open(System.IO.Directory.GetCurrentDirectory() + "\\Redes\\" + nomeRede + ".ndn", FileMode.Open))
            {
                IFormatter formatter = new BinaryFormatter();
                return (Network)formatter.Deserialize(stream);
            }
        }

    }
}
