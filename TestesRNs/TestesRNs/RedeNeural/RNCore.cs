using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuronDotNet.Core;
using NeuronDotNet.Core.Backpropagation;
using TestesRNs.Modelo;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Configuration;
using NeuronDotNet.Core.Initializers;

namespace TestesRNs.RedeNeural
{
    public class RNCore
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

        public static double Treinar(string nomeRedeNeural, List<Treinamento> treinamentos, int numeroNeuronios, double taxaAprendizado, int ciclos)
        {
            if (treinamentos.Count == 0)
                return -1;

            int inputLayerCount = treinamentos.First().Input.Count();
            int outputLayerCount = treinamentos.First().Output.Count();

            BackpropagationNetwork network;
            //int numeroNeuronios = 4;
            //double taxaAprendizado = 0.25d;

            ActivationLayer inputLayer = new LinearLayer(inputLayerCount);
            ActivationLayer hiddenLayer = new SigmoidLayer(numeroNeuronios);
            ActivationLayer outputLayer = new LinearLayer(outputLayerCount);
            new BackpropagationConnector(inputLayer, hiddenLayer).Initializer = new RandomFunction(0, 0.3d);
            new BackpropagationConnector(hiddenLayer, outputLayer).Initializer = new RandomFunction(0, 0.3d);
            network = new BackpropagationNetwork(inputLayer, outputLayer);
            network.SetLearningRate(taxaAprendizado);

            TrainingSet trainingSet = new TrainingSet(inputLayerCount, outputLayerCount);
            foreach (Treinamento treinamento in treinamentos)
            {
                trainingSet.Add(new TrainingSample(treinamento.Input.ToArray(), treinamento.Output.ToArray()));
            }

            double lastMeanSquareError = 1;
            network.EndEpochEvent += new TrainingEpochEventHandler(
                delegate(object senderNetwork, TrainingEpochEventArgs argsNw)
                {
                    if (argsNw.TrainingIteration > 0 && argsNw.TrainingIteration % 100 == 0)
                    {
                        double erroAt = Convert.ToDouble(senderNetwork.GetType().GetProperty("MeanSquaredError").GetValue(senderNetwork, null));
                        if (erroAt > lastMeanSquareError && Math.Abs(lastMeanSquareError - erroAt) < 0.01)
                        {
                            network.StopLearning();
                        }
                        else
                            lastMeanSquareError = erroAt;
                    }
                });

            network.Learn(trainingSet, ciclos);
            double erroGeralRede = 0;
            foreach (Treinamento treinamento in treinamentos)
            {
                double[] previsao = network.Run(treinamento.Input.ToArray());
                //double erroRede = 1 - Math.Min(previsao.First(), treinamento.Output.First()) / Math.Max(previsao.First(), treinamento.Output.First());
                double erroRede = Math.Abs(previsao.First() - treinamento.Output.First()) / treinamento.Output.First();
                erroGeralRede += erroRede;
            }
            erroGeralRede = erroGeralRede / treinamentos.Count;

            using (Stream stream = File.Open(diretorioRedes + nomeRedeNeural + ".ndn", FileMode.Create))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, network);
            }

            return erroGeralRede;
        }
    }
}
