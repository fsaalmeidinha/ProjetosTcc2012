using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBaseUtils;
using NeuronDotNet.Core;

namespace PrevisaoFinanceiraHelper
{
    public class PrevisaoFinanceira
    {
        public static List<double[]> PreverCotacaoAtivo(string papel, DateTime dtInicial, int qtdDias)
        {
            List<double[]> resultado = new List<double[]>();
            //Dados conhecidos necessários
            int dcn = 60;
            List<DadosBE> dadosBE = DataBaseUtils.DataBaseUtils.RecuperarCotacoesAtivo(papel);

            //DadoBE com os dados do dia anterior a previsao
            DadosBE dadoBEAnteriorPrimeiraPrevisao = dadosBE.Last(dado => dado.DataGeracao < dtInicial);
            int indDiaAnteriorPrimeiraPrevisao = dadosBE.IndexOf(dadoBEAnteriorPrimeiraPrevisao) + 1;//+1 pois este tb deve ser eliminado da lista

            //Seleciona apenas os dados necessários
            dadosBE = dadosBE.Skip(indDiaAnteriorPrimeiraPrevisao - dcn).Take(dcn + qtdDias).ToList();

            for (int indDiaInicioPrevisao = 0; indDiaInicioPrevisao < qtdDias; indDiaInicioPrevisao += 30)//30 é o numero de previsoes das redes
            {
                List<double> dadosNormalizados = new List<double>();

                //Quantidade de dados conhecidos necessarios para realizar a previsao
                int qtdDadosConhecidosNecessarios = dcn - resultado.Count;
                if (qtdDadosConhecidosNecessarios > 0)
                    dadosNormalizados = DataBaseUtils.DataBaseUtils.NormalizarDados(dadosBE.Skip(dadosBE.Count - dcn - (qtdDias - indDiaInicioPrevisao)).Take(qtdDadosConhecidosNecessarios).Select(dado => (double)dado.PrecoAbertura).ToList(), papel);

                //Pega os ultimos dcn itens, ou os n ultimos itens existentes (onde n é menor ou igual a dcn)
                dadosNormalizados.AddRange(resultado.Select(res => res[1]).Reverse().Take(dcn).Reverse().ToList());

                Dictionary<int, string> redePorDia = CaptacaoMelhoresRedes.RNAssessor.SelecionarRedePorDia(papel, dadosNormalizados);
                //for (int i = 0; i < 30; i++)
                //{
                //    redePorDia[i] = redePorDia[15];
                //}
                double[] output = new double[30];

                //Pega as redes em que o dia que irá prever seja menor ou igual ao tamanho do seu output, pois as demais terao de ser feitar previsoes em cima de previsoes anteriores
                foreach (KeyValuePair<int, string> redeDoDia in redePorDia.Where(rd => rd.Key + 1 <= Convert.ToInt32(rd.Value.Split(new string[] { "js" }, StringSplitOptions.RemoveEmptyEntries).Last().Split('_').First())))
                {
                    //Pega a ultima rede de previsao, para a divisao de cross validation = 7, pois é a unica que nao conhece os ultimos dados que queremos prever
                    Network redePrevisao = RedeNeural_PrevisaoFinanceira.RNAssessor.RecuperarRedesNeuraisAgrupadasPorConfiguracao(redeDoDia.Value).Last().Value;
                    output[redeDoDia.Key] = redePrevisao.Run(dadosNormalizados.Skip(dadosNormalizados.Count - redePrevisao.InputLayer.NeuronCount).ToArray())[redeDoDia.Key];
                }

                //Realizar as previsoes em cima de previsao
                foreach (KeyValuePair<int, string> redeDoDia in redePorDia.Where(rd => rd.Key + 1 > Convert.ToInt32(rd.Value.Split(new string[] { "js" }, StringSplitOptions.RemoveEmptyEntries).Last().Split('_').First())))
                {
                    //Pega a ultima rede de previsao, para a divisao de cross validation = 7, pois é a unica que nao conhece os ultimos dados que queremos prever
                    Network redePrevisao = RedeNeural_PrevisaoFinanceira.RNAssessor.RecuperarRedesNeuraisAgrupadasPorConfiguracao(redeDoDia.Value).Last().Value;
                    List<double> inputRN = new List<double>();
                    //Adiciona os dados normalizados previamente conhecidos (apenas se os dados do output não forem suficientes)
                    if (redePrevisao.InputLayer.NeuronCount - redeDoDia.Key > 0)
                        inputRN.AddRange(dadosNormalizados.Skip((dadosNormalizados.Count - redePrevisao.InputLayer.NeuronCount) + redeDoDia.Key));

                    int skipDadosPreviamentePrevistos = redeDoDia.Key - redePrevisao.InputLayer.NeuronCount;
                    if (skipDadosPreviamentePrevistos < 0)
                        skipDadosPreviamentePrevistos = 0;

                    //Adiciona os dados que foram previamente previstos pelas redes no foreach de acima
                    inputRN.AddRange(output.Skip(skipDadosPreviamentePrevistos).Take(redePrevisao.InputLayer.NeuronCount - inputRN.Count));
                    output[redeDoDia.Key] = redePrevisao.Run(inputRN.ToArray())[0];
                }

                //Adiciona os resultados das previsoes e seus valores reais
                for (int i = 0; i < 30; i++)
                {
                    resultado.Add(new double[] { (double)(dadosBE[dadosBE.Count - (qtdDias - indDiaInicioPrevisao) + i].PrecoAbertura), output[i] });
                }
            }

            List<double> outputDesnormalizado = DataBaseUtils.DataBaseUtils.DesnormalizarDados(resultado.Select(res => res[1]).ToList(), papel);
            for (int i = 0; i < qtdDias; i++)
            {
                resultado[i][1] = outputDesnormalizado[i];
            }
            return resultado;
        }

