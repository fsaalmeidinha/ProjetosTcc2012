using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestesRNs.Modelo;
using TestesRNs.RedeNeural;

namespace TestesRNs
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-br");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("pt-br");

            List<string> papeis = new List<string>() { "BVSP", "ETER3", "GOLL4", "NATU3", "PETR4", "VALE5" };
            List<Versao> versoes = new List<Versao>() { Versao.V6001, Versao.V6002, Versao.V6004, Versao.V6008, Versao.V6016, Versao.V6032 };
            List<List<Versao>> versoesTreinar = new List<List<Versao>>();

            for (int i = 1; i <= (versoes.Max(ver => Convert.ToInt32(ver)) - 6000) * 2 - 1; i++)
            {
                string bin = ToBinary(i).PadLeft(versoes.Count, '0');
                //Seleciona os índices que devem ser adicionados
                List<int> indicesAdicionar = bin.Reverse()
                    .Select((val, ind) => new int[] { Convert.ToInt32(val.ToString()), ind })//Transforma em um array com o valor binario e o indice do valor
                    .Where(val => val[0] == 1)//Pega apenas os que tem valor binário == 1
                    .Select(val => val[1]).ToList();//Seleciona os índices do mesmo

                versoesTreinar.Add(versoes.Where((ver, ind) => indicesAdicionar.Contains(ind)).ToList());
            }

            List<Relatorio> relatorios = new List<Relatorio>();
            foreach (string papel in papeis)
            {
                foreach (List<Versao> versoesTreinoAtual in versoesTreinar)
                {
                    for (int tamanhoTendencia = 1; tamanhoTendencia <= 10; tamanhoTendencia++)
                    {
                        Relatorio relatorio = RNHelper.TreinarRedeNeural(papel, versoesTreinoAtual, tamanhoTendencia);
                        relatorios.Add(relatorio);
                    }
                }
            }

            Relatorio.GerarRelatorioExcel(relatorios);
        }

        private static string ToBinary(int inteiro)
        {
            // Declare a few variables we're going to need
            Int64 BinaryHolder;
            char[] BinaryArray;
            string BinaryResult = "";
            while (inteiro > 0)
            {

                BinaryHolder = inteiro % 2;

                BinaryResult += BinaryHolder;

                inteiro = inteiro / 2;

            }

            // The algoritm gives us the binary number in reverse order (mirrored)
            // We store it in an array so that we can reverse it back to normal
            BinaryArray = BinaryResult.ToCharArray();
            Array.Reverse(BinaryArray);
            BinaryResult = new string(BinaryArray);

            return BinaryResult;
        }
    }
}
