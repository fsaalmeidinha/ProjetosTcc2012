using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataBaseUtils
{
    public class DadosBE
    {
        public int Id { get; set; }
        public string NomeReduzido { get; set; }
        public DateTime DataGeracao { get; set; }
        public decimal PrecoAbertura { get; set; }
        public decimal PrecoAberturaNormalizado { get; set; }
    }
}
