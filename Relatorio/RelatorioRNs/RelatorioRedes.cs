using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RelatorioRNs.Model;

namespace RelatorioRNs
{
    public class RelatorioRedes
    {
        static DateTime dtInicialPrevisao = new DateTime(2011, 10, 1);
        static int qtdDiasPrever = 30;

        public static void GerarRelatorio()
        {
            List<Relatorio> relatorios = new List<Relatorio>();

            List<double[]> previsao;
            //PETR4 - V1
            previsao = PrevisaoFinanceiraHelper.PrevisaoFinanceira.PreverCotacaoAtivo("PETR4", dtInicialPrevisao, qtdDiasPrever);
            Relatorio relatorioPETR4_V1 = RecuperarAnalisesRelatorio("PETR4", "Rede Neural versão 1", true, previsao);
            relatorios.Add(relatorioPETR4_V1);

            //PETR4 - V2
            previsao = RedeNeuralPrevisaoFinanceira_v2.RNAssessor.PreverCotacao(dtInicialPrevisao, qtdDiasPrever);
            Relatorio relatorioPETR4_V2 = RecuperarAnalisesRelatorio("PETR4", "Rede Neural versão 2", true, previsao);
            relatorios.Add(relatorioPETR4_V2);

            //PETR4 - V3.4
            previsao = RedeNeuralPrevisaoFinanceira_v3.RNAssessor.PreverCotacao(dtInicialPrevisao, 3.4, qtdDiasPrever);
            Relatorio relatorioPETR4_V3_4 = RecuperarAnalisesRelatorio("PETR4", "Rede Neural versão 3.4", false, previsao);
            relatorios.Add(relatorioPETR4_V3_4);

            //PETR4 - V4.01
            previsao = RedeNeuralPrevisaoFinanceira_v3.RNAssessor.PreverCotacao(dtInicialPrevisao, 4.01, qtdDiasPrever);
            Relatorio relatorioPETR4_V4_01 = RecuperarAnalisesRelatorio("PETR4", "Rede Neural versão 4.01", false, previsao);
            relatorios.Add(relatorioPETR4_V4_01);

            //PETR4 - V5.01
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.01, qtdDiasPrever);
            Relatorio relatorioPETR4_V5_01 = RecuperarAnalisesRelatorio("PETR4", "Rede Neural versão 5.01", false, previsao);
            relatorios.Add(relatorioPETR4_V5_01);

            //PETR4 - V5.02
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.02, qtdDiasPrever);
            Relatorio relatorioPETR4_V5_02 = RecuperarAnalisesRelatorio("PETR4", "Rede Neural versão 5.02", false, previsao);
            relatorios.Add(relatorioPETR4_V5_02);

