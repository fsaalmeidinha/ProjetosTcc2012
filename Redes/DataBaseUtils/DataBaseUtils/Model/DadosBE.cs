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
        public decimal PrecoAbertura { get; set; }
        public DateTime DataGeracao { get; set; }
        public decimal CotacaoDolar { get; set; }

        //A partir da versao 3
        public decimal PrecoMaximo { get; set; }
        public decimal PrecoMinimo { get; set; }
        public decimal PrecoMedio { get; set; }
        public int TotalNegociacoes { get; set; }
        public int QuantidadeTotalNegociacoes { get; set; }
        public decimal ValorTotalNegociacoes { get; set; }
        
        //Removido
        //public decimal PrecoAberturaNormalizado { get; set; }
        
        //A partir da versao 2
        public double ValorNormalizado { get; set; }
        public double ValorBollinger { get; set; }
        public decimal CotacaoDolarNormalizado { get; set; }
        public double EstacaoDoAno { get; set; }
        //Propriedade para uso interno
        public double ValorNormalizadoPrevisto { get; set; }
        public decimal CotacaoDolarNormalizadoPrevisto { get; set; }
        public double ValorPrevisto { get; set; }
        public decimal CotacaoDolarPrevisto { get; set; }

        



        /// <summary>
        /// RN_V3, Explicação em: Redes\03_09_RedeNeural_PrevisaoFinanceira_v3\Metodos_Indices\Medias_Moveis.txt
        /// </summary>
        public double PontuacaoMediasMoveis { get; set; }
        /// <summary>
        /// RN_V3, Explicação em: Redes\03_09_RedeNeural_PrevisaoFinanceira_v3\Metodos_Indices\Percentual_Volume_Negociacoes.txt
        /// </summary>
        public double PercentualTotalNegociacoes { get; set; }
    }
}
