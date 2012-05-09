using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuronDotNet.Core.Backpropagation;
using NeuronDotNet.Core.Initializers;
using NeuronDotNet.Core;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Configuration;

namespace RedeNeural_PrevisaoFinanceira
{
    internal class RedeNeural
    {
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

        public static void Treinar(string nomeRedeNeural, List<double> dadosTreinamento, int janelaEntrada, int janelaSaida, int numeroNeuronios, double taxaAprendizado, int ciclos)
        {
            if (dadosTreinamento.Count < janelaEntrada)
                return;

            /*Cria um mapeamento de entradas para saida com o janelamento informado*/
            List<KeyValuePair<double[], double[]>> dadosPorJanelamento = Utils.SelecionarCotacoesPorJanelamento(Utils.NormalizarDados(dadosTreinamento), janelaEntrada, janelaSaida, true);
            /*Cria um mapeamento de entradas para saida com o janelamento informado*/

            BackpropagationNetwork network;
            //int numeroNeuronios = 4;
            //double taxaAprendizado = 0.25d;

            ActivationLayer inputLayer = new LinearLayer(janelaEntrada);
            ActivationLayer hiddenLayer = new SigmoidLayer(numeroNeuronios);
            ActivationLayer outputLayer = new SigmoidLayer(janelaSaida);
            new BackpropagationConnector(inputLayer, hiddenLayer).Initializer = new RandomFunction(0d, 0.3d);
            new BackpropagationConnector(hiddenLayer, outputLayer).Initializer = new RandomFunction(0d, 0.3d);
            network = new BackpropagationNetwork(inputLayer, outputLayer);
            network.SetLearningRate(taxaAprendizado);

            TrainingSet trainingSet = new TrainingSet(janelaEntrada, janelaSaida);
            foreach (KeyValuePair<double[], double[]> kvp in dadosPorJanelamento)
            {
                trainingSet.Add(new TrainingSample(kvp.Key, kvp.Value));
            }

            network.EndEpochEvent += new TrainingEpochEventHandler(
                delegate(object senderNetwork, TrainingEpochEventArgs argsNw)
                {
                    //trainingProgressBar.Value = (int)(argsNw.TrainingIteration * 100d / cycles);
                    //Application.DoEvents();
                });

            bool erroAceito = false;
            int cicloAtual = ciclos / 5;
            while (erroAceito == false && cicloAtual <= ciclos)
            {
                network.Learn(trainingSet, cicloAtual);
                foreach (KeyValuePair<double[], double[]> kvp in dadosPorJanelamento)
                {
                    double[] previsao = network.Run(kvp.Key);
                    double erroAcumulado = 0;
                    for (int i = 0; i < janelaSaida; i++)
                    {
                        erroAcumulado += Math.Abs(100 - previsao[i] * 100 / kvp.Value[i]);
                    }
                    double erroMedio = erroAcumulado / janelaSaida;

                    if (erroMedio > 1.0)//Verifica se houve mais de 1% de erro
                    {
                        erroAceito = false;
                        trainingSet.Add(new TrainingSample(kvp.Key, kvp.Value));
                    }
                    else
                        erroAceito = true;
                }
                cicloAtual += ciclos / 5;
            }

            using (Stream stream = File.Open(diretorioRedes + nomeRedeNeural + ".ndn", FileMode.Create))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, network);
            }
        }
    }
}
