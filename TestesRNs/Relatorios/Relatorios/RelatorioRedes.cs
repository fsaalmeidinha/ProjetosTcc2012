using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Relatorios.Model;
using TestesRNs.Modelo;
using TestesRNs.RedeNeural;

namespace Relatorios
{
    public class RelatorioRedes
    {
        static int qtdDiasRelatorio = 55;
        static DateTime dataInicialPrevisao = new DateTime(2012, 1, 09);
        //{
        //    get
        //    {
        //        if (_dataInicialPrevisao == null)
        //        {
        //            List<DadoBE> dadosBE = DadoBE.PegarTodos("PETR4");
        //            DateTime dataIni = dadosBE.Last(dado => dado.DataGeracao < new DateTime(2012, 1, 11)).DataGeracao;
        //        }
        //        return _dataInicialPrevisao.GetValueOrDefault();
        //    }
        //}

        public static Relatorio PegarRelatorioPorPapelEVersao(string nomeRede)
        {
            string papel;
            Versao versao;
            List<double[]> previsoes = RNHelper.PreverAtivo(dataInicialPrevisao, qtdDiasRelatorio + 1, nomeRede, out papel, out versao);
            return GerarRelatorioRN(previsoes, papel, versao);
        }
        public static Relatorio PegarRelatorioPorPapelEVersao(string papel, Versao versao)
        {
            List<double[]> previsoes = RNHelper.PreverAtivo(dataInicialPrevisao, qtdDiasRelatorio + 1, papel, versao);
            return GerarRelatorioRN(previsoes, papel, versao);
        }
        private static Relatorio GerarRelatorioRN(List<double[]> previsoes, string papel, Versao versao)
        {
            Relatorio relatorio = new Relatorio()
            {
                Papel = papel,
                Versao = versao
            };
            for (int indPrevisao = 1; indPrevisao < previsoes.Count; indPrevisao++)
            {
                double erro = Math.Abs(previsoes[indPrevisao][0] - previsoes[indPrevisao][1]) / previsoes[indPrevisao][1];

                relatorio.ErroMedio += erro / qtdDiasRelatorio;
                Relatorio.RelatorioDia relatorioDia = new Relatorio.RelatorioDia()
                {
                    Real = previsoes[indPrevisao][0],
                    Previsto = previsoes[indPrevisao][1],
                    Erro = erro
                };

                if ((relatorioDia.Real > previsoes[indPrevisao - 1][0] && relatorioDia.Previsto > previsoes[indPrevisao - 1][0])
                     || (relatorioDia.Real < previsoes[indPrevisao - 1][0] && relatorioDia.Previsto < previsoes[indPrevisao - 1][0]))
                    relatorioDia.AcompanhouTendencia = true;

                relatorio.RelatoriosDia.Add(relatorioDia);
            }
            int qtdAcompanhouTendencia = relatorio.RelatoriosDia.Count(rel => rel.AcompanhouTendencia);
            relatorio.AcompanhouTendencia = String.Format("{0} de {1} ({2}% de acompanhamento de tendêndia)", qtdAcompanhouTendencia, previsoes.Count - 1, qtdAcompanhouTendencia / (double)(previsoes.Count - 1) * 100);

            return relatorio;
        }

        public static List<Relatorio> PegarTodosRelatorios()
        {
            List<Relatorio> relatorios = new List<Relatorio>();
            List<string> nomesRedes = RNHelper.PegarTodosNomesRedes();
            foreach (string nomeRN in nomesRedes)
            {
                relatorios.Add(PegarRelatorioPorPapelEVersao(nomeRN));
            }
            return relatorios;
        }
    }
}
