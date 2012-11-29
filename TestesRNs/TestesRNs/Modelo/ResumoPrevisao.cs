using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestesRNs.Modelo
{
    public class ResumoPrevisao
    {
        public ResumoPrevisao()
        {
            this.Previsoes = new List<Previsao>();
        }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int TamanhoTendencia { get; set; }
        public string Papel { get; set; }
        public List<Versao> Versoes { get; set; }
        public List<Previsao> Previsoes { get; set; }

        public class Previsao
        {
            public DateTime Data { get; set; }
            public double ValorAtivo { get; set; }
            public ResultadoPrevisao ResultadoPrevisao { get; set; }
        }
    }
}
