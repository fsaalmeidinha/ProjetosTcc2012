using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnaliseTecnica.Modelo;

namespace AnaliseTecnica.RN
{
    public class AnaliseAtivo
    {
        public static List<DadoBE> AtivosComprar(string papel, int tamanhoTendencia, out DateTime dtInicial, out DateTime dtFinal, out int totalDados)
        {
            List<DadoBE> dadosBE = DadoBE.PegarTodos(papel);
            dtInicial = dadosBE.First().DataGeracao;
            dtFinal = dadosBE.Last().PegarNAntes(tamanhoTendencia).DataGeracao;
            totalDados = dadosBE.Count - tamanhoTendencia;

            return dadosBE.Take(dadosBE.Count - tamanhoTendencia).Where(dado => dado.ValorBollinger[0] == 1).ToList();
        }
    }
}
