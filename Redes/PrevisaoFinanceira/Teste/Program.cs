using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrevisaoFinanceiraHelper;

namespace Teste
{
    class Program
    {
        static void Main(string[] args)
        {
            List<double[]> previsao = PrevisaoFinanceira.PreverCotacaoAtivo("PETR4", new DateTime(2011, 10, 1), 90);

        }
    }
}
