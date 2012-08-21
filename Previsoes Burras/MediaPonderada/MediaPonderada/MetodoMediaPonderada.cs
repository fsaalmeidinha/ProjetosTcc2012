using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBaseUtils;

namespace MediaPonderada
{
    public class MetodoMediaPonderada
    {
        public static List<double[]> PreverMediasPonderada(string nomeAtivo, DateTime dataInicio, DateTime dataTermino)
        {
            int quantMedia = 5;
            double divisor = 0;
            int quantDiasPrevisao = (int)dataTermino.Subtract(dataInicio).TotalDays;
            List<double[]> previsao;
            List<DadosBE> cotacoes = DataBaseUtils.DataBaseUtils.RecuperarCotacoesAtivo(nomeAtivo);

            previsao = cotacoes.Where(r => r.DataGeracao >= dataInicio.AddDays(-quantMedia)).Take(quantDiasPrevisao + quantMedia).Select(r => new double[] { (double)r.PrecoAbertura, (double)r.PrecoAbertura }).ToList();

            for (int i = 1; i <= quantMedia; i++)
            {
                divisor += i;
            }

            for (int i = quantMedia; i < quantDiasPrevisao + quantMedia; i++)
            {
                previsao[i][1] = previsao.Skip(i - quantMedia).Take(quantMedia).Select((cot, ind)=> cot[1] * (ind + 1)).Sum() / divisor;
            }

            return previsao.Skip(quantMedia).ToList();
        }
    }
}