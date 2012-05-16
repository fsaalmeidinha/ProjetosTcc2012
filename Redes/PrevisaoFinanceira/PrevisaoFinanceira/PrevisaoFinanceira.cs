using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBaseUtils;

namespace PrevisaoFinanceiraHelper
{
    public class PrevisaoFinanceira
    {
        public List<double> PreverCotacaoAtivo(string papel)
        {
            List<DadosBE> dadosBE = DataBaseUtils.DataBaseUtils.RecuperarCotacoesAtivo(papel);

            return null;

            //List<string> redes = RedeNeural_PrevisaoFinanceira.RNAssessor.ListarRedes(papel);
        }
    }
}
