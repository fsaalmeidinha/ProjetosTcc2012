using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBaseUtils;

namespace MediaPonderada
{
    public class MetodoMediaPonderada
    {
        public static List<double[]> PreverMediasPonderada(string nomeAtivo, DateTime dataInicio, int quantDiasPrevisao, bool previsaoSobrePrevisao = true)
        {
            int quantMedia = 30;
            double peso = 0.2, pesoAux = 1;
            double divisor = 0;

            List<double[]> previsao;
            List<DadosBE> cotacoes = DataBaseUtils.DataBaseUtils.RecuperarCotacoesAtivo(nomeAtivo);

            previsao = cotacoes.Where(r => r.DataGeracao >= dataInicio.AddDays(-quantMedia)).Take(quantDiasPrevisao + quantMedia).Select(r => new double[] { (double)r.PrecoAbertura, (double)r.PrecoAbertura }).ToList();

            for (double i = (1 + peso); i <= (1 + peso * quantMedia); i += peso)
            {
                divisor += i;
            }

            for (int i = quantMedia; i < quantDiasPrevisao + quantMedia; i++)
            {
                if (previsaoSobrePrevisao)
                    previsao[i][1] = previsao.Skip(i - quantMedia).Take(quantMedia).Select(cot => cot[1] * (pesoAux += peso)).Sum() / divisor;
                else
                    previsao[i][1] = previsao.Skip(i - quantMedia).Take(quantMedia).Select(cot => cot[0] * (pesoAux += peso)).Sum() / divisor;
                pesoAux = 1;
            }

            return previsao.Skip(quantMedia).ToList();
        }

        public static List<double[]> PreverMediasPonderadaAleatoria(string nomeAtivo, DateTime dataInicio, DateTime dataTermino)
        {
            int quantMedia = 5;
            double peso = 0.5, pesoAux = 1;
            double divisor = 0;

            int quantDiasPrevisao = (int)dataTermino.Subtract(dataInicio).TotalDays;
            List<double[]> previsao;
            List<DadosBE> cotacoes = DataBaseUtils.DataBaseUtils.RecuperarCotacoesAtivo(nomeAtivo);

            previsao = cotacoes.Where(r => r.DataGeracao >= dataInicio.AddDays(-quantMedia)).Take(quantDiasPrevisao + quantMedia).Select(r => new double[] { (double)r.PrecoAbertura, (double)r.PrecoAbertura }).ToList();

            for (double i = (1 + peso); i <= (1 + peso * quantMedia); i += peso)
            {
                divisor += i;
            }

            for (int i = 0; i < quantMedia; i++)
            {
                previsao[i][1] = previsao[new Random(i).Next(0, (quantDiasPrevisao + quantMedia) - 1)][0];
            }

            for (int i = quantMedia; i < quantDiasPrevisao + quantMedia; i++)
            {
                previsao[i][1] = previsao.Skip(i - quantMedia).Take(quantMedia).Select(cot => cot[1] * (pesoAux += peso)).Sum() / divisor;
                pesoAux = 1;
            }

            return previsao.Skip(quantMedia).ToList();
        }
    }
}