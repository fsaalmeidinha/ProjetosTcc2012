using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace AnaliseTecnica.Modelo
{
    public class DadoBE
    {
        public double BandaCentral { get; set; }
        public double BandaInferior { get; set; }
        public double BandaSuperior { get; set; }
        #region PROPRIEDADES

        public int Id { get; set; }
        public string NomeReduzido { get; set; }
        public DateTime DataGeracao { get; set; }
        public double CotacaoDolar { get; set; }
        //public double CotacaoDolarNormalizado { get; set; }
        public double PrecoFechamento { get; set; }
        //public double PrecoFechamentoNormalizado { get; set; }
        public double PrecoMaximo { get; set; }
        public double PrecoMinimo { get; set; }
        public double PrecoAbertura { get; set; }
        public int QuantidadeTotalNegociacoes { get; set; }
        public long VolumeNegociacao { get; set; }
        public DadoBE Anterior { get; private set; }
        public DadoBE Proximo { get; private set; }

        #endregion PROPRIEDADES

        #region INDICES
        private double MediaMovel;
        public List<double> ValorBollinger { get; set; }

        public double PercentualTotalNegociacoes { get; set; }
        public double PercentualTotalNegociacoesMediaNDias { get; set; }

        public double PercentualCrescimentoDolar { get; set; }
        public double PercentualCrescimentoValorAtivoMediaNDias { get; set; }

        public List<double> PercentualCrescimentoValorAtivo { get; set; }

        public double PercentualDesviosPadroesEmRelacaoNDias { get; set; }

        public List<double> DuracaoTendencias { get; set; }
        public List<double> AnaliseMediaMovelSimples5Dias { get; set; }
        #endregion INDICES

        public static List<DadoBE> PegarTodos(string papel)
        {
            List<DadoBE> listCotacoes = new List<DadoBE>();
            DataTableReader dtr = null;

            try
            {
                dtr = RetornaDados(papel);

                while (dtr.Read())
                {
                    DadoBE cotacao = new DadoBE();

                    cotacao.Id = Convert.ToInt32(dtr["ID"]);
                    cotacao.NomeReduzido = papel.ToUpper();
                    cotacao.DataGeracao = Convert.ToDateTime(dtr["DATAGERACAO"]);
                    cotacao.CotacaoDolar = Convert.ToDouble(dtr["VALORDOLAR"]);
                    //cotacao.CotacaoDolarNormalizado = NormalizarDado(cotacao.CotacaoDolar, "DOLAR");
                    cotacao.PrecoMaximo = Convert.ToDouble(dtr["PRECOMAXIMO"]);
                    cotacao.PrecoMinimo = Convert.ToDouble(dtr["PRECOMINIMO"]);
                    cotacao.PrecoAbertura = Convert.ToDouble(dtr["PRECOABERTURA"]);
                    cotacao.VolumeNegociacao = Convert.ToInt64(dtr["VOLUMENEGOCIACAO"]);
                    cotacao.PrecoFechamento = Convert.ToDouble(dtr["PRECOFECHAMENTO"]);

                    /*cotacao.Id = (int)dtr["id"];
                    cotacao.NomeReduzido = dtr["nomeresumido"].ToString();
                    cotacao.DataGeracao = (DateTime)dtr["datageracao"];
                    cotacao.CotacaoDolar = Convert.ToDouble((decimal)dtr["valorDolar"]);
                    //cotacao.CotacaoDolarNormalizado = NormalizarDado(cotacao.CotacaoDolar, "DOLAR");
                    cotacao.PrecoMaximo = Convert.ToDouble((decimal)dtr["PRECOMAX"]);
                    cotacao.PrecoMinimo = Convert.ToDouble((decimal)dtr["PRECOMIN"]);
                    cotacao.PrecoMedio = Convert.ToDouble((decimal)dtr["PRECOMED"]);
                    cotacao.QuantidadeTotalNegociacoes = (int)dtr["QUANTIDADETOTALNEGO"];
                    cotacao.PrecoFechamento = Convert.ToDouble((decimal)dtr["precoabertura"]);*/

                    listCotacoes.Add(cotacao);
                }
                //Ordena pela data
                listCotacoes = listCotacoes.OrderBy(cot => cot.DataGeracao).ToList();
                // TratarDesdobramento(listCotacoes);

                for (int indCotacao = 0; indCotacao < listCotacoes.Count - 2; indCotacao++)
                {
                    listCotacoes[indCotacao].PrecoFechamento = listCotacoes[indCotacao + 1].PrecoFechamento;
                }
                //Elimina os 2 ultimos pq não tem o preço de fechamento nem o preço de fechamendo do dia seguinte
                listCotacoes = listCotacoes.Take(listCotacoes.Count - 2).ToList();


                listCotacoes[0].Proximo = listCotacoes[1];
                listCotacoes.Last().Anterior = listCotacoes[listCotacoes.Count - 2];
                for (int i = 1; i < listCotacoes.Count - 1; i++)
                {
                    listCotacoes[i].Proximo = listCotacoes[i + 1];
                    listCotacoes[i].Anterior = listCotacoes[i - 1];
                }

                listCotacoes = PreencherIndices(listCotacoes);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao recuperar os dados");
            }
            finally
            {
                if (dtr != null)
                    dtr.Dispose();
            }

            return listCotacoes;
            //return listCotacoes.Skip(listCotacoes.Count / 10 * 8).ToList();
        }

        public DadoBE PegarNApos(int n)
        {
            DadoBE dadoRetornar = this;
            for (int i = 0; i < n; i++)
            {
                dadoRetornar = dadoRetornar.Proximo;
            }

            return dadoRetornar;
        }

        public DadoBE PegarNAntes(int n)
        {
            DadoBE dadoRetornar = this;
            for (int i = 0; i < n; i++)
            {
                dadoRetornar = dadoRetornar.Anterior;
            }

            return dadoRetornar;
        }

        #region METODOS DE PREENCHIMENTO DOS INDICES

        private static List<DadoBE> PreencherIndices(List<DadoBE> listCotacoes)
        {
            int dadosIgnorarInicio = 0;
            dadosIgnorarInicio = Math.Max(dadosIgnorarInicio, PreencherValorBollinger(listCotacoes));

            return listCotacoes.Skip(dadosIgnorarInicio).ToList();
        }

        public static int PreencherValorBollinger(List<DadoBE> listCotacoes, int numeroDiasMedia = 20)
        {
            double d = 2;
            if (numeroDiasMedia < 15) d = 1.9;
            if (numeroDiasMedia > 30) d = 2.1;

            listCotacoes.Take(numeroDiasMedia * 2).ToList().ForEach(cot => cot.ValorBollinger = new List<double>() { 0, 0 });
            //Analisaremos periodos de 20 dias
            for (int i = numeroDiasMedia; i < listCotacoes.Count; i++)
            {
                //Calcula a média dos 20 dias anteriores e alimenta a propriedade "MediaMovel" do dado
                listCotacoes[i].MediaMovel = ValorMediaMovel(listCotacoes[i], numeroDiasMedia);//Convert.ToDouble(listCotacoes.Skip(i - numeroDiasMedia).Take(numeroDiasMedia).Sum(cot => cot.PrecoFechamento) / numeroDiasMedia);
                //Temos que calcular o desvio padrao da BandaCentral (MediaMovel), portanto isso só é possivel quando tivermos ao menos 20 médias móveis calculadas
                if (i >= (numeroDiasMedia * 2))
                {
                    //Calculo das bandas http://www.investmax.com.br/iM/content.asp?contentid=660
                    double bandaCentral = listCotacoes[i].MediaMovel;
                    double bandaSuperior = listCotacoes[i].MediaMovel + d * Math.Sqrt(listCotacoes.Skip(i - numeroDiasMedia).Take(numeroDiasMedia).Sum(cot => Math.Pow((double)cot.PrecoFechamento - listCotacoes[i].MediaMovel, 2)) / numeroDiasMedia);
                    double bandaInferior = listCotacoes[i].MediaMovel - d * Math.Sqrt(listCotacoes.Skip(i - numeroDiasMedia).Take(numeroDiasMedia).Sum(cot => Math.Pow((double)cot.PrecoFechamento - listCotacoes[i].MediaMovel, 2)) / numeroDiasMedia);

                    listCotacoes[i - 1].BandaCentral = listCotacoes[i].MediaMovel;
                    listCotacoes[i - 1].BandaInferior = bandaInferior;
                    listCotacoes[i - 1].BandaSuperior = bandaSuperior;

                    ////Ex: bandaSuperior = 10, bandaInferior = 2, cotacao = 4.8567, temos: (4.8567 - 2) * 1 / (10-2) = 0.3570875
                    //listCotacoes[i].ValorBollinger = 1 / (bandaSuperior - bandaInferior) * ((double)listCotacoes[i - 1].PrecoFechamento - bandaInferior);

                    //O ativo está acima da banda central
                    if (listCotacoes[i - 1].PrecoFechamento > bandaCentral)
                    {
                        double val = 1 / (bandaSuperior - bandaCentral) * ((double)listCotacoes[i - 1].PrecoFechamento - bandaCentral);
                        //val = val > 1 ? 1 : val;
                        val = val > 0.8 ? 1 : 0;
                        listCotacoes[i].ValorBollinger = new List<double>() { 0, val };
                    }
                    //O ativo está abaixo da banda central
                    else
                    {
                        double val = 1 - (1 / (bandaCentral - bandaInferior) * ((double)listCotacoes[i - 1].PrecoFechamento - bandaInferior));
                        val = val > 0.8 ? 1 : 0;
                        listCotacoes[i].ValorBollinger = new List<double>() { val, 0 };
                    }
                }

                //Desvio Padrao - http://pt.wikipedia.org/wiki/Desvio_padr%C3%A3o
            }

            return numeroDiasMedia * 2;
        }

        public static double ValorMediaMovel(DadoBE dadoBE, int n)
        {
            int count = 0;
            double somatorio = 0;
            DadoBE dadoBE_SMA = dadoBE.Anterior;
            for (int i = 0; i < n; i++)
            {
                if (dadoBE_SMA == null)
                    break;
                count++;
                somatorio += dadoBE_SMA.PrecoFechamento;
                dadoBE_SMA = dadoBE_SMA.Anterior;
            }

            if (count == 0)
                return 0;
            return somatorio / count;
        }

        #endregion METODOS DE PREENCHIMENTO DOS INDICES

        #region Métodos para o acesso ao BD(Wagner)

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

        #endregion Métodos para o acesso ao BD(Wagner)

        #region DESDOBRAMENTO

        /// <summary>
        /// Trata os desdobramentos (verifica alterações de 50% no valor de um dia para o outro)
        /// </summary>
        /// <param name="listCotacoes"></param>
        private static void TratarDesdobramento(List<DadoBE> listCotacoes)
        {
            //Tratando desdobramento
            for (int i = 1; i < listCotacoes.Count; i++)
            {
                if (listCotacoes[i].PrecoFechamento >= listCotacoes[i - 1].PrecoFechamento * (double)1.5 || listCotacoes[i].PrecoFechamento <= listCotacoes[i - 1].PrecoFechamento / (double)1.5)
                {
                    double desdobramento = listCotacoes[i].PrecoFechamento / listCotacoes[i - 1].PrecoFechamento;
                    //Caso haja um desdobramento, tratar todos os dados seguintes
                    for (int j = i; j < listCotacoes.Count; j++)
                    {
                        listCotacoes[j].PrecoAbertura /= desdobramento;
                        listCotacoes[j].PrecoFechamento /= desdobramento;
                        listCotacoes[j].PrecoMinimo /= desdobramento;
                        listCotacoes[j].PrecoMaximo /= desdobramento;
                    }
                }
            }
        }

        #endregion DESDOBRAMENTO
    }
}
