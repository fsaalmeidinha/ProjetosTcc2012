using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace DataBaseUtils
{
    public class DataBaseUtils
    {
        public static List<DadosBE> RecuperarCotacoesAtivo(string papel)
        {
            List<DadosBE> listCotacoes = new List<DadosBE>();
            DadosBE Cotacao = null;
            DataTableReader dtr = null;

            try
            {
                dtr = RetornaDados(papel);

                while (dtr.Read())
                {
                    Cotacao = new DadosBE();

                    Cotacao.Id = (int)dtr["id"];
                    Cotacao.NomeReduzido = dtr["nomeresumido"].ToString();
                    Cotacao.DataGeracao = (DateTime)dtr["datageracao"];
                    Cotacao.PrecoAbertura = (decimal)dtr["precoabertura"];
                    Cotacao.PrecoAberturaNormalizado = (decimal)dtr["precoaberturaNormalizado"];

                    listCotacoes.Add(Cotacao);
                }
                TratarDesdobramento(listCotacoes);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (dtr != null)
                    dtr.Dispose();
            }

            return listCotacoes;
        }

        public static double NormalizarDado(double dado, string papel)
        {
            double min = Convert.ToDouble(Resources.ResourceManager.GetString("ValorMinimo" + papel.ToUpper()));
            double max = Convert.ToDouble(Resources.ResourceManager.GetString("ValorMaximo" + papel.ToUpper()));

            return (dado - min) / (max - min);
        }

        public static List<double> NormalizarDados(List<double> dados, string papel)
        {
            double min = Convert.ToDouble(Resources.ResourceManager.GetString("ValorMinimo" + papel.ToUpper()));
            double max = Convert.ToDouble(Resources.ResourceManager.GetString("ValorMaximo" + papel.ToUpper()));

            List<double> dadosList = new List<double>();
            dados.ForEach(dado => dadosList.Add((dado - min) / (max - min)));
            return dadosList;
        }

        public static double DesnormalizarDado(double dado, string papel)
        {
            double min = Convert.ToDouble(Resources.ResourceManager.GetString("ValorMinimo" + papel.ToUpper()));
            double max = Convert.ToDouble(Resources.ResourceManager.GetString("ValorMaximo" + papel.ToUpper()));

            return dado * (max - min) + min;
        }

        public static List<double> DesnormalizarDados(List<double> dados, string papel)
        {
            double min = Convert.ToDouble(Resources.ResourceManager.GetString("ValorMinimo" + papel.ToUpper()));
            double max = Convert.ToDouble(Resources.ResourceManager.GetString("ValorMaximo" + papel.ToUpper()));

            List<double> dadosList = new List<double>();
            dados.ForEach(dado => dadosList.Add(dado * (max - min) + min));
            return dadosList;
        }

        /// <summary>
        /// Retorna uma lista de KeyValuePair, onde a chave são os valores de input e o seu valor correspondente são os valores de output
        /// </summary>
        /// <param name="dados"></param>
        /// <param name="janelaEntrada">tamanho do input</param>
        /// <param name="janelaSaida">tamanho do output</param>
        /// <param name="considerarSaidasComoEntradas">se verdadeiro, teremos 'dados.Count / janelaEntrada' registros como saida, caso contrario, 'dados.Count / (janelaEntrada + janelaSaida)' </param>
        /// <returns></returns>
        public static List<KeyValuePair<double[], double[]>> SelecionarCotacoesPorJanelamento(List<double> dados, int janelaEntrada, int janelaSaida, bool considerarSaidasComoEntradas)
        {
            /*Cria um mapeamento de entradas para saida com o janelamento informado*/
            List<KeyValuePair<double[], double[]>> dadosPorJanelamento = new List<KeyValuePair<double[], double[]>>();
            for (int i = 0; i < dados.Count - (janelaEntrada + janelaSaida); i += janelaEntrada + (considerarSaidasComoEntradas ? 0 : janelaSaida))
            {
                dadosPorJanelamento.Add(new KeyValuePair<double[], double[]>(dados.Skip(i).Take(janelaEntrada).ToArray(), dados.Skip(i + janelaEntrada).Take(janelaSaida).ToArray()));
            }
            /*Cria um mapeamento de entradas para saida com o janelamento informado*/

            return dadosPorJanelamento;
        }

        /// <summary>
        /// Trata os desdobramentos (verifica alterações de 50% no valor de um dia para o outro)
        /// </summary>
        /// <param name="listCotacoes"></param>
        private static void TratarDesdobramento(List<DadosBE> listCotacoes)
        {
            //Tratando desdobramento
            for (int i = 1; i < listCotacoes.Count; i++)
            {
                if (listCotacoes[i].PrecoAbertura >= listCotacoes[i - 1].PrecoAbertura * (decimal)1.5 || listCotacoes[i].PrecoAbertura <= listCotacoes[i - 1].PrecoAbertura / (decimal)1.5)
                {
                    decimal desdobramento = listCotacoes[i].PrecoAbertura / listCotacoes[i - 1].PrecoAbertura;
                    //Caso haja um desdobramento, tratar todos os dados seguintes
                    for (int j = i; j < listCotacoes.Count; j++)
                    {
                        listCotacoes[j].PrecoAbertura /= desdobramento;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nomePapel"></param>
        /// <returns></returns>
        private static DataTableReader RetornaDados(String nomePapel)
        {
            SqlCommand cmdSQL = new SqlCommand();
            DataSet dsSQL = new DataSet();
            DataTableReader oDataTable = null;

            cmdSQL.Connection = RetornaConexao();
            cmdSQL.CommandType = System.Data.CommandType.StoredProcedure;
            cmdSQL.CommandText = "SP_BUSCA_DADOS";

            cmdSQL.Parameters.AddWithValue("@nomepapel", nomePapel);

            try
            {
                new SqlDataAdapter(cmdSQL).Fill(dsSQL);
                oDataTable = dsSQL.CreateDataReader();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmdSQL.Connection.Dispose();
                cmdSQL.Dispose();
                dsSQL.Dispose();
            }

            return oDataTable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static SqlConnection RetornaConexao()
        {
            String conex = null;

            conex = ConfigurationManager.ConnectionStrings["FinanceInvest"].ConnectionString;

            SqlConnection oConexao = new SqlConnection(conex);

            return oConexao;
        }
    }
}
