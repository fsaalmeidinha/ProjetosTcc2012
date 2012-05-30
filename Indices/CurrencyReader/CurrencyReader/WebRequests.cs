using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

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

        public static void SalvarCotacao(DateTime data, decimal valor)
        {
            SqlConnection conn = null;
            SqlCommand sqlComan = null;

            try
            {
                conn = new SqlConnection(@"Data Source=WAGNER-PC\SQLEXPRESS;Initial Catalog=FinanceInvest;Integrated Security=True");
                conn.Open();

                sqlComan = new SqlCommand("Insert into CotacaoDolar(data, valor) values(@data, @valor)", conn);
                sqlComan.CommandType = System.Data.CommandType.Text;

                sqlComan.Parameters.Add("@data", System.Data.SqlDbType.SmallDateTime);
                sqlComan.Parameters["@data"].Value = data;
                sqlComan.Parameters.Add("@valor", System.Data.SqlDbType.Decimal);
                sqlComan.Parameters["@valor"].Value = valor;

                sqlComan.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlComan != null)
                    sqlComan.Dispose();
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
    }
}
