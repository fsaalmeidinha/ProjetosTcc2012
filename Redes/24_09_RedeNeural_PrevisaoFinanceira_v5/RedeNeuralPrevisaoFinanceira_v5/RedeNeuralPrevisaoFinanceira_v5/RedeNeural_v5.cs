using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using DataBaseUtils;
using DataBaseUtils.Model;
using NeuronDotNet.Core.Backpropagation;
using NeuronDotNet.Core;
using NeuronDotNet.Core.Initializers;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace RedeNeuralPrevisaoFinanceira_v5
{
    internal class RedeNeural_v5
    {
        private static string diretorioRedes
        {
            get
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["DiretorioRedes_V5"]))
                    return ConfigurationManager.AppSettings["DiretorioRedes_V5"];
                else
                    return System.IO.Directory.GetCurrentDirectory();
            }
        }

        public static void Treinar(string papel, string nomeRedeNeural, List<DadosBE> dadosBE, int numeroNeuronios, double taxaAprendizado, int ciclos, int numeroDivisoesCrossValidation, int shift, double versao)
        {
            if (dadosBE.Count == 0)
                return;

            List<Treinamento> treinamentos = DataBaseUtils.DataBaseUtils.SelecionarTreinamentos_V5(dadosBE, versao);
            treinamentos = treinamentos.Where(trein => trein.DivisaoCrossValidation != shift).ToList();

            //Trata os valores que estiverem fora do padrao
            foreach (Treinamento treinamento in treinamentos)
            {
                for (int indTreinamento = 0; indTreinamento < treinamento.Input.Count; indTreinamento++)
                {
                    if (treinamento.Input[indTreinamento] > 1)
                        treinamento.Input[indTreinamento] = 1;
                    if (treinamento.Input[indTreinamento] < 0)
                        treinamento.Input[indTreinamento] = 0;
                }
            }

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

            network.Learn(trainingSet, ciclos);
            double erroGeralRede = 0;
            foreach (Treinamento treinamento in treinamentos)
            {
                double[] previsao = network.Run(treinamento.Input.ToArray());
                double erroRede = 1 - Math.Min(previsao.First(), treinamento.Output.First()) / Math.Max(previsao.First(), treinamento.Output.First());
                erroGeralRede += erroRede;
            }
            erroGeralRede = erroGeralRede / treinamentos.Count;

            using (Stream stream = File.Open(diretorioRedes + "\\RedesPrevisaoFinanceira\\" + nomeRedeNeural + ".ndn", FileMode.Create))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, network);
            }
        }

    }
}
