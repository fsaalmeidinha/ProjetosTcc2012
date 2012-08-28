using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataBaseUtils
{
    public class DadosBE
    {
        internal double MediaMovel;

        public int Id { get; set; }
        public string NomeReduzido { get; set; }
        public DateTime DataGeracao { get; set; }
        public decimal PrecoAbertura { get; set; }
        public decimal PrecoAberturaNormalizado { get; set; }

        //A partir da versao 2
        public double ValorNormalizado { get; set; }
        public double ValorBollinger { get; set; }
        public decimal CotacaoDolar { get; set; }
        public decimal CotacaoDolarNormalizado { get; set; }
        public double EstacaoDoAno { get; set; }
        //Propriedade para uso interno
        public double ValorNormalizadoPrevisto { get; set; }
        public decimal CotacaoDolarNormalizadoPrevisto { get; set; }
        public double ValorPrevisto { get; set; }
        public decimal CotacaoDolarPrevisto { get; set; }
    }
}
