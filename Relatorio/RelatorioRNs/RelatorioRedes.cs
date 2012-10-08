using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RelatorioRNs
{
    public class RelatorioRedes
    {
        static DateTime dtInicialPrevisao = new DateTime(2011, 10, 1);
        static int qtdDiasPrever = 30;

        public static void GerarRelatorio()
        {
            List<double[]> previsao;
            //V1
            previsao = PrevisaoFinanceiraHelper.PrevisaoFinanceira.PreverCotacaoAtivo("PETR4", dtInicialPrevisao, qtdDiasPrever);

            //V2
            previsao = RedeNeuralPrevisaoFinanceira_v2.RNAssessor.PreverCotacao(dtInicialPrevisao, qtdDiasPrever);

            //V3.4
            previsao = RedeNeuralPrevisaoFinanceira_v3.RNAssessor.PreverCotacao(dtInicialPrevisao, 3.4, qtdDiasPrever);
            previsoesPorRN.Add("PETR4_RN_V3_4", previsao.Select(prev => prev[1]).ToList());


        }
    }
}
