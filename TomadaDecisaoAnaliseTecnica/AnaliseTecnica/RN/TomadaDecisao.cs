using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnaliseTecnica.Modelo;

namespace AnaliseTecnica.RN
{
    public class TomadaDecisao
    {
        public static List<ResultadoTomadaDecisao> TomarDecisoes(DateTime dataInicial, DateTime dataFinal)
        {
            string[] papeis = new string[] { "VALE5", "PETR4", "NATU3", "ETER3", "GOLL4", "BVSP" };
            List<ResultadoTomadaDecisao> resultados = new List<ResultadoTomadaDecisao>();
            foreach (string papel in papeis)
            {
                DateTime dtInicial;
                DateTime dtFinal;
                int totalDados;
                List<DadoBE> compras = AnaliseAtivo.AtivosComprar(papel, out dtInicial, out dtFinal, out totalDados);
                ResultadoTomadaDecisao resultado = new ResultadoTomadaDecisao() { Papel = papel, DataInicial = dtInicial, DataFinal = dtFinal, TotalDados = totalDados };
                resultado.TotalNegociacoesDeCompra = compras.Count;

                foreach (DadoBE dadoBEComprar in compras)
                {
                    DadoBE dadoBEVender = dadoBEComprar.PegarNApos(5);

                    double variacaoEmReais = Math.Abs(dadoBEComprar.PrecoFechamento - dadoBEVender.PrecoFechamento);
                    double percentualVariacao_G_P = variacaoEmReais / dadoBEComprar.PrecoFechamento;
                    if (dadoBEComprar.PrecoFechamento < dadoBEVender.PrecoFechamento)
                    {
                        resultado.PercentualGanhoPerda += percentualVariacao_G_P;
                        resultado.TotalAcertos++;
                    }
                    else
                        resultado.PercentualGanhoPerda -= percentualVariacao_G_P;
                }

                resultado.PercentualGanhoPerda *= 100;
                resultado.PercentualAcerto = 100.00 / resultado.TotalNegociacoesDeCompra * resultado.TotalAcertos;
                resultado.PercentualMedioGanhoPerda = resultado.PercentualGanhoPerda / resultado.TotalNegociacoesDeCompra;

                resultados.Add(resultado);
            }

            return resultados;
        }
    }
}
