using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuronDotNet.Core;

namespace CaptacaoMelhoresRedes.Model
{
    internal class RedePrevisaoFinanceira
    {
        public RedePrevisaoFinanceira()
        {
            TaxaMediaAcertoPorDia = new Dictionary<int, double>();
        }

        public string NomeRede { get; set; }
        public int JanelaEntrada { get; set; }
        public int JanelaSaida { get; set; }
        public Network RedeNeuralPrevisaoFinanceira { get; set; }
        public Dictionary<int, double> TaxaMediaAcertoPorDia { get; set; }
    }
}
