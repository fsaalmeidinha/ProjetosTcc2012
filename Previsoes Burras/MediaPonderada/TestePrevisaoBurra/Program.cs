using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPonderada;


namespace TestePrevisaoBurra
{
    class Program
    {
        static void Main(string[] args)
        {
            List<double[]> lista = MetodoMediaPonderada.PreverMediasPonderada("PETR4", new DateTime(2009, 06, 01), new DateTime(2009, 07, 01));

            for (int i = 0; i < lista.Count; i++)
            {
                Console.WriteLine("Valor real: {0}; Valor previsto: {1}", lista[i][0], lista[i][1]);
            }

            Console.Read();
        }
    }
}
