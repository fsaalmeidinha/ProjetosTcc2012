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

namespace RedeNeuralPrevisaoFinanceira_v3
{
    internal class RedeNeural_v3
    {
        private static string diretorioRedes
        {
            get
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["DiretorioRedes_V3"]))
                    return ConfigurationManager.AppSettings["DiretorioRedes_V3"];
                else
                    return System.IO.Directory.GetCurrentDirectory();
            }
        }

        public static void Treinar(string papel, string nomeRedeNeural, List<DadosBE> dadosBE, int numeroNeuronios, double taxaAprendizado, int ciclos, int numeroDivisoesCrossValidation, int shift, double versao)
        {
            if (dadosBE.Count == 0)
                return;

            List<Treinamento> treinamentos = DataBaseUtils.DataBaseUtils.SelecionarTreinamentos_V3(dadosBE, versao);
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
                cicloAtual = 5000;
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
