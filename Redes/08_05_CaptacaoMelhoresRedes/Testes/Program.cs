using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CaptacaoMelhoresRedes;

namespace Testes
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime dtNow = DateTime.Now;
            System.Diagnostics.Debug.WriteLine("Inicio: " + dtNow);

            RNAssessor rnAssessor = new RNAssessor();
            rnAssessor.TreinarRedesCaptacao();

            TimeSpan ts = DateTime.Now.Subtract(dtNow);
            System.Diagnostics.Debug.WriteLine("Fim: " + DateTime.Now);
            System.Diagnostics.Debug.WriteLine("Tempo gasto : " + ts);
        }
    }
}
