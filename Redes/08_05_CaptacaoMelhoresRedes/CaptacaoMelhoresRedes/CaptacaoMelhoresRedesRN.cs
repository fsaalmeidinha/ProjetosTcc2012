using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedeNeural_PrevisaoFinanceira;
using NeuronDotNet.Core;
using NeuronDotNet.Core.Backpropagation;
using NeuronDotNet.Core.Initializers;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Configuration;
using ConverterTabela;

namespace CaptacaoMelhoresRedes
{
    public class CaptacaoMelhoresRedesRN
    {
        //Número de dias de previsão permitidas
        private int totalDiasPrevisao = 30;
        private int versao = 1;
        private int ciclos = 10000;
        private static string diretorioRedesCaptacao
        {
            get
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["DiretorioRedesCaptacao"]))
                    return ConfigurationManager.AppSettings["DiretorioRedesCaptacao"];
                else
                    return System.IO.Directory.GetCurrentDirectory() + "\\RedesCaptacao\\";

            }
        }

        public List<string> Teste()
        {
            List<string> redes = RNAssessor.ListarRedes();
            ConfiguracaoCaptacaoRedes configuracaoCaptacao = new ConfiguracaoCaptacaoRedes();

            List<DadosBE> dadosBE = new ConverterTabela.Converter().DePara("PETR4").ToList().ConvertAll(dado => (DadosBE)dado);
            configuracaoCaptacao.Dados = Utils.NormalizarDados(dadosBE.Select(dadoBE => (double)dadoBE.PrecoAbertura).ToList());
            configuracaoCaptacao.Papel = "PETR4";
            foreach (string nomeRede in redes)
            {
                Network redeNeuralPrevisaoFinanceira = RNAssessor.RecuperarRedeNeural(nomeRede);
                RedePrevisaoFinanceira rpf = new RedePrevisaoFinanceira();
                rpf.JanelaEntrada = Convert.ToInt32(nomeRede.Split(new string[] { "_je", "_js" }, StringSplitOptions.RemoveEmptyEntries)[1]);
                rpf.JanelaSaida = Convert.ToInt32(nomeRede.Split(new string[] { "_js", "_nn" }, StringSplitOptions.RemoveEmptyEntries)[1]);
                rpf.RedeNeuralPrevisaoFinanceira = redeNeuralPrevisaoFinanceira;

                configuracaoCaptacao.RedesPrevisao.Add(rpf);
            }

            TreinarRedes(configuracaoCaptacao);

            return redes;
        }

        private void TreinarRedes(ConfiguracaoCaptacaoRedes configuracaoCaptacao)
        {
            //Seleção dos dados para o treinamento
            Dictionary<List<double>, List<double>> dadosTreinamento = SelecionarInput_Output(configuracaoCaptacao.Dados, configuracaoCaptacao.RedesPrevisao.Max(rp => rp.JanelaEntrada));

            //Inicializa todos os dias da previsao
            for (int diaTreinamento = 0; diaTreinamento < totalDiasPrevisao; diaTreinamento++)
            {
                foreach (RedePrevisaoFinanceira redePrevisao in configuracaoCaptacao.RedesPrevisao)
                {
                    redePrevisao.TaxaMediaAcertoPorDia.Add(diaTreinamento, 0);
                }
            }

            //Seleciona os treinamentos por dia de previsao
            Dictionary<int, TrainingSet> treinamentosPorDia = SelecionarTreinamentosPorDia(dadosTreinamento, configuracaoCaptacao.RedesPrevisao);//new Dictionary<int, TrainingSet>();

            foreach (KeyValuePair<int, TrainingSet> treinamento in treinamentosPorDia)
            {
                string nomeRede = string.Format("captacaoMelhorRede_{0}_{1}", treinamento.Key, versao);
                TreinarRede(nomeRede, treinamento.Value);
            }
        }

        private void TreinarRede(string nomeRede, TrainingSet trainingSet)
        {
            BackpropagationNetwork network;
            int numeroNeuronios = 4;
            double taxaAprendizado = 0.25d;

            ActivationLayer inputLayer = new LinearLayer(trainingSet.InputVectorLength);
            ActivationLayer hiddenLayer = new SigmoidLayer(numeroNeuronios);
            ActivationLayer outputLayer = new SigmoidLayer(trainingSet.OutputVectorLength);
            new BackpropagationConnector(inputLayer, hiddenLayer).Initializer = new RandomFunction(0d, 0.3d);
            new BackpropagationConnector(hiddenLayer, outputLayer).Initializer = new RandomFunction(0d, 0.3d);
            network = new BackpropagationNetwork(inputLayer, outputLayer);
            network.SetLearningRate(taxaAprendizado);

            //TrainingSet trainingSet = new TrainingSet(janelaEntrada, janelaSaida);
            //foreach (KeyValuePair<double[], double[]> kvp in dadosPorJanelamento)
            //{
            //    trainingSet.Add(new TrainingSample(kvp.Key, kvp.Value));
            //}

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
                foreach (TrainingSample treinamento in trainingSet.TrainingSamples.Distinct())
                {
                    double[] previsao = network.Run(treinamento.InputVector);
                    double erroAcumulado = 0;
                    for (int indRede = 0; indRede < trainingSet.OutputVectorLength; indRede++)
                    {
                        erroAcumulado += 1 - Math.Min(previsao[indRede], treinamento.OutputVector[indRede]) / Math.Max(previsao[indRede], treinamento.OutputVector[indRede]);
                    }
                    double erroMedio = erroAcumulado / trainingSet.TrainingSampleCount;

                    if (erroMedio > 0.3)//Verifica se houve mais de 3% de erro
                    {
                        erroAceito = false;
                        trainingSet.Add(treinamento);
                    }
                    else
                        erroAceito = erroAceito && true;
                }
                cicloAtual += ciclos / 5;
            }

            using (Stream stream = File.Open(diretorioRedesCaptacao + nomeRede + ".ndn", FileMode.Create))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, network);
            }
        }

        private Dictionary<int, TrainingSet> SelecionarTreinamentosPorDia(Dictionary<List<double>, List<double>> dadosTreinamento, List<RedePrevisaoFinanceira> redesPrevisaoFinanceira)
        {
            Dictionary<int, TrainingSet> treinamentosPorDia = new Dictionary<int, TrainingSet>();
            for (int dia = 0; dia < totalDiasPrevisao; dia++)
            {
                treinamentosPorDia.Add(dia, new TrainingSet(dadosTreinamento.First().Key.Count, redesPrevisaoFinanceira.Count));
            }

            //Roda as redes para cada um dos dados de treinamento selecionados
            foreach (KeyValuePair<List<double>, List<double>> dadoTreinamento in dadosTreinamento)
            {
                List<double[]> taxaAcertoPorDiaParaTreinamento = new List<double[]>();
                foreach (RedePrevisaoFinanceira redePrevisao in redesPrevisaoFinanceira)
                {
                    //Roda a rede neural, gerando a previsao
                    double[] outputPrevisao = redePrevisao.RedeNeuralPrevisaoFinanceira.Run(dadoTreinamento.Key.Take(redePrevisao.JanelaEntrada).ToArray());
                    double[] taxaAcertoPorDia = new double[totalDiasPrevisao];
                    for (int diasPrevistosRN = 0; diasPrevistosRN < outputPrevisao.Length; diasPrevistosRN++)
                    {
                        //Calcula a taxa de acerto da previsao
                        double ta = Math.Min(outputPrevisao[diasPrevistosRN], dadoTreinamento.Value[diasPrevistosRN]) / Math.Max(outputPrevisao[diasPrevistosRN], dadoTreinamento.Value[diasPrevistosRN]);
                        taxaAcertoPorDia[diasPrevistosRN] = ta;
                        redePrevisao.TaxaMediaAcertoPorDia[diasPrevistosRN] += ta;
                    }
                    taxaAcertoPorDiaParaTreinamento.Add(taxaAcertoPorDia);
                }

                for (int dia = 0; dia < totalDiasPrevisao; dia++)
                {
                    double[] outputCaptacao = new double[redesPrevisaoFinanceira.Count];
                    for (int indRede = 0; indRede < redesPrevisaoFinanceira.Count; indRede++)
                    {
                        outputCaptacao[indRede] = taxaAcertoPorDiaParaTreinamento[indRede][dia];
                    }
                    TrainingSample ts = new TrainingSample(dadoTreinamento.Key.ToArray(), outputCaptacao);
                    treinamentosPorDia[dia].Add(ts);
                }
            }

            //Divide a taxa de acerto pela quantidade de treinamentos para saber a taxa média
            foreach (RedePrevisaoFinanceira redePrevisao in redesPrevisaoFinanceira)
            {
                for (int i = 0; i < totalDiasPrevisao; i++)
                {
                    redePrevisao.TaxaMediaAcertoPorDia[i] /= dadosTreinamento.Count;
                }
            }

            return treinamentosPorDia;
        }

        private Dictionary<List<double>, List<double>> SelecionarInput_Output(List<double> dados, int tamanhoInput)
        {
            Dictionary<List<double>, List<double>> dadosTreinamento = new Dictionary<List<double>, List<double>>();
            for (int i = 0; i < dados.Count - (tamanhoInput + totalDiasPrevisao); i += 3)
            {
                dadosTreinamento.Add(dados.Skip(i).Take(tamanhoInput).ToList(), dados.Skip(i + tamanhoInput).Take(totalDiasPrevisao).ToList());
            }
            return dadosTreinamento;
        }

        internal class ConfiguracaoCaptacaoRedes
        {
            public ConfiguracaoCaptacaoRedes()
            {
                RedesPrevisao = new List<RedePrevisaoFinanceira>();
            }

            public string Papel { get; set; }
            public List<double> Dados { get; set; }
            public List<RedePrevisaoFinanceira> RedesPrevisao { get; set; }
        }

        internal class RedePrevisaoFinanceira
        {
            public RedePrevisaoFinanceira()
            {
                TaxaMediaAcertoPorDia = new Dictionary<int, double>();
            }

            public int JanelaEntrada { get; set; }
            public int JanelaSaida { get; set; }
            public Network RedeNeuralPrevisaoFinanceira { get; set; }
            public Dictionary<int, double> TaxaMediaAcertoPorDia { get; set; }
        }
        //public static void Treinar(string nomeRedeNeural, List<double> dadosTreinamento, int janelaEntrada, int janelaSaida, int numeroNeuronios, double taxaAprendizado, int ciclos)
        //{
        //    if (dadosTreinamento.Count < janelaEntrada)
        //        return;

        //    /*Cria um mapeamento de entradas para saida com o janelamento informado*/
        //    List<KeyValuePair<double[], double[]>> dadosPorJanelamento = Utils.SelecionarCotacoesPorJanelamento(Utils.NormalizarDados(dadosTreinamento), janelaEntrada, janelaSaida, true);
        //    /*Cria um mapeamento de entradas para saida com o janelamento informado*/

        //    BackpropagationNetwork network;
        //    //int numeroNeuronios = 4;
        //    //double taxaAprendizado = 0.25d;

        //    ActivationLayer inputLayer = new LinearLayer(janelaEntrada);
        //    ActivationLayer hiddenLayer = new SigmoidLayer(numeroNeuronios);
        //    ActivationLayer outputLayer = new SigmoidLayer(janelaSaida);
        //    new BackpropagationConnector(inputLayer, hiddenLayer).Initializer = new RandomFunction(0d, 0.3d);
        //    new BackpropagationConnector(hiddenLayer, outputLayer).Initializer = new RandomFunction(0d, 0.3d);
        //    network = new BackpropagationNetwork(inputLayer, outputLayer);
        //    network.SetLearningRate(taxaAprendizado);

        //    TrainingSet trainingSet = new TrainingSet(janelaEntrada, janelaSaida);
        //    foreach (KeyValuePair<double[], double[]> kvp in dadosPorJanelamento)
        //    {
        //        trainingSet.Add(new TrainingSample(kvp.Key, kvp.Value));
        //    }

        //    network.EndEpochEvent += new TrainingEpochEventHandler(
        //        delegate(object senderNetwork, TrainingEpochEventArgs argsNw)
        //        {
        //            //trainingProgressBar.Value = (int)(argsNw.TrainingIteration * 100d / cycles);
        //            //Application.DoEvents();
        //        });

        //    bool erroAceito = false;
        //    int cicloAtual = ciclos / 5;
        //    while (erroAceito == false && cicloAtual <= ciclos)
        //    {
        //        network.Learn(trainingSet, cicloAtual);
        //        foreach (KeyValuePair<double[], double[]> kvp in dadosPorJanelamento)
        //        {
        //            double[] previsao = network.Run(kvp.Key);
        //            double erroAcumulado = 0;
        //            for (int i = 0; i < janelaSaida; i++)
        //            {
        //                erroAcumulado += Math.Abs(100 - previsao[i] * 100 / kvp.Value[i]);
        //            }
        //            double erroMedio = erroAcumulado / janelaSaida;

        //            if (erroMedio > 1.0)//Verifica se houve mais de 1% de erro
        //            {
        //                erroAceito = false;
        //                trainingSet.Add(new TrainingSample(kvp.Key, kvp.Value));
        //            }
        //            else
        //                erroAceito = true;
        //        }
        //        cicloAtual += ciclos / 5;
        //    }

        //    //using (Stream stream = File.Open(System.IO.Directory.GetCurrentDirectory() + "\\" + nomeRedeNeural + ".ndn", FileMode.Create))
        //    using (Stream stream = File.Open(System.IO.Directory.GetCurrentDirectory() + "\\Redes\\" + nomeRedeNeural + ".ndn", FileMode.Create))
        //    {
        //        IFormatter formatter = new BinaryFormatter();
        //        formatter.Serialize(stream, network);
        //    }
        //}
    }
}
