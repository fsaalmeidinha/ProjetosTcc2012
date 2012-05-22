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
            List<string> redes = RNAssessor.ListarRedes("PETR4");

            DateTime dtNow = DateTime.Now;
            System.Diagnostics.Debug.WriteLine("Inicio: " + dtNow);
            RNAssessor rnAssessor = new RNAssessor();
            List<string> nomeRedes = rnAssessor.TreinarRedes();

            TimeSpan ts = DateTime.Now.Subtract(dtNow);
            System.Diagnostics.Debug.WriteLine("Fim: " + DateTime.Now);
            System.Diagnostics.Debug.WriteLine("Tempo gasto : " + ts);
        }
    }
}
