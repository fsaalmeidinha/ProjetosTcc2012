using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBaseUtils;
using System.Configuration;
using DataBaseUtils.Model;
using NeuronDotNet.Core.Backpropagation;
using NeuronDotNet.Core;
using NeuronDotNet.Core.Initializers;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace RedeNeuralPrevisaoFinanceira_v2
{
    internal class RedeNeural_v2
    {
        private static string diretorioRedes
        {
            get
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["DiretorioRedes"]))
                    return ConfigurationManager.AppSettings["DiretorioRedes"];
                else
                    return System.IO.Directory.GetCurrentDirectory();

            }
        }

        //public void TreinarRedeNeural(List<DadosBE> dadosBE, string papel = "PETR4")
        //{

        //}

        public static void Treinar(string papel, string nomeRedeNeural, List<DadosBE> dadosBE, int janelaEntrada, int numeroNeuronios, double taxaAprendizado, int ciclos, int numeroDivisoesCrossValidation, int shift)
        {
            if (dadosBE.Count < janelaEntrada)
                return;

            /*//inputLayerCount será a soma de 2+n ao valor da janela de entrada, onde n é o (TOE(tamanho original da entrada) - 5) / 5(arredondado para baixo). Ou seja, para cada 5 dias informados além de 5 iniciais, +1 cotação do dolar será informada
            //EX: passaremos a cotação do dia 1 ao dia 9 (9 dias) para prever o decimo dia. Portanto:
            //-TOE - 5 = 4.
            //4 / 5 = 0.8, arredondando para baixo, 0.
            //portanto apenas 2 cotações do dolar serão informadas: a do dia 1 e a do dia 9
            //de 10 a 14 dias de entrada, 3 cotaçoes do dolar. de 15 a 19, 4 cotações do dolar e assim por diante.
            int inputLayerCount = janelaEntrada + 2;

            //Somamos 2 a janela de entrada pois informaremos também a estação do ano e o valor de bollinger
            inputLayerCount += 2;

            //O primeiro valor será a cotação do ativo para o dia seguinte e o segundo valor a cotação do dolar para o dia seguinte
            int outputLayerCount = 2;
            */

            List<Treinamento> treinamentos = DataBaseUtils.DataBaseUtils.SelecionarTreinamentos_V2(dadosBE, janelaEntrada, 1);
            treinamentos = treinamentos.Where(trein => trein.DivisaoCrossValidation != shift).ToList();

            int inputLayerCount = treinamentos.First().Input.Count();
            int outputLayerCount = treinamentos.First().Output.Count();

            BackpropagationNetwork network;
            //int numeroNeuronios = 4;
            //double taxaAprendizado = 0.25d;

            ActivationLayer inputLayer = new LinearLayer(inputLayerCount);
            ActivationLayer hiddenLayer = new SigmoidLayer(numeroNeuronios);
            ActivationLayer outputLayer = new SigmoidLayer(outputLayerCount);
            new BackpropagationConnector(inputLayer, hiddenLayer).Initializer = new RandomFunction(0d, 0.3d);
            new BackpropagationConnector(hiddenLayer, outputLayer).Initializer = new RandomFunction(0d, 0.3d);
            network = new BackpropagationNetwork(inputLayer, outputLayer);
            network.SetLearningRate(taxaAprendizado);

            TrainingSet trainingSet = new TrainingSet(inputLayerCount, outputLayerCount);
            //foreach (KeyValuePair<double[], double[]> kvp in dadosPorJanelamento)
            //{
            //    trainingSet.Add(new TrainingSample(kvp.Key, kvp.Value));
            //}
            foreach (Treinamento treinamento in treinamentos)
            {
                trainingSet.Add(new TrainingSample(treinamento.Input.ToArray(), treinamento.Output.ToArray()));
            }

            network.EndEpochEvent += new TrainingEpochEventHandler(
                delegate(object senderNetwork, TrainingEpochEventArgs argsNw)
                {
                    //trainingProgressBar.Value = (int)(argsNw.TrainingIteration * 100d / cycles);
                    //Application.DoEvents();
                });

            bool erroAceito = false;
            int cicloAtual = ciclos / 2;
            while (erroAceito == false && cicloAtual <= ciclos)
            {
                erroAceito = true;
                network.Learn(trainingSet, cicloAtual);
                double erroGeralRede = 0;
                foreach (Treinamento treinamento in treinamentos)
                {
                    double[] previsao = network.Run(treinamento.Input.ToArray());
                    double erroRede = 1 - Math.Min(previsao.First(), treinamento.Output.First()) / Math.Max(previsao.First(), treinamento.Output.First());
                    erroGeralRede += erroRede;
                    if (erroRede > 0.01)//Verifica se houve mais de 1% de erro
                    {
                        trainingSet.Add(new TrainingSample(treinamento.Input.ToArray(), treinamento.Output.ToArray()));
                    }
                }
                erroGeralRede = erroGeralRede / treinamentos.Count;
                if (erroGeralRede > 0.01)
                    erroAceito = false;
                cicloAtual += ciclos / 2;
            }

            using (Stream stream = File.Open(diretorioRedes + "\\RedesPrevisaoFinanceira\\" + nomeRedeNeural + ".ndn", FileMode.Create))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, network);
            }
        }

    }
}
