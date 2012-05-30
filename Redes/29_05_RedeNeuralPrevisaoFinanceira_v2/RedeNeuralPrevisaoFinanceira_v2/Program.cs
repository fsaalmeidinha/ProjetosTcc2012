using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBaseUtils;

namespace RedeNeuralPrevisaoFinanceira_v2
{
    class Program
    {
        static void Main(string[] args)
        {
            List<DadosBE> dadosBE = DataBaseUtils.DataBaseUtils.RecuperarCotacoesAtivo("PETR4");
        }
    }
}
