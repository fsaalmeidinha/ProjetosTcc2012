using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CurrencyReader
{
    class Program
    {
        static void Main(string[] args)
        {
            string valores = WebRequests.LerCotacao();

            try
            {

                valores = valores.Replace(',', '.');
                List<String> cotacoes = valores.Split(new char[] { ' ', '\n' }).ToList(); ;
                cotacoes.RemoveAll(d => d.Contains('\r'));
                
                for (int i = 0; i < cotacoes.Count - 1; i += 2)
                {
                    DateTime data = Convert.ToDateTime(cotacoes[i]);

                    WebRequests.SalvarCotacao(data, Convert.ToDecimal(cotacoes[i + 1], System.Globalization.CultureInfo.GetCultureInfo("en-US")));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}
