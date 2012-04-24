using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuronDotNet.Core.Backpropagation;
using NeuronDotNet.Core.Initializers;
using NeuronDotNet.Core;
using HelloWorld.Model;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace RedeNeural_PrevisaoFinanceira
{
    public class RedeNeural
    {
        static void Main(string[] args)
        {
            List<double> real = new List<double>();
            List<double> previsto = new List<double>();
        }

        public static void Run(ref List<double> real, ref List<double> previsto)
        {
            List<Cotacao> cotacoes = HelloWorld.RN.CotacaoBovespaRN.LerCotacoesPorPapel("PETR4 ");
            double min = 0;
            double max = 0;

            List<double> cotacoesNormalizadas = NormalizeValues(cotacoes.Select(cot => (double)cot.Valor), ref min, ref max);
            int janela = 3;
            Training(cotacoesNormalizadas.Take(cotacoesNormalizadas.Count / 10 * 8).ToList(), janela, 10000);
            Network nw = GetNetwork();

            List<KeyValuePair<double[], double>> dadosPorJanelamento = GetCotacoesPorJanelamento(cotacoesNormalizadas.Take(cotacoesNormalizadas.Count / 10 * 8).ToList(), janela);

            for (int i = 0; i < dadosPorJanelamento.Count; i++)
            {
                //double previsao = nw.Run(dadosPorJanelamento[i].Key)[0] * (max - min) + min;
                double previsao = Desnormalizar(min, max, nw.Run(dadosPorJanelamento[i].Key)[0]);
                System.Diagnostics.Debug.WriteLine(String.Format("Previsao: {0}  |  Real: {1}  |  ErroAbsoluto: {2}  |  PorcentagemErro: {3}",
                    previsao,
                    Desnormalizar(min, max, dadosPorJanelamento[i].Value),
                    Desnormalizar(min, max, dadosPorJanelamento[i].Value) - previsao,
                    (previsao / Desnormalizar(min, max, dadosPorJanelamento[i].Value) - 1) * 100));
            }
            //return cotacoes.Select(cot => (cot - minVal) / (maxVal - minVal)).ToList();

            for (int i = 0; i < 10; i++)
                System.Diagnostics.Debug.WriteLine("  --  ");

            /*Dados Conhecidos*/
            List<double> previsoesDadosConhecidos = new List<double>();
            List<double> previsoesDadosDesconhecidos = new List<double>();
            double[] input = new double[janela];
            //Inicia com o dado de 3 dias (conforme janela)
            for (int i = 0; i < janela; i++)
            {
                input[i] = cotacoesNormalizadas.Take(cotacoesNormalizadas.Count / 10 * 8).Skip(i).First();
            }
            previsoesDadosConhecidos.Add(Desnormalizar(min, max, nw.Run(input)[0]));
            foreach (double val in cotacoesNormalizadas.Take(cotacoesNormalizadas.Count / 10 * 8).Skip(janela))
            {
                for (int i = 0; i < janela - 1; i++)
                {
                    input[i] = input[i + 1];
                }
                input[janela - 1] = val;
                previsoesDadosConhecidos.Add(Desnormalizar(min, max, nw.Run(input)[0]));
            }
            /*Dados Conhecidos*/

            /*Dados Desconhecidos*/
            //Inicia com o dado de 3 dias (conforme janela)
            for (int i = 0; i < janela; i++)
            {
                input[i] = cotacoesNormalizadas.Take(cotacoesNormalizadas.Count / 10 * 8).Skip(i).First();
            }
            previsoesDadosDesconhecidos.Add(Desnormalizar(min, max, nw.Run(input)[0]));
            foreach (double val in cotacoesNormalizadas.Skip(cotacoesNormalizadas.Count / 10 * 8).Skip(janela))
            {
                for (int i = 0; i < janela - 1; i++)
                {
                    input[i] = input[i + 1];
                }
                input[janela - 1] = Normalizar(min, max, previsoesDadosDesconhecidos.Last());
                previsoesDadosDesconhecidos.Add(Desnormalizar(min, max, nw.Run(input)[0]));
            }
            /*Dados Desconhecidos*/

            System.Diagnostics.Debug.WriteLine(String.Format("DadoConhecido: {0}  -  DadoDesconhecido: {1}  -  Erro:{2}",
                previsoesDadosConhecidos.Last(),
                previsoesDadosDesconhecidos.Last(),
                previsoesDadosConhecidos.Last() - previsoesDadosDesconhecidos.Last()));

            //real = cotacoesNormalizadas.Skip(cotacoesNormalizadas.Count / 10 * 8).Select(cot => Desnormalizar(min, max, cot)).ToList();
            real = cotacoesNormalizadas.Take(cotacoesNormalizadas.Count / 10 * 8).Select(cot => Desnormalizar(min, max, cot)).ToList();
            //previsto = previsoesDadosDesconhecidos.ToList();
            previsto = previsoesDadosConhecidos.ToList();
        }

        static void Training(List<double> dadosTreinamento, int janela, int ciclos)
        {
            if (dadosTreinamento.Count < janela)
                return;

            /*Cria um mapeamento de entradas para saida com o janelamento informado*/
            //List<KeyValuePair<double[], double>> dadosPorJanelamento = new List<KeyValuePair<double[], double>>();
            //for (int i = 0; i < dadosTreinamento.Count - janela - 1; i += janela + 1)
            //{
            //    dadosPorJanelamento.Add(new KeyValuePair<double[], double>(dadosTreinamento.Skip(i).Take(janela).ToArray(), dadosTreinamento.Skip(i).Take(janela + 1).First()));
            //}
            List<KeyValuePair<double[], double>> dadosPorJanelamento = GetCotacoesPorJanelamento(dadosTreinamento, janela);
            /*Cria um mapeamento de entradas para saida com o janelamento informado*/

            BackpropagationNetwork network;
            int neuronCount = 4;
            double learningRate = 0.25d;

            ActivationLayer inputLayer = new LinearLayer(janela);
            SigmoidLayer hiddenLayer = new SigmoidLayer(neuronCount);
            SigmoidLayer outputLayer = new SigmoidLayer(1);
            new BackpropagationConnector(inputLayer, hiddenLayer).Initializer = new RandomFunction(0d, 0.3d);
            new BackpropagationConnector(hiddenLayer, outputLayer).Initializer = new RandomFunction(0d, 0.3d);
            network = new BackpropagationNetwork(inputLayer, outputLayer);
            network.SetLearningRate(learningRate);

            TrainingSet trainingSet = new TrainingSet(janela, 1);
            foreach (KeyValuePair<double[], double> kvp in dadosPorJanelamento)
            {
                trainingSet.Add(new TrainingSample(kvp.Key, new double[] { kvp.Value }));
            }

            network.EndEpochEvent += new TrainingEpochEventHandler(
                delegate(object senderNetwork, TrainingEpochEventArgs argsNw)
                {
                    //trainingProgressBar.Value = (int)(argsNw.TrainingIteration * 100d / cycles);
                    //Application.DoEvents();
                });

            bool correct = false;
            int currentCycles = ciclos / 5;
            while (correct == false && currentCycles <= ciclos)
            {
                network.Learn(trainingSet, currentCycles);
                foreach (KeyValuePair<double[], double> kvp in dadosPorJanelamento)
                {
                    double previsao = network.Run(kvp.Key)[0];
                    if (Math.Abs(kvp.Value - previsao) > (kvp.Value / 100 * 0.5))//Verifica se houve mais de 5% de erro
                    {
                        correct = false;
                        trainingSet.Add(new TrainingSample(kvp.Key, new double[] { kvp.Value }));
                    }
                    else
                        correct = true;
                }
                currentCycles += ciclos / 5;
            }

            using (Stream stream = File.Open(System.IO.Directory.GetCurrentDirectory() + @"\network.ndn", FileMode.Create))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, network);
            }
        }

        static List<KeyValuePair<double[], double>> GetCotacoesPorJanelamento(List<double> dados, int janela)
        {
            /*Cria um mapeamento de entradas para saida com o janelamento informado*/
            List<KeyValuePair<double[], double>> dadosPorJanelamento = new List<KeyValuePair<double[], double>>();
            for (int i = 0; i < dados.Count - janela - 1; i += janela + 1)
            {
                dadosPorJanelamento.Add(new KeyValuePair<double[], double>(dados.Skip(i).Take(janela).ToArray(), dados.Skip(i).Take(janela + 1).First()));
            }
            /*Cria um mapeamento de entradas para saida com o janelamento informado*/

            return dadosPorJanelamento;
        }

        static Network GetNetwork()
        {
            using (Stream stream = File.Open(System.IO.Directory.GetCurrentDirectory() + @"\network.ndn", FileMode.Open))
            {
                IFormatter formatter = new BinaryFormatter();
                return (Network)formatter.Deserialize(stream);
            }
        }

        static List<double> NormalizeValues(IEnumerable<double> cotacoes, ref double min, ref double max)
        {
            if (cotacoes == null)
                return null;
            else if (cotacoes.Count() == 0)
                return new List<double>();

            double maxVal = max = cotacoes.Max() * 1.5f;
            double minVal = min = cotacoes.Min() / 1.5f;

            //return cotacoes.Select(cot => (cot - minVal) / (maxVal - minVal)).ToList();
            return cotacoes.Select(cot => Normalizar(minVal, maxVal, cot)).ToList();
        }

        static double Normalizar(double min, double max, double valor)
        {
            return (valor - min) / (max - min);
        }

        static double Desnormalizar(double min, double max, double valor)
        {
            return valor * (max - min) + min;
        }
    }
}
