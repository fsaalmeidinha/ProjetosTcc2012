using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedeNeuralPrevisaoFinanceira_v3;

namespace TesteRN_V3
{
    class Program
    {
        static void Main(string[] args)
        {
            //ETER3,GOLL4,NATU3,PETR4,VALE5
            new RNAssessor().TreinarRedes("VALE5");
        }
    }
}
