using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBaseUtils;

namespace AlisamentoExponencialSimples
{
    public class MetodoAlisamentoExpSimples
    {
        public static List<double[]> PreverAlisamentoExponencialSimples(string nomeAtivo, DateTime dataInicio, int quantDiasPrevisao, bool previsaoSobrePrevisao = true, double coeficiente = 0.9)
        {
            double previsao, alfa, valorReal, previsaoAnterior;

            int quant = 2;

            alfa = coeficiente;

            List<double[]> previsoes;
            List<DadosBE> cotacoes = DataBaseUtils.DataBaseUtils.RecuperarCotacoesAtivo(nomeAtivo);

            previsoes = cotacoes.Where(r => r.DataGeracao >= dataInicio.AddDays(-quant)).Take(quantDiasPrevisao + quant).Select(r => new double[] { (double)r.PrecoAbertura, (double)r.PrecoAbertura }).ToList();

            for (int i = quant; i < quantDiasPrevisao + quant; i++)
            {
                valorReal = previsoes[i - 1][1];

                if (previsaoSobrePrevisao)
                    previsaoAnterior = previsoes[i - 2][1];
                else
                    previsaoAnterior = previsoes[i - 2][0];

                previsao = alfa * valorReal + (1 - alfa) * previsaoAnterior;

                previsoes[i][1] = previsao;
            }

            return previsoes.Skip(quant).ToList();
        }
    }
}
