﻿using System;
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
using CaptacaoMelhoresRedes.Model;
using OfficeOpenXml;
using DataBaseUtils.Model;
using System.Threading;

namespace CaptacaoMelhoresRedes
{
    public class CaptacaoMelhoresRedesRN
    {
        //Número de dias de previsão permitidas
        private static int totalDiasPrevisao = 30;
        internal static int Versao = 1;
        private static int ciclos = 10000;
        private static int quantidadeShifts = 8;
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
        private static string diretorioRelatorioCrossOver
        {
            get
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["DiretorioRelatorioCrossOver"]))
                    return ConfigurationManager.AppSettings["DiretorioRelatorioCrossOver"];
                else
                    return System.IO.Directory.GetCurrentDirectory() + "\\DiretorioRelatorioCrossOver\\";

            }
        }

        /// <summary>
        /// Treina as redes de captação necessárias para identificar a melhor rede para cada um dos dias de previsao
        /// Por exemplo: se forem permitidos 30 dias de previsao, teremos 30 redes para cada papel
        /// </summary>
        /// <param name="configuracaoCaptacao"></param>
        internal static void Treinar(ConfiguracaoCaptacaoRedes configuracaoCaptacao)
        {
            //Seleciona os treinamentos por dia de previsao, sendo o input os dados de entrada da rede de previsao financeira e como saida,
            //a taxa de acerto de cada uma das redes executadas em cada dia de previsao
            //OBS: A taxa de acerto da rede por dia é alimentada
            Dictionary<int, TrainingSet> treinamentosPorDia = SelecionarTreinamentosPorDia(configuracaoCaptacao);//dadosTreinamentoPorShift, configuracaoCaptacao.RedesPrevisao);

            GerarRelatorioCrossOver(String.Format("{0}_{1}_{2}", Directory.GetFiles(diretorioRelatorioCrossOver).Count() + 1, configuracaoCaptacao.Papel, DateTime.Now.ToString().Replace("/", "_").Replace(":", "_".Replace(" ", ""))), configuracaoCaptacao.RedesPrevisao);

            //foreach (KeyValuePair<int, TrainingSet> treinamento in treinamentosPorDia)
            //{
            //    string nomeRede = CaptacaoMelhoresRedesRN.RecuperarNomeRede(configuracaoCaptacao.Papel, treinamento.Key);
            //    TreinarRedeDiaria(nomeRede, treinamento.Value);
            //}
            int numeroProcessadores = 4;
            for (int indTrein = 0; indTrein < treinamentosPorDia.Count; indTrein += numeroProcessadores)
            {
                Thread[] threads = new Thread[numeroProcessadores];
                for (int indProcessador = 0; indProcessador < numeroProcessadores; indProcessador++)
                {
                    if (treinamentosPorDia.Count - (indTrein + indProcessador) <= 0)
                        continue;
                    string nomeRede = CaptacaoMelhoresRedesRN.RecuperarNomeRede(configuracaoCaptacao.Papel, indTrein + indProcessador);
                    TrainingSet ts = treinamentosPorDia[indTrein + indProcessador];
                    threads[indProcessador] = new Thread(() => TreinarRedeDiaria(nomeRede, ts));//[indTrein + indProcessador];
                    threads[indProcessador].Start();
                }
                for (int indProcessador = 0; indProcessador < numeroProcessadores; indProcessador++)
                {
                    if (threads[indProcessador] != null)
                        threads[indProcessador].Join();
                }
            }
        }

        internal static string RecuperarNomeRede(string papel, int dia)
        {
            return string.Format("CaptacaoMelhorRede_papel{0}_dia{1}_v{2}", papel, dia, CaptacaoMelhoresRedesRN.Versao);
        }

        /// <summary>
        /// Treina a rede que diz qual é a melhor configuração de rede para um papel em um dia
        /// </summary>
        /// <param name="nomeRede"></param>
        /// <param name="trainingSet"></param>
        private static void TreinarRedeDiaria(string nomeRede, TrainingSet trainingSet)
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

        /// <summary>
        /// Roda as redes para cada um dos treinamentos, separando os treinamentos por dia
        /// Calcula também a taxa média de acerto de cada rede neural por dia
        /// </summary>
        /// <param name="dadosTreinamento"></param>
        /// <param name="redesPrevisaoFinanceira"></param>
        /// <returns></returns>
        private static Dictionary<int, TrainingSet> SelecionarTreinamentosPorDia(ConfiguracaoCaptacaoRedes configuracaoCaptacao)
        {
            int tamanhoEntrada = configuracaoCaptacao.RedesPrevisao.Max(rp => rp.JanelaEntrada);
            List<Treinamento> treinamentos = DataBaseUtils.DataBaseUtils.SelecionarTreinamentos(configuracaoCaptacao.Dados, tamanhoEntrada, totalDiasPrevisao, 2);
            //Dictionary<int, Dictionary<List<double>, List<double>>> dadosTreinamentoPorShift = new Dictionary<int, Dictionary<List<double>, List<double>>>();
            ////Resgata o maior tamanho das entradas das redes neurais
            ////Cria o dicionario de input output por shift
            //for (int numShift = 0; numShift < quantidadeShifts; numShift++)
            //{
            //    //Seleciona oa dados antes do shift
            //    dadosTreinamentoPorShift.Add(numShift, SelecionarInput_Output(configuracaoCaptacao.Dados.Take((configuracaoCaptacao.Dados.Count / (quantidadeShifts - 1)) * numShift).ToList(), tamanhoEntrada));
            //    //Seleciona os dados depois do shift
            //    dadosTreinamentoPorShift[numShift].ToList().AddRange(SelecionarInput_Output(configuracaoCaptacao.Dados.Skip((configuracaoCaptacao.Dados.Count / (quantidadeShifts - 1)) * (numShift + 1)).ToList(), tamanhoEntrada));
            //}

            //Inicializa todos os dias da previsao
            for (int diaTreinamento = 0; diaTreinamento < totalDiasPrevisao; diaTreinamento++)
            {
                foreach (RedePrevisaoFinanceira redePrevisao in configuracaoCaptacao.RedesPrevisao)
                {
                    redePrevisao.TaxaMediaAcertoPorDia.Add(diaTreinamento, 0);
                }
            }

            Dictionary<int, TrainingSet> treinamentosPorDia = new Dictionary<int, TrainingSet>();
            for (int dia = 0; dia < totalDiasPrevisao; dia++)
            {
                treinamentosPorDia.Add(dia, new TrainingSet(tamanhoEntrada, configuracaoCaptacao.RedesPrevisao.Count));
            }

            //Roda as redes para cada um dos dados de treinamento selecionados
            foreach (Treinamento treinamento in treinamentos)
            {
                //Guarda um historico dos outputs das redes para tratar as redes que tem output menor do que o tamanho da previsao
                List<List<double>> outputsRedes = new List<List<double>>();
                List<double[]> taxasAcertoRedes = new List<double[]>();
                foreach (RedePrevisaoFinanceira redePrevisao in configuracaoCaptacao.RedesPrevisao)
                {
                    Network redeNeural = redePrevisao.RedeNeuralPrevisaoFinanceiraPorDivisaoCrossValidation[treinamento.DivisaoCrossValidation];
                    //Roda a rede neural, gerando a previsao                             \/ como a rede pode receber um input menor do que os treinamentos, pegamos apenas os ultimos registros do mesmo
                    double[] outputPrevisao = redeNeural.Run(treinamento.Input.Skip(treinamento.Input.Count - redePrevisao.JanelaEntrada).Take(redePrevisao.JanelaEntrada).ToArray());
                    //Alimenta o historico de previsoes de cada rede
                    outputsRedes.Add(outputPrevisao.ToList());
                    double[] taxaAcertoPorDia = new double[totalDiasPrevisao];
                    for (int diasPrevistosRN = 0; diasPrevistosRN < outputPrevisao.Length; diasPrevistosRN++)
                    {
                        //Calcula a taxa de acerto da previsao
                        double ta = Math.Min(outputPrevisao[diasPrevistosRN], treinamento.Output[diasPrevistosRN]) / Math.Max(outputPrevisao[diasPrevistosRN], treinamento.Output[diasPrevistosRN]);
                        taxaAcertoPorDia[diasPrevistosRN] = ta;
                        redePrevisao.TaxaMediaAcertoPorDia[diasPrevistosRN] += ta;
                    }
                    taxasAcertoRedes.Add(taxaAcertoPorDia);
                }

                //Lista com o melhor desultado de cada dia (cada item da lista corresponde a um dia..)
                List<double> melhoresResultadosDia = new List<double>(treinamento.Input);
                //Trata as taxas de acerto que não foram calculadas pois a rede tem output menor do que a quantidade de dias da previsao
                for (int dia = 1; dia < totalDiasPrevisao; dia++)
                {
                    //Recupera o indice da rede que tem a melhor taxa de acerto para o dia anterior
                    int indMelhorRedeParaODiaAnterior = taxasAcertoRedes.IndexOf(taxasAcertoRedes.OrderByDescending(taxasAcertoRede => taxasAcertoRede[dia - 1]).First());
                    //Adiciona o melhor resultado a lista de melhores resultados
                    melhoresResultadosDia.Add(outputsRedes[indMelhorRedeParaODiaAnterior][dia - 1]);
                    for (int indRede = 0; indRede < configuracaoCaptacao.RedesPrevisao.Count; indRede++)
                    {
                        //Verifica se a taxa de acerto do dia para a rede ja foi calculada
                        if (taxasAcertoRedes[indRede][dia] == 0)
                        {
                            //Ultiliza os ultimos melhores dados para fazer uma nova previsao
                            double[] inputRede = melhoresResultadosDia.Skip(melhoresResultadosDia.Count - configuracaoCaptacao.RedesPrevisao[indRede].JanelaEntrada).Take(configuracaoCaptacao.RedesPrevisao[indRede].JanelaEntrada).ToArray();
                            double[] outputRede = configuracaoCaptacao.RedesPrevisao[indRede].RedeNeuralPrevisaoFinanceiraPorDivisaoCrossValidation[treinamento.DivisaoCrossValidation].Run(inputRede);
                            //Adiciona o dia previsto na lista de outputs
                            outputsRedes[indRede].Add(outputRede.Last());
                            //Calcula a taxa de acerto da previsao
                            double ta = Math.Min(outputRede.Last(), treinamento.Output[dia]) / Math.Max(outputRede.Last(), treinamento.Output[dia]);
                            //Adiciona a taxa de acerto da rede para o dia
                            taxasAcertoRedes[indRede][dia] = ta;
                            //Atualiza a taxa de acerto da rede
                            configuracaoCaptacao.RedesPrevisao[indRede].TaxaMediaAcertoPorDia[dia] += ta;
                        }
                    }
                }

                for (int dia = 0; dia < totalDiasPrevisao; dia++)
                {
                    double[] outputCaptacao = new double[configuracaoCaptacao.RedesPrevisao.Count];
                    for (int indRede = 0; indRede < configuracaoCaptacao.RedesPrevisao.Count; indRede++)
                    {
                        outputCaptacao[indRede] = taxasAcertoRedes[indRede][dia];
                    }
                    TrainingSample ts = new TrainingSample(treinamento.Input.ToArray(), outputCaptacao);
                    treinamentosPorDia[dia].Add(ts);
                }
            }

            //Divide a taxa de acerto pela quantidade de treinamentos para saber a taxa média
            foreach (RedePrevisaoFinanceira redePrevisao in configuracaoCaptacao.RedesPrevisao)
            {
                for (int i = 0; i < totalDiasPrevisao; i++)
                {
                    redePrevisao.TaxaMediaAcertoPorDia[i] /= treinamentos.Count;
                }
            }

            return treinamentosPorDia;
        }

        /// <summary>
        /// Quebra os dados da cotação do ativo em vários treinamentos para a rede neural de captação de dados
        /// </summary>
        /// <param name="dados"></param>
        /// <param name="tamanhoInput"></param>
        /// <returns></returns>
        private static Dictionary<List<double>, List<double>> SelecionarInput_Output(List<double> dados, int tamanhoInput)
        {
            Dictionary<List<double>, List<double>> dadosTreinamento = new Dictionary<List<double>, List<double>>();
            for (int i = 0; i < dados.Count - (tamanhoInput + totalDiasPrevisao); i += 3)
            {
                dadosTreinamento.Add(dados.Skip(i).Take(tamanhoInput).ToList(), dados.Skip(i + tamanhoInput).Take(totalDiasPrevisao).ToList());
            }
            return dadosTreinamento;
        }

        /// <summary>
        /// Gera o relatório que diz qual que é a taxa de acerto de cada rede neural para cada um dos dias
        /// </summary>
        /// <param name="redesNeurais"></param>
        private static void GerarRelatorioCrossOver(string nomePlanilha, List<RedePrevisaoFinanceira> redesNeurais)
        {
            /*
             * //Referencias
             * //http://excelpackage.codeplex.com/wikipage?title=Creating%20an%20Excel%20spreadsheet%20from%20scratch&referringTitle=Home
             * 
             * FileInfo newFile = new FileInfo(diretorioRelatorioCrossOver + nomePlanilha + ".xlsx");
            using (ExcelPackage xlPackage = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("RelatorioCrossValidation");
                worksheet.Cell(1, 1).Value = DateTime.Now.ToString();
                //Cabeçalho
                worksheet.Cell(2, 1).Value = "Nome da Rede";
                worksheet.Cell(2, 2).Value = "Janela Entrada";
                worksheet.Cell(2, 3).Value = "Janela Saida";
                worksheet.Cell(2, 4).Value = "Num Neurônios";
                worksheet.Cell(2, 5).Value = "Taxa Aprendizado";
                worksheet.Cell(2, 6).Value = "Ciclos Treinamento";
                for (int colDia = 1; colDia <= totalDiasPrevisao; colDia++)
                {
                    worksheet.Cell(2, colDia + 6).Value = "Dia " + colDia;
                }

                int linha = 3;
                //Linhas da coluna
                foreach (RedePrevisaoFinanceira redePrevisao in redesNeurais)
                {
                    worksheet.Cell(linha, 1).Value = redePrevisao.NomeRede;
                    worksheet.Cell(linha, 2).Value = redePrevisao.JanelaEntrada.ToString();
                    worksheet.Cell(linha, 3).Value = redePrevisao.JanelaSaida.ToString();
                    worksheet.Cell(linha, 4).Value = redePrevisao.NomeRede.Split(new string[] { "_nn", "_ta" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    worksheet.Cell(linha, 5).Value = redePrevisao.NomeRede.Split(new string[] { "_ta", "_ct" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    worksheet.Cell(linha, 6).Value = redePrevisao.NomeRede.Split(new string[] { "_ct", "_v" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    for (int dia = 1; dia <= totalDiasPrevisao; dia++)
                    {
                        worksheet.Cell(linha, 6 + dia).Value = redePrevisao.TaxaMediaAcertoPorDia[dia - 1].ToString();
                    }
                    linha++;
                }
                xlPackage.Save();
            }*/
            //Cria o arquivo do excel
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            excelApp.Workbooks.Add(1);
            Microsoft.Office.Interop.Excel.Workbook arq_de_trab = (Microsoft.Office.Interop.Excel.Workbook)excelApp.Workbooks.Add(1);
            Microsoft.Office.Interop.Excel.Worksheet planilha = (Microsoft.Office.Interop.Excel.Worksheet)excelApp.Worksheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            planilha.Cells[1, 1] = DateTime.Now.ToString();
            //Cabeçalho
            planilha.Cells[2, 1] = "Nome da Rede";
            planilha.Cells[2, 2] = "Janela Entrada";
            planilha.Cells[2, 3] = "Janela Saida";
            planilha.Cells[2, 4] = "Num Neurônios";
            planilha.Cells[2, 5] = "Taxa Aprendizado";
            planilha.Cells[2, 6] = "Ciclos Treinamento";
            for (int colDia = 1; colDia <= totalDiasPrevisao; colDia++)
            {
                planilha.Cells[2, colDia + 6] = "Dia " + colDia;
            }

            int linha = 3;
            //Linhas da coluna
            foreach (RedePrevisaoFinanceira redePrevisao in redesNeurais)
            {
                planilha.Cells[linha, 1] = redePrevisao.NomeRede;
                planilha.Cells[linha, 2] = redePrevisao.JanelaEntrada;
                planilha.Cells[linha, 3] = redePrevisao.JanelaSaida;
                planilha.Cells[linha, 4] = redePrevisao.NomeRede.Split(new string[] { "_nn", "_ta" }, StringSplitOptions.RemoveEmptyEntries)[1];
                planilha.Cells[linha, 5] = redePrevisao.NomeRede.Split(new string[] { "_ta", "_ct" }, StringSplitOptions.RemoveEmptyEntries)[1];
                planilha.Cells[linha, 6] = redePrevisao.NomeRede.Split(new string[] { "_ct", "_v" }, StringSplitOptions.RemoveEmptyEntries)[1];
                for (int dia = 1; dia <= totalDiasPrevisao; dia++)
                {
                    planilha.Cells[linha, 6 + dia] = redePrevisao.TaxaMediaAcertoPorDia[dia - 1];
                }
                linha++;
            }

            arq_de_trab.SaveAs(diretorioRelatorioCrossOver + nomePlanilha, Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel7, Type.Missing,
                Type.Missing, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            arq_de_trab.Close();
        }
    }
}
