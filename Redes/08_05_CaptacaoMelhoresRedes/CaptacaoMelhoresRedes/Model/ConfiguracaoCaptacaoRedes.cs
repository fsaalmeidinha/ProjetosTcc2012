using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaptacaoMelhoresRedes.Model
{
    internal class ConfiguracaoCaptacaoRedes
    {
        public ConfiguracaoCaptacaoRedes()
        {
            RedesPrevisao = new List<RedePrevisaoFinanceira>();
        }

        public string Papel { get; set; }
        public List<double> Dados { get; set; }
        public List<RedePrevisaoFinanceira> RedesPrevisao { get; set; }
    }
}
