using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader sr = new StreamReader(@"C:\Felipe\TCC\DadosTreinamento\Diarias\Diarias\03_2012\COTAHIST_D01032012.txt");
            string arquivo = sr.ReadToEnd();
            arquivo = arquivo.Substring(245, arquivo.Length - (246 * 2));//Remove o 'cabeçolho' e o 'footer'
            arquivo = arquivo.Replace("\r\n", "");
            while (arquivo.Length > 0)
            {
                CotacaoBovespa cb = new CotacaoBovespa();
                cb.TipoRegistro = Convert.ToInt16(arquivo.Substring(0, 2));
                cb.DataPregao = new DateTime(Convert.ToInt32(arquivo.Substring(2, 4)), Convert.ToInt32(arquivo.Substring(6, 2)), Convert.ToInt32(arquivo.Substring(8, 2)));
                arquivo = arquivo.Substring(10, arquivo.Length - 10);
            }
        }

        public class CotacaoBovespa
        {
            /// <summary>
            /// TIPREG - TIPO DE REGISTRO 
            /// FIXO “01”
            /// N(02) 01 02
            /// </summary>
            public Int16 TipoRegistro { get; set; }
            /// <summary>
            /// DATA DO PREGÃO 
            /// FORMATO  “AAAAMMDD” 
            /// N(08) 03 10
            /// </summary>
            public DateTime DataPregao { get; set; }
            /// <summary>
            /// CODBDI  - CÓDIGO BDI
            /// UTILIZADO PARA CLASSIFICAR OS PAPÉIS NA EMISSÃO DO BOLETIM DIÁRIO DE INFORMAÇÕES VER TABELA ANEXA
            /// X(02) 11 1
            /// </summary>
            public Int16 CodigoBDI { get; set; }
            /// <summary>
            /// EGOCIAÇÃO DO PAPEL X(12) 13 24
            /// </summary>
            public string CodigoNegociacao { get; set; }
            /// <summary>
            /// TPMERC - TIPO DE MERCADO
            /// CÓD. DO MERCADO EM QUE O PAPEL ESTÁ CADASTRADO
            /// N(03) 25 27
            /// </summary>
            public Int16 TipoMercado { get; set; }
            /// <summary>
            /// NOMRES - NOME RESUMIDO DA EMPRESA EMISSORA DO PAPEL 
            /// X(12) 28 3
            /// </summary>
            public string NomeResumidoEmpresa { get; set; }
            /// <summary>
            /// ESPECI - ESPECIFICAÇÃO DO PAPEL 
            /// VER TABELA ANEXA 
            /// X(10) 40 49
            /// </summary>
            public string EspecificacaoPapel { get; set; }
            /// <summary>
            /// PRAZOT - PRAZO EM DIAS  DO MERCADO A TERMO 
            /// X(03) 50 52
            /// </summary>
            public int PrazoEmDiasDoMercadoATermo { get; set; }
            /// <summary>
            /// MODREF - MOEDA DE REFERÊNCIA
            /// MOEDA USADA NA DATA DO PREGÃO 
            /// X(04) 53 56
            /// </summary>
            public string Moeda { get; set; }
            /// <summary>
            /// PREABE - PREÇO DE ABERTURA DO PAPEL-MERCADO NO PREGÃO
            /// N(11)V99 57 69
            /// </summary>
            public double PrecoAberturaNoPregao { get; set; }
            /// <summary>
            /// PREMAX - PREÇO MÁXIMO DO PAPEL-MERCADO NO PREGÃO 
            /// N(11)V99 70 82
            /// </summary>
            public double PrecoMaximoPapelNoPregao { get; set; }
            /// <summary>
            /// PREMIN - PREÇO MÍNIMO DO PAPELMERCADO NO PREGÃO 
            /// N(11)V99 83 95
            /// </summary>
            public double PrecoMinimoPapelNoPregao { get; set; }
            /// <summary>
            /// PREMED - PREÇO MÉDIO DO PAPELMERCADO NO PREGÃO 
            /// N(11)V99 96 108
            /// </summary>
            public double PrecoMedioPapelNoPregao { get; set; }
            /// <summary>
            /// PREULT - PREÇO DO ÚLTIMO NEGÓCIO DO PAPEL-MERCADO  NO PREGÃO 
            /// N(11)V99 109 121
            /// </summary>
            public double PrecoFechamentoPapelNoPregao { get; set; }
            /// <summary>
            /// PREOFC - PREÇO DA MELHOR OFERTA DE COMPRA DO PAPEL-MERCADO 
            /// N(11)V99 122 134
            /// </summary>
            public double PrecoMelhorOfertaCompraPapelNoMercado { get; set; }
            /// <summary>
            /// PREOFV - PREÇO DA MELHOR OFERTA DE VENDA DO PAPEL-MERCADO 
            /// N(11)V99 135 147
            /// </summary>
            public double PrecoMelhorOfertaVendaPapelNoMercado { get; set; }
            /// <summary>
            /// TOTNEG - NEG. - NÚMERO DE NEGÓCIOS EFETUADOS COM O PAPELMERCADO NO PREGÃO 
            /// N(05) 148 152
            /// </summary>
            public int NumeroNegociacoesPapelNoPregao { get; set; }
            /// <summary>
            /// QUATOT - QUANTIDADE TOTAL DE TÍTULOS NEGOCIADOS NESTE PAPELMERCADO 
            /// N(18) 153 170
            /// </summary>
            public int QuantidadeTitulosNegociadosNoPapel { get; set; }
            /// <summary>
            /// VOLTOT - VOLUME TOTAL DE TÍTULOS NEGOCIADOS NESTE PAPELMERCADO 
            /// N(16)V99 171 188
            /// </summary>
            public double VolumeTitulosNegociadosNoPapel { get; set; }
            /// <summary>
            /// PREEXE - PREÇO DE EXERCÍCIO PARA O MERCADO DE OPÇÕES OU VALOR DO  CONTRATO PARA O MERCADO DE TERMO SECUNDÁRIO 
            /// N(11)V99 189 201
            /// </summary>
            public double PrecoExercicioDoPapel { get; set; }
            /// <summary>
            /// INDOPC - INDICADOR DE CORREÇÃO DE PREÇOS DE EXERCÍCIOS OU VALORES DE CONTRATO PARA OS MERCADOS DE OPÇÕES OU TERMO SECUNDÁRIO 
            /// N(01) 202 202
            /// </summary>
            public int IndicadorCorrecaoPrecoExercicio { get; set; }
            /// <summary>
            /// DATVEN - DATA DO VENCIMENTO PARA OS MERCADOS DE OPÇÕES OU TERMO SECUNDÁRIO 
            /// FORMATO “AAAAMMDD”  
            /// N(08) 203 210
            /// </summary>
            public DateTime DataVencimentoOpcaoOuTermoSecundario { get; set; }
            /// <summary>
            /// FATCOT - FATOR DE COTAÇÃO DO PAPEL 
            /// ‘1’ = COTAÇÃO UNITÁRIA ‘1000’ = COTAÇÃO POR LOTE DE MIL AÇÕES 
            /// N(07) 211 217
            /// </summary>
            public int FatorCotacaoDoPapel { get; set; }
            /// <summary>
            /// PTOEXE - PREÇO DE EXERCÍCIO EM PONTOS PARA OPÇÕES REFERENCIADAS EM DÓLAR OU VALOR DE CONTRATO EM PONTOS PARA TERMO SECUNDÁRIO
            /// PARA OS REFERENCIADOS EM DÓLAR, CADA PONTO EQUIVALE AO VALOR, NA MOEDA CORRENTE, DE UM CENTÉSIMO DA TAXA MÉDIA DO DÓLAR COMERCIAL INTERBANCÁRIO DE FECHAMENTO DO DIA ANTERIOR, OU SEJA, 1 PONTO = 1/100 US$ 
            /// N(07)V06 218 230
            /// </summary>
            public double PontosExercicio { get; set; }
            /// <summary>
            /// CODI S I  - CÓDIGO DO PAPEL NO SISTEMA ISIN OU CÓDIGO INTERNO DO PAPEL
            /// CÓDIGO DO PAPEL NO SISTEMA ISIN A PARTIR DE 15-05-1995 
            /// X(12) 231 242
            /// </summary>
            public string CodigoInternoPapel { get; set; }
            /// <summary>
            /// DISMES - NÚMERO DE DISTRIBUIÇÃO DO PAPEL
            /// NÚMERO DE SEQÜÊNCIA DO PAPEL CORRESPONDENTE AO ESTADO DE DIREITO VIGENTE 
            /// 9(03) 243 245
            /// </summary>
            public int NumeroDistribuicaoPapel { get; set; }

        }

    }
}
