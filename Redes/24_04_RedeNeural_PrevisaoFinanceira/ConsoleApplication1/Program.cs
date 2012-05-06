using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedeNeural_PrevisaoFinanceira;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            RNAssessor rnAssessor = new RNAssessor();
            rnAssessor.ExecutarRedeNeural("PETR4");
        }
    }
}