        //public static List<KeyValuePair<double, double>> PreverCotacaoAtivo(string papel)
        //{
        //    List<DadosBE> dadosBE = DataBaseUtils.DataBaseUtils.RecuperarCotacoesAtivo(papel);
        //    List<double> dadosNormalizados = DataBaseUtils.DataBaseUtils.NormalizarDados(dadosBE.Skip(dadosBE.Count - 90).Take(60).Select(dado => (double)dado.PrecoAbertura).ToList(), papel);
        //    List<string> redes = RedeNeural_PrevisaoFinanceira.RNAssessor.ListarRedes(papel);
        //    Dictionary<int, string> redePorDia = CaptacaoMelhoresRedes.RNAssessor.SelecionarRedePorDia(papel, dadosNormalizados.Take(60).ToList());

        //    double[] output = new double[30];
        //    //Pega as redes em que o dia que irá prever seja menor ou igual ao tamanho do seu output, pois as demais terao de ser feitar previsoes em cima de previsoes anteriores
        //    foreach (KeyValuePair<int, string> redeDoDia in redePorDia.Where(rd => rd.Key + 1 <= Convert.ToInt32(rd.Value.Split(new string[] { "js" }, StringSplitOptions.RemoveEmptyEntries).Last().Split('_').First())))
        //    {
        //        //Pega a ultima rede de previsao, para a divisao de cross validation = 7, pois é a unica que nao conhece os ultimos dados que queremos prever
        //        Network redePrevisao = RedeNeural_PrevisaoFinanceira.RNAssessor.RecuperarRedesNeuraisAgrupadasPorConfiguracao(redeDoDia.Value).Last().Value;
        //        output[redeDoDia.Key] = redePrevisao.Run(dadosNormalizados.Skip(dadosNormalizados.Count - redePrevisao.InputLayer.NeuronCount).ToArray())[redeDoDia.Key];
        //    }

        //    //Realizar as previsoes em cima de previsao
        //    foreach (KeyValuePair<int, string> redeDoDia in redePorDia.Where(rd => rd.Key + 1 > Convert.ToInt32(rd.Value.Split(new string[] { "js" }, StringSplitOptions.RemoveEmptyEntries).Last().Split('_').First())))
        //    {
        //        //Pega a ultima rede de previsao, para a divisao de cross validation = 7, pois é a unica que nao conhece os ultimos dados que queremos prever
        //        Network redePrevisao = RedeNeural_PrevisaoFinanceira.RNAssessor.RecuperarRedesNeuraisAgrupadasPorConfiguracao(redeDoDia.Value).Last().Value;
        //        List<double> inputRN = new List<double>();
        //        //Adiciona os dados normalizados previamente conhecidos (apenas se os dados do output não forem suficientes)
        //        if (redePrevisao.InputLayer.NeuronCount - (redeDoDia.Key + 1) > 0)
        //            inputRN.AddRange(dadosNormalizados.Skip((dadosNormalizados.Count - redePrevisao.InputLayer.NeuronCount) + redeDoDia.Key));

        //        int skipDadosPreviamentePrevistos = redeDoDia.Key - redePrevisao.InputLayer.NeuronCount;
        //        if (skipDadosPreviamentePrevistos < 0)
        //            skipDadosPreviamentePrevistos = 0;

        //        //Adiciona os dados que foram previamente previstos pelas redes no foreach de acima
        //        inputRN.AddRange(output.Skip(skipDadosPreviamentePrevistos).Take(redePrevisao.InputLayer.NeuronCount - inputRN.Count));
        //        output[redeDoDia.Key] = redePrevisao.Run(inputRN.ToArray())[0];
        //    }

        //    List<double> outputDesnormalizado = DataBaseUtils.DataBaseUtils.DesnormalizarDados(output.ToList(), papel);
        //    List<KeyValuePair<double, double>> resultado = new List<KeyValuePair<double, double>>();
        //    for (int i = 30; i > 0; i--)
        //    {
        //        resultado.Add(new KeyValuePair<double, double>((double)dadosBE[dadosBE.Count - i - 1].PrecoAbertura, outputDesnormalizado[30 - i]));
        //    }
        //    return resultado;
        //}
    }
}
