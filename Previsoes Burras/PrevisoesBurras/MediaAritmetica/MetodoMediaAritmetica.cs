using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBaseUtils;

namespace MediaAritmetica
{
    public class MetodoMediaAritmetica
    {
        public static List<double[]> PreverMediasAritmetica(string nomeAtivo, DateTime dataInicio, int qtdDiasPrever)
        {
            int quantMedia = 5;

            List<double[]> previsoes;
            List<DadosBE> cotacoes = DataBaseUtils.DataBaseUtils.RecuperarCotacoesAtivo(nomeAtivo);
            //Dados para o calculo das previsoes (quantMedia antes da previsao)
            previsoes = DataBaseUtils.DataBaseUtils.SelecionarUltimosNDadosAntesDaDataDaPrevisao(cotacoes, dataInicio, quantMedia)
                .Select(cot => new double[] { (double)cot.PrecoAbertura, (double)cot.PrecoAbertura }).ToList();
            //Adiciona os dados das previsoes(qtdDiasPrever)
            previsoes.AddRange(cotacoes.Where(cot => cot.DataGeracao >= dataInicio).Take(qtdDiasPrever).Select(cot => new double[] { (double)cot.PrecoAbertura, (double)cot.PrecoAbertura }));

            //previsoes = cotacoes.Where(cot => cot.DataGeracao >= dataInicio.AddDays(quantMedia)).Take(qtdDiasPrever + quantMedia).Select(cot => new double[] { (double)cot.PrecoAbertura, (double)cot.PrecoAbertura }).ToList();

            for (int i = quantMedia; i < qtdDiasPrever + quantMedia; i++)
            {
                previsoes[i][1] = (previsoes.Skip(i - quantMedia).Take(quantMedia).Select(cot => cot[1]).Sum()) / quantMedia;
            }

            return previsoes.Skip(quantMedia).ToList();
        }

        public static List<double[]> PreverMediasAritmeticaAleatoria(string nomeAtivo, DateTime dataInicio, DateTime dataTermino)
        {
            int quantMedia = 5;
            int quantDiasPrevisao = (int)dataTermino.Subtract(dataInicio).TotalDays;

            List<double[]> previsoes;
            List<DadosBE> cotacoes = DataBaseUtils.DataBaseUtils.RecuperarCotacoesAtivo(nomeAtivo);

            previsoes = cotacoes.Where(cot => cot.DataGeracao >= dataInicio.AddDays(quantMedia)).Take(quantDiasPrevisao + quantMedia).Select(cot => new double[] { (double)cot.PrecoAbertura, (double)cot.PrecoAbertura }).ToList();

            for (int i = 0; i < quantMedia; i++)
            {
                previsoes[i][1] = previsoes[new Random(i).Next(0, (quantDiasPrevisao + quantMedia) - 1)][0];
            }

            for (int i = quantMedia; i < quantDiasPrevisao + quantMedia; i++)
            {
                previsoes[i][1] = (previsoes.Skip(i - quantMedia).Take(quantMedia).Select(cot => cot[1]).Sum()) / quantMedia;
            }

            return previsoes.Skip(quantMedia).ToList();
        }
    }
}
