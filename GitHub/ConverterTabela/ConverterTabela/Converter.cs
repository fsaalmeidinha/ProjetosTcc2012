using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Linq;

namespace ConverterTabela
{
    /// <summary>
    /// 
    /// </summary>
    public class Converter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nomePapel"></param>
        /// <returns></returns>
        public List<DadosBE> DePara(String nomePapel)
        {
            List<DadosBE> listCotacoes = new List<DadosBE>();
            DadosBE Cotacao = null;
            DataTableReader dtr = null;

            try
            {
                dtr = RetornaDados(nomePapel);

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

        /// <summary>
        /// Trata os desdobramentos (verifica alterações de 50% no valor de um dia para o outro)
        /// </summary>
        /// <param name="listCotacoes"></param>
        private void TratarDesdobramento(List<DadosBE> listCotacoes)
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
        private DataTableReader RetornaDados(String nomePapel)
        {
            SqlCommand cmdSQL = new SqlCommand();
            DataSet dsSQL = new DataSet();
            Connection oConexao = new Connection();
            DataTableReader oDataTable = null;

            cmdSQL.Connection = oConexao.RetornaConexao();
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
    }

    /// <summary>
    /// 
    /// </summary>
    public class DadosBE
    {
        public int Id { get; set; }
        public string NomeReduzido { get; set; }
        public DateTime DataGeracao { get; set; }
        public decimal PrecoAbertura { get; set; }
        public decimal PrecoAberturaNormalizado { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class Connection
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public SqlConnection RetornaConexao()
        {
            String conex = null;

            conex = ConfigurationManager.ConnectionStrings["FinanceInvest"].ConnectionString;

            SqlConnection oConexao = new SqlConnection(conex);

            return oConexao;
        }
    }
}
