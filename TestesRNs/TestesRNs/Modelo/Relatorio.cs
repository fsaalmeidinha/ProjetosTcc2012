using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using NeuronDotNet.Core;
using TestesRNs.RedeNeural;
using System.Configuration;

namespace TestesRNs.Modelo
{
    public class Relatorio
    {
        private static string diretorioRedes
        {
            get
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["DiretorioRedes"]))
                    return ConfigurationManager.AppSettings["DiretorioRedes"];
                else
                    return System.IO.Directory.GetCurrentDirectory();
            }
        }

        public Relatorio(string papel, List<Versao> versoes, int tamanhoTendencia, DateTime dtInicial, DateTime dtFinal, int qtdTotalDados)
        {
            this.Papel = papel;
            this.Versoes = versoes;
            this.Indices = new List<string>();
            foreach (Versao versao in versoes)
            {
                this.Indices.Add(((DescriptionAttribute)typeof(Versao).GetMember(versao.ToString())[0].GetCustomAttributes(typeof(DescriptionAttribute), false).First()).Description);
            }

            this.DataInicial = dtInicial;
            this.DataFinal = dtFinal;
            this.TotalDados = qtdTotalDados;
            this.TamanhoTendencia = tamanhoTendencia;
        }
        #region PROPRIEDADES
        public string Papel { get; private set; }
        public List<string> Indices { get; private set; }
        public List<Versao> Versoes { get; private set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int TotalDados { get; set; }
        public int TamanhoTendencia { get; set; }

        public int TendenciasAlta { get; set; }
        public int AcertosTendenciasAlta { get; set; }
        public double PercentualAcertoTendenciasAlta { get { return ((double)this.AcertosTendenciasAlta) / this.TendenciasAlta * 100; } }

        public int TendenciasBaixa { get; set; }
        public int AcertosTendenciasBaixa { get; set; }
        public double PercentualAcertoTendenciasBaixa { get { return ((double)this.AcertosTendenciasBaixa) / this.TendenciasBaixa * 100; } }

        public int TendenciasGerais { get { return this.TendenciasAlta + this.TendenciasBaixa; } }
        public int AcertosTendenciasGerais { get { return this.AcertosTendenciasAlta + this.AcertosTendenciasBaixa; } }
        public double PercentualAcertoTendenciasGerais { get { return ((double)(this.PercentualAcertoTendenciasAlta + this.PercentualAcertoTendenciasBaixa)) / 2; } }

        private double _acertoTreinamento = 0;
        public double PercentualAcertoTreinamento { get { return ((double)this._acertoTreinamento / this.numeroInsercoesTestes); } }

        private int numeroInsercoesTestes = 0;
        #endregion PROPRIEDADES

        public void AdicionarAoRelatorio(List<Treinamento> testes, double erroTreinamento, int tamanhoTendencia)
        {
            numeroInsercoesTestes++;
            this._acertoTreinamento += erroTreinamento;
            PreencherErroTendencias(testes, tamanhoTendencia);
        }
        private void PreencherErroTendencias(List<Treinamento> testes, int tamanhoTendencia)
        {
            Network redeNeural = RNHelper.RecuperarRedeNeural(this.Papel, this.Versoes, tamanhoTendencia);
            int numeroAcertos = 0;
            foreach (Treinamento treinamento in testes)
            {
                double[] previsao = redeNeural.Run(treinamento.Input.ToArray());
                bool tendenciaAlta = previsao[0] > previsao[1];
                if (tendenciaAlta)
                    this.TendenciasAlta++;
                else
                    this.TendenciasBaixa++;

                if (RNCore.ValoresMaximosNoMesmoIndice(previsao, treinamento.Output.ToArray()))
                {
                    numeroAcertos++;
                    if (tendenciaAlta)
                        this.AcertosTendenciasAlta++;
                    else
                        this.AcertosTendenciasBaixa++;
                }
            }
        }

        public static void GerarRelatorioExcel(List<Relatorio> relatorios)
        {
            //Cria o arquivo do excel
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            excelApp.Workbooks.Add(1);
            Microsoft.Office.Interop.Excel.Workbook arq_de_trab = (Microsoft.Office.Interop.Excel.Workbook)excelApp.Workbooks.Add(1);
            Microsoft.Office.Interop.Excel.Worksheet planilha = (Microsoft.Office.Interop.Excel.Worksheet)excelApp.Worksheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            planilha.Cells[1, 1] = "Data Geração:";
            planilha.Cells[1, 2] = DateTime.Now.ToShortDateString();

            planilha.Cells[3, 1] = "PAT(%)";
            planilha.Cells[3, 2] = "Percentual Acerto Treinamento(dados de teste são os mesmos do treinamento)";
            planilha.Cells[4, 1] = "PATG(%)";
            planilha.Cells[4, 2] = "Percentual Acerto Tendencias Gerais";
            planilha.Cells[5, 1] = "PATA(%)";
            planilha.Cells[5, 2] = "Percentual Acerto Tendencias Alta";
            planilha.Cells[6, 1] = "PATB(%)";
            planilha.Cells[6, 2] = "Percentual Acerto Tendencias Baixa";
            planilha.Cells[7, 1] = "TG";
            planilha.Cells[7, 2] = "Tendencias Gerais";
            planilha.Cells[8, 1] = "ATG";
            planilha.Cells[8, 2] = "Acertos Tendencias Gerais";
            planilha.Cells[9, 1] = "TA";
            planilha.Cells[9, 2] = "Tendencias Alta";
            planilha.Cells[10, 1] = "ATA";
            planilha.Cells[10, 2] = "Acertos Tendencias Alta";
            planilha.Cells[11, 1] = "TB";
            planilha.Cells[11, 2] = "Tendencias Baixa";
            planilha.Cells[12, 1] = "ATB";
            planilha.Cells[12, 2] = "Acertos Tendencias Baixa";

            int linha = 14;
            foreach (IGrouping<string, Relatorio> relatorioPorPapel in relatorios.GroupBy(rel => rel.Papel))
            {
                planilha.Cells[linha, 3] = String.Format("Papel: {0} ({1} - {2})- {3} dados", relatorioPorPapel.Key, relatorioPorPapel.First().DataInicial.ToShortDateString(), relatorioPorPapel.First().DataFinal.ToShortDateString(), relatorioPorPapel.First().TotalDados);
                linha++;

                planilha.Cells[linha, 3] = "Índices";
                planilha.Cells[linha, 4] = "PAT(%)";//Percentual Acerto Treinamento
                planilha.Cells[linha, 5] = "PATG(%)";//Percentual Acerto Tendencias Gerais
                planilha.Cells[linha, 6] = "PATA(%)";//Percentual Acerto Tendencias Alta
                planilha.Cells[linha, 7] = "PATB(%)";//Percentual Acerto Tendencias Baixa
                planilha.Cells[linha, 8] = "TG";//Tendencias Gerais
                planilha.Cells[linha, 9] = "ATG";//Acertos Tendencias Gerais
                planilha.Cells[linha, 10] = "TA";//Tendencias Alta
                planilha.Cells[linha, 11] = "ATA";//Acertos Tendencias Alta
                planilha.Cells[linha, 12] = "TB";//Tendencias Baixa
                planilha.Cells[linha, 13] = "ATB";//Acertos Tendencias Baixa
                linha++;
                foreach (IGrouping<string, Relatorio> relatorioPorPapelEIndices in relatorioPorPapel.GroupBy(rel => String.Join("; ", rel.Indices)))
                {
                    foreach (Relatorio relatorio in relatorioPorPapelEIndices.OrderByDescending(rel => rel.PercentualAcertoTendenciasGerais))
                    {
                        planilha.Cells[linha, 3] = "Tam.Tend:" + relatorio.TamanhoTendencia + "-" + String.Join("; ", relatorio.Indices);
                        planilha.Cells[linha, 4] = relatorio.PercentualAcertoTreinamento.ToString("N3");
                        planilha.Cells[linha, 5] = relatorio.PercentualAcertoTendenciasGerais.ToString("N3");
                        planilha.Cells[linha, 6] = relatorio.PercentualAcertoTendenciasAlta.ToString("N3");
                        planilha.Cells[linha, 7] = relatorio.PercentualAcertoTendenciasBaixa.ToString("N3");
                        planilha.Cells[linha, 8] = relatorio.TendenciasGerais;
                        planilha.Cells[linha, 9] = relatorio.AcertosTendenciasGerais;
                        planilha.Cells[linha, 10] = relatorio.TendenciasAlta;
                        planilha.Cells[linha, 11] = relatorio.AcertosTendenciasAlta;
                        planilha.Cells[linha, 12] = relatorio.TendenciasBaixa;
                        planilha.Cells[linha, 13] = relatorio.AcertosTendenciasBaixa;
                        linha++;
                    }
                }
                linha += 2;
            }

            arq_de_trab.SaveAs(diretorioRedes + "Relatorio.xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel7, Type.Missing,
                Type.Missing, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            arq_de_trab.Close();
        }
    }
}
