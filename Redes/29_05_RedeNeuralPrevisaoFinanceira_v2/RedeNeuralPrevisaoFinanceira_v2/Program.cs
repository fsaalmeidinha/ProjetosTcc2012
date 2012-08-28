using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBaseUtils;

namespace RedeNeuralPrevisaoFinanceira_v2
{
    class Program
    {
        static void Main(string[] args)
        {
            //new RNAssessor().TreinarRedes();
            RNAssessor.PreverCotacao(new DateTime(2011, 1, 1), 5);
        }
    }
}
