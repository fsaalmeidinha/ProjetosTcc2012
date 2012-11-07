using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnaliseTecnica.Modelo;

namespace AnaliseTecnica.RN
{
    public class AnaliseAtivo
    {
        public static List<DadoBE> AtivosComprar(string papel, out DateTime dtInicial, out DateTime dtFinal, out int totalDados)
        {
            List<DadoBE> dadosBE = DadoBE.PegarTodos(papel);
            dtInicial = dadosBE.First().DataGeracao;
            dtFinal = dadosBE.Last().PegarNAntes(5).DataGeracao;
            totalDados = dadosBE.Count - 5;

            return dadosBE.Take(dadosBE.Count - 5).Where(dado => dado.ValorBollinger[0] == 1).ToList();
        }
    }
}