            //PETR4 - V5.03
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.03, qtdDiasPrever);
            Relatorio relatorioPETR4_V5_03 = RecuperarAnalisesRelatorio("PETR4", "Rede Neural versão 5.03", false, previsao);
            relatorios.Add(relatorioPETR4_V5_03);

            //PETR4 - V5.04
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.04, qtdDiasPrever);
            Relatorio relatorioPETR4_V5_04 = RecuperarAnalisesRelatorio("PETR4", "Rede Neural versão 5.04", false, previsao);
            relatorios.Add(relatorioPETR4_V5_04);

            //PETR4 - V5.05
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.05, qtdDiasPrever);
            Relatorio relatorioPETR4_V5_05 = RecuperarAnalisesRelatorio("PETR4", "Rede Neural versão 5.05", false, previsao);
            relatorios.Add(relatorioPETR4_V5_05);

            //PETR4 - V5.06
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.06, qtdDiasPrever);
            Relatorio relatorioPETR4_V5_06 = RecuperarAnalisesRelatorio("PETR4", "Rede Neural versão 5.06", false, previsao);
            relatorios.Add(relatorioPETR4_V5_06);

            //PETR4 - V5.07
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.07, qtdDiasPrever);
            Relatorio relatorioPETR4_V5_07 = RecuperarAnalisesRelatorio("PETR4", "Rede Neural versão 5.07", false, previsao);
            relatorios.Add(relatorioPETR4_V5_07);

            //PETR4 - V5.08
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.08, qtdDiasPrever);
            Relatorio relatorioPETR4_V5_08 = RecuperarAnalisesRelatorio("PETR4", "Rede Neural versão 5.08", false, previsao);
            relatorios.Add(relatorioPETR4_V5_08);

            //PETR4 - V5.09
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.09, qtdDiasPrever);
            Relatorio relatorioPETR4_V5_09 = RecuperarAnalisesRelatorio("PETR4", "Rede Neural versão 5.09", false, previsao);
            relatorios.Add(relatorioPETR4_V5_09);

            //PETR4 - V5.10
            previsao = RedeNeuralPrevisaoFinanceira_v5.RNAssessor.PreverCotacao(dtInicialPrevisao, 5.10, qtdDiasPrever);
            Relatorio relatorioPETR4_V5_10 = RecuperarAnalisesRelatorio("PETR4", "Rede Neural versão 5.10", false, previsao);
            relatorios.Add(relatorioPETR4_V5_10);

            //ETER3 - V3.4
            previsao = RedeNeuralPrevisaoFinanceira_v3.RNAssessor.PreverCotacao(dtInicialPrevisao, 3.4, qtdDiasPrever, "ETER3");
            Relatorio relatorioETER3_V3_4 = RecuperarAnalisesRelatorio("ETER3", "Rede Neural versão 3.4", false, previsao);
            relatorios.Add(relatorioETER3_V3_4);

            //GOLL4 - V3.4
            previsao = RedeNeuralPrevisaoFinanceira_v3.RNAssessor.PreverCotacao(dtInicialPrevisao, 3.4, qtdDiasPrever, "GOLL4");
            Relatorio relatorioGOLL4_V3_4 = RecuperarAnalisesRelatorio("GOLL4", "Rede Neural versão 3.4", false, previsao);
            relatorios.Add(relatorioGOLL4_V3_4);

            //NATU3 - V3.4
            previsao = RedeNeuralPrevisaoFinanceira_v3.RNAssessor.PreverCotacao(dtInicialPrevisao, 3.4, qtdDiasPrever, "NATU3");
            Relatorio relatorioNATU3_V3_4 = RecuperarAnalisesRelatorio("NATU3", "Rede Neural versão 3.4", false, previsao);
            relatorios.Add(relatorioNATU3_V3_4);

            //VALE5 - V3.4
            previsao = RedeNeuralPrevisaoFinanceira_v3.RNAssessor.PreverCotacao(dtInicialPrevisao, 3.4, qtdDiasPrever, "VALE5");
            Relatorio relatorioVALE5_V3_4 = RecuperarAnalisesRelatorio("VALE5", "Rede Neural versão 3.4", false, previsao);
            relatorios.Add(relatorioVALE5_V3_4);

            GerarRelatorioExcel(relatorios);
        }

        private static Relatorio RecuperarAnalisesRelatorio(string papel, string nomeRN, bool previsaoSobrePrevisao, List<double[]> previsoes)
        {
            Relatorio relatorio = new Relatorio() { NomeRN = nomeRN, Papel = papel };

            for (int ind = 0; ind < previsoes.Count; ind++)
            {
                Relatorio.RelatorioDia relDia = new Relatorio.RelatorioDia();
                relDia.Real = previsoes[ind][0];
                relDia.Previsto = previsoes[ind][1];
                relDia.Erro = Math.Abs(1 - (relDia.Real / relDia.Previsto));
                relatorio.ErroAcumulado += relDia.Erro;

                if (ind > 0)
                {
                    double valorRealOntem = previsoes[ind - 1][0];

                    double valorComparar = 0;
                    if (previsaoSobrePrevisao)
                        valorComparar = previsoes[ind - 1][1];
                    else
                        valorComparar = valorRealOntem;

                    if ((relDia.Real > valorRealOntem && relDia.Previsto > valorRealOntem)
                     || (relDia.Real < valorRealOntem && relDia.Previsto < valorRealOntem))
                        relDia.AcompanhouTendencia = true;
                }

                relatorio.RelatoriosDia.Add(relDia);
            }
            relatorio.ErroMedio = relatorio.ErroAcumulado / previsoes.Count;

            int qtdAcompanhouTendencia = relatorio.RelatoriosDia.Count(rel => rel.AcompanhouTendencia);
            relatorio.AcompanhouTendencia = String.Format("{0} de {1} ({2}% de acompanhamento de tendêndia)", qtdAcompanhouTendencia, previsoes.Count - 1, qtdAcompanhouTendencia / (double)(previsoes.Count - 1) * 100);

            return relatorio;
        }

        private static void GerarRelatorioExcel(List<Relatorio> relatorios)
        {
            //Cria o arquivo do excel
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            excelApp.Workbooks.Add(1);
            Microsoft.Office.Interop.Excel.Workbook arq_de_trab = (Microsoft.Office.Interop.Excel.Workbook)excelApp.Workbooks.Add(1);
            Microsoft.Office.Interop.Excel.Worksheet planilha = (Microsoft.Office.Interop.Excel.Worksheet)excelApp.Worksheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            int linha = 1;
            foreach (Relatorio relatorio in relatorios)
            {
                planilha.Cells[linha++, 1] = String.Format("{0} ({1})", relatorio.Papel, relatorio.NomeRN, relatorio.AcompanhouTendencia);
                planilha.Cells[linha++, 1] = String.Format("Tendencias:{0}", relatorio.AcompanhouTendencia);
                planilha.Cells[linha++, 1] = String.Format("Erro Médio:{0}%", relatorio.ErroMedio);
                planilha.Cells[linha, 1] = "Real";
                for (int indCol = 0; indCol < relatorio.RelatoriosDia.Count; indCol++)
                {
                    planilha.Cells[linha, indCol + 2] = relatorio.RelatoriosDia[indCol].Real;
                }

                linha++;
                planilha.Cells[linha, 1] = "Previsto";
                for (int indCol = 0; indCol < relatorio.RelatoriosDia.Count; indCol++)
                {
                    planilha.Cells[linha, indCol + 2] = relatorio.RelatoriosDia[indCol].Previsto;
                }

                linha++;
                planilha.Cells[linha, 1] = "Erro";
                for (int indCol = 0; indCol < relatorio.RelatoriosDia.Count; indCol++)
                {
                    planilha.Cells[linha, indCol + 2] = relatorio.RelatoriosDia[indCol].Erro;
                }
                linha += 3;
            }

            arq_de_trab.SaveAs("C:\\Users\\Felipe\\Desktop\\tcc\\Relatorio\\RelatorioRNs\\RelatoriosExcel\\RelatorioRedes.xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel7, Type.Missing,
                Type.Missing, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            arq_de_trab.Close();
        }
    }
}
