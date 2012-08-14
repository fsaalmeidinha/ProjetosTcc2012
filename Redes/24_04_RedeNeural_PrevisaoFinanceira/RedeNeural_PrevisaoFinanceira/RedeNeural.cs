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
using DataBaseUtils.Model;

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

        public static void Treinar(string papel, string nomeRedeNeural, List<double> dadosTreinamento, int janelaEntrada, int janelaSaida, int numeroNeuronios, double taxaAprendizado, int ciclos, int numeroDivisoesCrossValidation, int shift)
        {
            if (dadosTreinamento.Count < janelaEntrada)
                return;

            List<Treinamento> treinamentos = DataBaseUtils.DataBaseUtils.SelecionarTreinamentos(DataBaseUtils.DataBaseUtils.NormalizarDados(dadosTreinamento, papel), janelaEntrada, janelaSaida, 1);
            treinamentos = treinamentos.Where(trein => trein.DivisaoCrossValidation != shift).ToList();
            ///*Cria um mapeamento de entradas para saida com o janelamento informado*/
            //List<KeyValuePair<double[], double[]>> dadosPorJanelamento = DataBaseUtils.DataBaseUtils.SelecionarCotacoesPorJanelamentoPulandoNDias(DataBaseUtils.DataBaseUtils.NormalizarDados(dadosTreinamento.Take((dadosTreinamento.Count / (numeroDivisoesCrossValidation - 1)) * shift).ToList(), papel), janelaEntrada, janelaSaida, 2);
            ////Corta em 2 chamdas de métodos para nao juntar os dados antes da quebra de registros e depois da quebra de registros, pois poderia estar juntando dados de dias muito distantes
            //dadosPorJanelamento.AddRange(DataBaseUtils.DataBaseUtils.SelecionarCotacoesPorJanelamentoPulandoNDias(DataBaseUtils.DataBaseUtils.NormalizarDados(dadosTreinamento.Skip((dadosTreinamento.Count / (numeroDivisoesCrossValidation - 1)) * (shift + 1)).ToList(), papel), janelaEntrada, janelaSaida, 2));
            ///*Cria um mapeamento de entradas para saida com o janelamento informado*/

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
                    double erroAcumulado = 0;
                    for (int i = 0; i < janelaSaida; i++)
                    {
                        erroAcumulado += 1 - Math.Min(previsao[i], treinamento.Output[i]) / Math.Max(previsao[i], treinamento.Output[i]);
                        //erroAcumulado += Math.Abs(100 - previsao[i] * 100 / kvp.Value[i]);
                    }
                    double erroMedio = erroAcumulado / janelaSaida;
                    erroGeralRede += erroMedio;
                    if (erroMedio > 0.01)//Verifica se houve mais de 1% de erro
                    {
                        trainingSet.Add(new TrainingSample(treinamento.Input.ToArray(), treinamento.Output.ToArray()));
                    }
                }
                erroGeralRede = erroGeralRede / treinamentos.Count;
                if (erroGeralRede > 0.01)
                    erroAceito = false;
                cicloAtual += ciclos / 2;
            }

            using (Stream stream = File.Open(diretorioRedes + nomeRedeNeural + ".ndn", FileMode.Create))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, network);
            }
        }

        public List<string> TreinarRedes()
        {
            string papel = "PETR4";
            List<DadosBE> dadosBE = DataBaseUtils.DataBaseUtils.RecuperarCotacoesAtivo(papel);
            //dadosBE = dadosBE.Take(dadosBE.Count - 120).ToList();
            List<double> dados = dadosBE.ConvertAll(cot => (double)cot.PrecoAbertura);
            List<int> listNumeroNeuronios = new List<int>() { 2, 4, 8, 12 };
            List<double> listTaxasAprendizado = new List<double>() { 0.1, 0.25, 0.5 };
            int ciclosTreinamento = 50000;
            List<KeyValuePair<int, int>> listInput_Output = new List<KeyValuePair<int, int>>();
            //listInput_Output.Add(new KeyValuePair<int, int>(5, 1));
            //listInput_Output.Add(new KeyValuePair<int, int>(10, 1));
            //listInput_Output.Add(new KeyValuePair<int, int>(20, 1));
            //listInput_Output.Add(new KeyValuePair<int, int>(30, 1));
            //listInput_Output.Add(new KeyValuePair<int, int>(40, 1));
            //listInput_Output.Add(new KeyValuePair<int, int>(50, 1));
            //listInput_Output.Add(new KeyValuePair<int, int>(60, 1));
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
    }
}
