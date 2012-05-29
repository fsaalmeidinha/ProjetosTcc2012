using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CurrencyReader
{
    public class WebRequests
    {
        public static string LerCotacao(string moeda = "usd")
        {
            StringBuilder sb = new StringBuilder();
            for (DateTime dtAtual = new DateTime(2006, 01, 01); dtAtual <= new DateTime(2012, 04, 01); dtAtual = dtAtual.AddMonths(1))
            {
                Uri uri = new Uri(String.Format("http://currencies.apps.grandtrunk.net/getrange/{0}/{1}/{2}/brl",
                    dtAtual.ToString("yyyy-MM-dd"),
                    dtAtual.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"),
                    moeda));
                System.Net.WebRequest req = System.Net.WebRequest.Create(uri);
                System.Net.WebResponse resp = req.GetResponse();
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                sb.AppendLine(sr.ReadToEnd());
            }

            return sb.ToString();
        }
    }
}
