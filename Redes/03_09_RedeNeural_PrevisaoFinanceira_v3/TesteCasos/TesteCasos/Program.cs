using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TesteCasos
{
    class Program
    {
        static void Main(string[] args)
        {
            //////List<double> valores = new List<double>() { 5, 6, 2 };
            List<double> valores = new List<double>() { 3, 2, 1 };
            //List<double> valores = new List<double>() { 3, 1, 2 };
            //List<double> valores = new List<double>() { 2, 4, 3 };
            //List<double> valores = new List<double>() { 2, 3, 1 };
            //List<double> valores = new List<double>() { 1, 2, 3 };
            List<bool> valoresBool = RecuperaValoresBooleanos(valores);
            List<int> arrayValores = RecuperarArrayTransformacaoBoolInt(valoresBool);

        }

        static List<bool> RecuperaValoresBooleanos(List<double> valores)
        {
            List<bool> valoresBool = new List<bool>();
            for (int indValX = valores.Count - 1; indValX > 0; indValX--)
            {
                for (int indValY = indValX - 1; indValY >= 0; indValY--)
                {
                    valoresBool.Add(valores[indValX] > valores[indValY]);
                }
            }
            return valoresBool;
        }

        static List<int> RecuperarArrayTransformacaoBoolInt(List<bool> valoresBool)
        {
            Func<int, int> getIntFromBoolInd = ind => Convert.ToInt32(Math.Pow(2, ind));
            int valor = 0;
            for (int i = 0; i < valoresBool.Count; i++)
            {
                if (valoresBool[i])
                    valor += getIntFromBoolInd(i);
            }

            int numComb = RecuperarNumeroCombinacoes(valoresBool.Count);
            List<int> arrayRetorno = new List<int>();
            for (int i = 0; i < numComb; i++)
            {
                if (i == valor)
                    arrayRetorno.Add(1);
                else
                    arrayRetorno.Add(0);
            }

            return arrayRetorno;
        }

        static int RecuperarNumeroCombinacoes(int count)
        {
            return Convert.ToInt32(Math.Pow(2, count));
        }
    }
}
