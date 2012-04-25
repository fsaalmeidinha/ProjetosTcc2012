using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedeNeural_PrevisaoFinanceira
{
    public abstract class Utils
    {
        static double multiplicadorNormalizacao = 2;

        /// <summary>
        /// Normaliza uma lista de dados
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="dados"></param>
        public static List<double> NormalizarDados(List<double> dados)
        {
            double min = dados.Min();
            double max = dados.Max();
            max *= multiplicadorNormalizacao;
            min /= multiplicadorNormalizacao;

            List<double> dadosList = new List<double>();
            dados.ForEach(dado => dadosList.Add((dado - min) / (max - min)));
            return dadosList;
        }

        /// <summary>
        /// Desnormaliza uma lista de dados
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="dados"></param>
        public static List<double> DesnormalizarDados(double min, double max, List<double> dados)
        {
            max *= multiplicadorNormalizacao;
            min /= multiplicadorNormalizacao;

            List<double> dadosList = new List<double>();
            dados.ForEach(dado => dadosList.Add(dado * (max - min) + min));
            return dadosList;
        }

        /// <summary>
        /// Retorna uma lista de KeyValuePair, onde a chave são os valores de input e o seu valor correspondente são os valores de output
        /// </summary>
        /// <param name="dados"></param>
        /// <param name="janelaEntrada">tamanho do input</param>
        /// <param name="janelaSaida">tamanho do output</param>
        /// <param name="considerarSaidasComoEntradas">se verdadeiro, teremos 'dados.Count / janelaEntrada' registros como saida, caso contrario, 'dados.Count / (janelaEntrada + janelaSaida)' </param>
        /// <returns></returns>
        public static List<KeyValuePair<double[], double[]>> SelecionarCotacoesPorJanelamento(List<double> dados, int janelaEntrada, int janelaSaida, bool considerarSaidasComoEntradas)
        {
            /*Cria um mapeamento de entradas para saida com o janelamento informado*/
            List<KeyValuePair<double[], double[]>> dadosPorJanelamento = new List<KeyValuePair<double[], double[]>>();
            for (int i = 0; i < dados.Count - (janelaEntrada + janelaSaida); i += janelaEntrada + (considerarSaidasComoEntradas ? 0 : janelaSaida))
            {
                dadosPorJanelamento.Add(new KeyValuePair<double[], double[]>(dados.Skip(i).Take(janelaEntrada).ToArray(), dados.Skip(i + janelaEntrada).Take(janelaSaida).ToArray()));
            }
            /*Cria um mapeamento de entradas para saida com o janelamento informado*/

            return dadosPorJanelamento;
        }

    }
}
