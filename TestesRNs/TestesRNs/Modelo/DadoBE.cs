using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace TestesRNs.Modelo
{
    public class DadoBE
    {
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
        public double PrecoMedio { get; set; }
        public int QuantidadeTotalNegociacoes { get; set; }
        public DadoBE Anterior { get; private set; }
        public DadoBE Proximo { get; private set; }

        #endregion PROPRIEDADES

        #region INDICES
        private double MediaMovel;
        public double ValorBollinger { get; set; }

        public double PercentualTotalNegociacoes { get; set; }
        public double PercentualTotalNegociacoesMediaNDias { get; set; }

        public double PercentualCrescimentoDolar { get; set; }
        public double PercentualCrescimentoValorAtivoMediaNDias { get; set; }

        public List<double> PercentualCrescimentoValorAtivo { get; set; }

        public double PercentualDesviosPadroesEmRelacaoNDias { get; set; }

        public List<double> DuracaoTendencias { get; set; }
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

                    cotacao.Id = (int)dtr["id"];
                    cotacao.NomeReduzido = dtr["nomeresumido"].ToString();
                    cotacao.DataGeracao = (DateTime)dtr["datageracao"];
                    cotacao.CotacaoDolar = Convert.ToDouble((decimal)dtr["valorDolar"]);
                    //cotacao.CotacaoDolarNormalizado = NormalizarDado(cotacao.CotacaoDolar, "DOLAR");
                    cotacao.PrecoMaximo = Convert.ToDouble((decimal)dtr["PRECOMAX"]);
                    cotacao.PrecoMinimo = Convert.ToDouble((decimal)dtr["PRECOMIN"]);
                    cotacao.PrecoMedio = Convert.ToDouble((decimal)dtr["PRECOMED"]);
                    cotacao.QuantidadeTotalNegociacoes = (int)dtr["QUANTIDADETOTALNEGO"];
                    cotacao.PrecoFechamento = Convert.ToDouble((decimal)dtr["precoabertura"]);

                    listCotacoes.Add(cotacao);
                }
                //Ordena pela data
                listCotacoes = listCotacoes.OrderBy(cot => cot.DataGeracao).ToList();
                TratarDesdobramento(listCotacoes);

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
            dadosIgnorarInicio = Math.Max(dadosIgnorarInicio, PreencherDuracaoTendencias(listCotacoes));

            return listCotacoes.Skip(dadosIgnorarInicio).ToList();
        }

        public static int PreencherValorBollinger(List<DadoBE> listCotacoes)
        {
            //Analisaremos periodos de 20 dias
            for (int i = 20; i < listCotacoes.Count; i++)
            {
                //Calcula a média dos 20 dias anteriores e alimenta a propriedade "MediaMovel" do dado
                listCotacoes[i].MediaMovel = Convert.ToDouble(listCotacoes.Skip(i - 20).Take(20).Sum(cot => cot.PrecoFechamento) / 20);
                //Temos que calcular o desvio padrao da BandaCentral (MediaMovel), portanto isso só é possivel quando tivermos ao menos 20 médias móveis calculadas
                if (i >= 40)
                {
                    //Calculo das bandas http://www.investmax.com.br/iM/content.asp?contentid=660 PS: Fizemos * 2.3 para dar uma margem a mais
                    double bandaSuperior = listCotacoes[i].MediaMovel + 2.3 * Math.Sqrt(Math.Pow(listCotacoes.Skip(i - 20).Take(20).Sum(cot => (double)cot.PrecoFechamento - cot.MediaMovel), 2) / 20);
                    double bandaInferior = listCotacoes[i].MediaMovel - 2.3 * Math.Sqrt(Math.Pow(listCotacoes.Skip(i - 20).Take(20).Sum(cot => (double)cot.PrecoFechamento - cot.MediaMovel), 2) / 20);

                    //Ex: bandaSuperior = 10, bandaInferior = 2, cotacao = 4.8567, temos: (4.8567 - 2) * 1 / (10-2) = 0.3570875
                    listCotacoes[i].ValorBollinger = 1 / (bandaSuperior - bandaInferior) * ((double)listCotacoes[i - 1].PrecoFechamento - bandaInferior);
                }

                //Desvio Padrao - http://pt.wikipedia.org/wiki/Desvio_padr%C3%A3o
            }

            return 40;
        }

        public static int PreencherDuracaoTendencias(List<DadoBE> listCotacoes)
        {
            List<double> listaTendencias = new List<double>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            //os menores índices representam as tendencias mais próximas
            //ind0-> variacao real() QUEDA
            //ind1-> 0.05 * qtdDiasNaTendencia QUEDA
            //ind0-> variacao real() AUMENTO
            //ind1-> 0.05 * qtdDiasNaTendencia AUMENTO

            //Se o primeiro valor for maior que o segundo, iniciamos em uma tendencia de queda
            bool tendenciaQueda = listCotacoes[0].PrecoFechamento > listCotacoes[1].PrecoFechamento;
            foreach (DadoBE dadoBE in listCotacoes)
            {
                if (dadoBE.Anterior == null || dadoBE.Anterior.Anterior == null)
                {
                    dadoBE.DuracaoTendencias = listaTendencias.Skip(listaTendencias.Count - 12).ToList();
                    continue;
                }

                double variacaoEmReais = Math.Abs(dadoBE.Anterior.Anterior.PrecoFechamento - dadoBE.Anterior.PrecoFechamento);
                double variacao = variacaoEmReais / dadoBE.Anterior.Anterior.PrecoFechamento;
                if (tendenciaQueda)
                {
                    //Está mantendo a tendencia de queda
                    if (dadoBE.Anterior.Anterior.PrecoFechamento > dadoBE.Anterior.PrecoFechamento)
                    {
                        listaTendencias[listaTendencias.Count - 3]++;
                        listaTendencias[listaTendencias.Count - 4] += variacao;
                    }
                    else
                    {
                        listaTendencias.AddRange(new List<double>() { 0, 0, variacao, 1 });
                        tendenciaQueda = false;
                    }
                }
                else
                {
                    //Está mantendo a tendencia de subir o valor do ativo
                    if (dadoBE.Anterior.Anterior.PrecoFechamento < dadoBE.Anterior.PrecoFechamento)
                    {
                        listaTendencias[listaTendencias.Count - 1]++;
                        listaTendencias[listaTendencias.Count - 2] += variacao;
                    }
                    else
                    {
                        listaTendencias.AddRange(new List<double>() { variacao, 1, 0, 0 });
                        tendenciaQueda = true;
                    }
                }

                dadoBE.DuracaoTendencias = listaTendencias.Skip(listaTendencias.Count - 12).ToList();
            }

            //Trata a normalização dos valores e adiciona mais 1 índice para cada tendencia q é a média de variação
            foreach (DadoBE dadoBE in listCotacoes)
            {
                int indInsercao = 2;
                //3 análises de tendencias
                for (int i = 0; i < 6; i++)
                {
                    if (dadoBE.DuracaoTendencias[indInsercao - 1] > 0)
                    {
                        double variacaoMedia = dadoBE.DuracaoTendencias[indInsercao - 2] / dadoBE.DuracaoTendencias[indInsercao - 1];
                        dadoBE.DuracaoTendencias.Insert(indInsercao, variacaoMedia);
                        dadoBE.DuracaoTendencias[indInsercao - 1] = dadoBE.DuracaoTendencias[indInsercao - 1] / 10;//Admite que no máximo uma tendencia irá se assegurar por 10 cotações
                    }
                    else
                        dadoBE.DuracaoTendencias.Insert(indInsercao, 0);
                    indInsercao += 3;
                }
            }
            return 0;
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
                        listCotacoes[j].PrecoFechamento /= desdobramento;
                        listCotacoes[j].PrecoMinimo /= desdobramento;
                        listCotacoes[j].PrecoMaximo /= desdobramento;
                        listCotacoes[j].PrecoMedio /= desdobramento;
                    }
                }
            }
        }

        #endregion DESDOBRAMENTO

        //#region NORMALIZAÇÂO

        //public static double NormalizarDado(double dado, string papel)
        //{
        //    double min = Convert.ToDouble(Resources.ResourceManager.GetString("ValorMinimo" + papel.ToUpper()));
        //    double max = Convert.ToDouble(Resources.ResourceManager.GetString("ValorMaximo" + papel.ToUpper()));

        //    return (dado - min) / (max - min);
        //}

        //public static List<double> NormalizarDados(List<double> dados, string papel)
        //{
        //    double min = Convert.ToDouble(Resources.ResourceManager.GetString("ValorMinimo" + papel.ToUpper()));
        //    double max = Convert.ToDouble(Resources.ResourceManager.GetString("ValorMaximo" + papel.ToUpper()));

        //    List<double> dadosList = new List<double>();
        //    dados.ForEach(dado => dadosList.Add((dado - min) / (max - min)));
        //    return dadosList;
        //}

        //public static double DesnormalizarDado(double dado, string papel)
        //{
        //    double min = Convert.ToDouble(Resources.ResourceManager.GetString("ValorMinimo" + papel.ToUpper()));
        //    double max = Convert.ToDouble(Resources.ResourceManager.GetString("ValorMaximo" + papel.ToUpper()));

        //    return dado * (max - min) + min;
        //}

        //public static List<double> DesnormalizarDados(List<double> dados, string papel)
        //{
        //    double min = Convert.ToDouble(Resources.ResourceManager.GetString("ValorMinimo" + papel.ToUpper()));
        //    double max = Convert.ToDouble(Resources.ResourceManager.GetString("ValorMaximo" + papel.ToUpper()));

        //    List<double> dadosList = new List<double>();
        //    dados.ForEach(dado => dadosList.Add(dado * (max - min) + min));
        //    return dadosList;
        //}

        //#endregion NORMALIZAÇÂO
    }
}
