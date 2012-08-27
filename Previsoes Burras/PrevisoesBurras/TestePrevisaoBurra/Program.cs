using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPonderada;
using MediaAritmetica;
using AlisamentoExponencialSimples;


namespace TestePrevisaoBurra
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime inicio = new DateTime(2009, 06, 01);
            DateTime termino = new DateTime(2009, 07, 01);

            /*
            //List<double[]> listaPonderada = MetodoMediaPonderada.PreverMediasPonderada("PETR4", inicio, termino);
            //List<double[]> listaPonderadaAleatoria = MetodoMediaPonderada.PreverMediasPonderadaAleatoria("PETR4", inicio, termino);
            //List<double[]> listaAritmetica = MetodoMediaAritmetica.PreverMediasAritmetica("PETR4", inicio, termino);
            //List<double[]> listaAritmeticaAleatoria = MetodoMediaAritmetica.PreverMediasAritmeticaAleatoria("PETR4", inicio, termino);
            
            for (int i = 0; i < (int)termino.Subtract(inicio).TotalDays; i++)
            {
                //Console.WriteLine("Valor real: {0}; Valor prev. MPonderada: {1}", listaPonderada[i][0], listaPonderada[i][1]);
                //Console.WriteLine("Valor real: {0}; Valor prev. MPonderada Al.: {1}", listaPonderadaAleatoria[i][0], listaPonderadaAleatoria[i][1]);
                //Console.WriteLine("Valor real: {0}; Valor prev MAritmetica: {1}", listaAritmetica[i][0], listaAritmetica[i][1]);
                //Console.WriteLine("Valor real: {0}; Valor prev MAritmetica Al.: {1}", listaAritmeticaAleatoria[i][0], listaAritmeticaAleatoria[i][1]);
            }
            */

            List<double[]> listaAlisamentoExpSimples = null;

            listaAlisamentoExpSimples = MetodoAlisamentoExpSimples.PreverAlisamentoExponencialSimples("PETR4", inicio, termino);

            for (int i = 0; i < (int)termino.Subtract(inicio).TotalDays; i++)
            {

                Console.WriteLine("Valor real: {0}; Valor prev MAl. Expo. Simples: {1}", listaAlisamentoExpSimples[i][0], listaAlisamentoExpSimples[i][1]);
                //Console.WriteLine("Error: {0}", listaAlisamentoExpSimples[i][1] - listaAlisamentoExpSimples[i][0]);
            }

            Console.Read();
        }
    }
}
