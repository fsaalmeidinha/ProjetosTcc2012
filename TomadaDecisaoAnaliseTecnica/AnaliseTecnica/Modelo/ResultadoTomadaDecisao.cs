using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnaliseTecnica.Modelo
{
    public class ResultadoTomadaDecisao
    {
        public string Papel { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int TamanhoTendencia { get; set; }
        public int TotalDados { get; set; }
        public int TotalNegociacoesDeCompra { get; set; }
        public int TotalAcertos { get; set; }
        public double PercentualAcerto { get; set; }
        public double PercentualGanhoPerda { get; set; }
        public double PercentualMedioGanhoPerda { get; set; }
    }
}
