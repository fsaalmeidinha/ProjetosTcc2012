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
        public double PrecoFechamentoDiaSeguinte { get; set; }
        public decimal PrecoAbertura { get; set; }
        public double PrecoFechamento { get; set; }
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
        public double PrecoFechamentoNormalizadoDiaSeguinte { get; set; }
        public double PrecoFechamentoNormalizado { get; set; }
        public double ValorBollinger { get; set; }
        public decimal CotacaoDolarNormalizado { get; set; }
        public double EstacaoDoAno { get; set; }
        //Propriedade para uso interno
        public double ValorNormalizadoPrevisto { get; set; }
        public decimal CotacaoDolarNormalizadoPrevisto { get; set; }
        public double ValorPrevisto { get; set; }
        public decimal CotacaoDolarPrevisto { get; set; }





        /// <summary>
        /// INDICE- RN_V3, Explicação em: Redes\03_09_RedeNeural_PrevisaoFinanceira_v3\Metodos_Indices\3_Medias_Moveis.doc
        /// </summary>
        public double Pontuacao3MediasMoveis { get; set; }
        /// <summary>
        /// INDICE- RN_V3, Explicação em: Redes\03_09_RedeNeural_PrevisaoFinanceira_v3\Metodos_Indices\PercentualTotalNegociacoesMediaNDias.doc
        /// </summary>
        public double PercentualTotalNegociacoesMediaNDias { get; set; }
        /// <summary>
        /// INDICE- RN_V3, Explicação em: Redes\03_09_RedeNeural_PrevisaoFinanceira_v3\Metodos_Indices\PercentualTotalNegociacoes.doc
        /// </summary>
        public double PercentualTotalNegociacoes { get; set; }
        /// <summary>
        /// INDICE- RN_V3, Explicação em: Redes\03_09_RedeNeural_PrevisaoFinanceira_v3\Metodos_Indices\PercentualCrescimentoDolar.doc
        /// </summary>
        public double PercentualCrescimentoDolar { get; set; }
        /// <summary>
        /// INDICE- RN_V3, Explicação em: Redes\03_09_RedeNeural_PrevisaoFinanceira_v3\Metodos_Indices\PercentualCrescimentoAtivoMediaNDias.doc
        /// </summary>
        public double PercentualCrescimentoValorAtivoMediaNDias { get; set; }
        /// <summary>
        /// INDICE- RN_V3, Explicação em: Redes\03_09_RedeNeural_PrevisaoFinanceira_v3\Metodos_Indices\PercentualCrescimentoValorAtivo.doc
        /// O ultimo item da lista é o mais recente, sendo o penultimo o segundo mais recente e assim por diante
        /// </summary>
        public List<double> PercentualCrescimentoValorAtivo { get; set; }
        /// <summary>
        /// INDICE- RN_V3, Explicação em: Redes\03_09_RedeNeural_PrevisaoFinanceira_v3\Metodos_Indices\PercentualDesviosPadroesEmRelacaoNDias.doc
        /// </summary>
        public double PercentualDesviosPadroesEmRelacaoNDias { get; set; }
        /// <summary>
        /// INDICE- RN_V3, Dia da semana da cotação do ativo
        /// </summary>
        public double DiaSemana { get; set; }
        /// <summary>
        /// INDICE- RN_V3, Explicação em: Redes\03_09_RedeNeural_PrevisaoFinanceira_v3\Metodos_Indices\PercentualValorAtivo_Max_Min_Med.doc
        /// </summary>
        public double PercentualValorAtivo_Max_Min_Med { get; set; }
    }
}
