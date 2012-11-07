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
        public DadoBE Anterior { get; private set; }
        public DadoBE Proximo { get; private set; }

        #endregion PROPRIEDADES

        #region INDICES
        private double MediaMovel;
        public List<double> ValorBollinger { get; set; }

        public List<double> DuracaoTendencias { get; set; }
        public List<double> AnaliseMediaMovelSimples5Dias { get; set; }

        public List<double> AnaliseWilliams_Percent_R_14P { get; set; }
        public List<double> AnaliseWilliams_Percent_R_28P { get; set; }

        public List<double> AnaliseArron_Up_Down { get; set; }
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
            dadosIgnorarInicio = Math.Max(dadosIgnorarInicio, PreencherDuracaoTendencias(listCotacoes));
            dadosIgnorarInicio = Math.Max(dadosIgnorarInicio, PreencherAnaliseMediaMovel5Dias(listCotacoes));
            dadosIgnorarInicio = Math.Max(dadosIgnorarInicio, PreencherAnaliseWilliams_Percent_R_14P(listCotacoes));
            dadosIgnorarInicio = Math.Max(dadosIgnorarInicio, PreencherAnaliseWilliams_Percent_R_28P(listCotacoes));
            dadosIgnorarInicio = Math.Max(dadosIgnorarInicio, PreencherAnaliseArron_Up_Down(listCotacoes));

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

        //Existe uma tendência de venda de um ativo quando seu preço cruzar de cima para baixo a sua média móvel. Da mesma forma, existe uma tendência de compra quando seu preço cruzar de baixo para cima a sua média móvel. 

        public static int PreencherAnaliseMediaMovel5Dias(List<DadoBE> listCotacoes)
        {
            int n = 5;
            listCotacoes.Take(n).ToList().ForEach(dado => dado.AnaliseMediaMovelSimples5Dias = new List<double>() { 0, 0 });

            foreach (DadoBE dadoBE in listCotacoes.Skip(n))
            {
                double mediaMoveld_menos_2 = ValorMediaMovel(dadoBE.Anterior, n);
                double mediaMoveld_menos_1 = ValorMediaMovel(dadoBE, n);
                if ((dadoBE.Anterior.Anterior.PrecoFechamento < mediaMoveld_menos_2)
                 && (dadoBE.Anterior.PrecoFechamento > mediaMoveld_menos_1))
                {
                    //Comprar..
                    dadoBE.AnaliseMediaMovelSimples5Dias = new List<double>() { 1, 0 };
                }
                else if ((dadoBE.Anterior.Anterior.PrecoFechamento > mediaMoveld_menos_2)
                      && (dadoBE.Anterior.PrecoFechamento < mediaMoveld_menos_1))
                {
                    //Vender
                    dadoBE.AnaliseMediaMovelSimples5Dias = new List<double>() { 0, 1 };
                }
                else
                {
                    dadoBE.AnaliseMediaMovelSimples5Dias = new List<double>() { 0, 0 };
                    ////Acompanhar a tendencia mais proxima, se houver uma nos 4 dias anteriores..
                    //DadoBE dadoAnterior = dadoBE.Anterior;
                    //for (int i = 0; i < 4; i++)
                    //{
                    //    if (dadoAnterior.AnaliseMediaMovelSimples5Dias[0] == 1)
                    //    {
                    //        dadoBE.AnaliseMediaMovelSimples5Dias = new List<double>() { 1, 0 };
                    //        break;
                    //    }
                    //    else if (dadoAnterior.AnaliseMediaMovelSimples5Dias[1] == 1)
                    //    {
                    //        dadoBE.AnaliseMediaMovelSimples5Dias = new List<double>() { 0, 1 };
                    //        break;
                    //    }
                    //    dadoAnterior = dadoAnterior.Anterior;
                    //}
                    //if (dadoBE.AnaliseMediaMovelSimples5Dias == null)
                    //{
                    //    dadoBE.AnaliseMediaMovelSimples5Dias = new List<double>() { 0, 0 };
                    //}
                }
            }
            return n;
        }

        public static int PreencherAnaliseWilliams_Percent_R_14P(List<DadoBE> listCotacoes)
        {
            int n = 14;

            foreach (DadoBE dadoBE in listCotacoes.Skip(n))
            {
                double williams = ValorWiliams_Percent_R(dadoBE, n);
                if (williams <= 100 && williams >= 80)
                {
                    //Comprar..
                    dadoBE.AnaliseWilliams_Percent_R_14P = new List<double>() { 1, 0 };
                }
                else if (williams <= 20 && williams >= 0)
                {
                    //Vender
                    dadoBE.AnaliseWilliams_Percent_R_14P = new List<double>() { 0, 1 };
                }
                else
                {
                    dadoBE.AnaliseWilliams_Percent_R_14P = new List<double>() { 0, 0 };
                }
                //dadoBE.AnaliseWilliams_Percent_R_14P = new List<double>() { williams < 0 ? 0 : (williams > 100 ? 100 : williams) };
            }
            return n;
        }

        public static int PreencherAnaliseWilliams_Percent_R_28P(List<DadoBE> listCotacoes)
        {
            int n = 28;

            foreach (DadoBE dadoBE in listCotacoes.Skip(n))
            {
                double williams = ValorWiliams_Percent_R(dadoBE, n);
                if (williams <= 100 && williams >= 80)
                {
                    //Comprar..
                    dadoBE.AnaliseWilliams_Percent_R_28P = new List<double>() { 1, 0 };
                }
                else if (williams <= 20 && williams >= 0)
                {
                    //Vender
                    dadoBE.AnaliseWilliams_Percent_R_28P = new List<double>() { 0, 1 };
                }
                else
                {
                    dadoBE.AnaliseWilliams_Percent_R_28P = new List<double>() { 0, 0 };
                }
            }
            return n;
        }

        public static int PreencherAnaliseArron_Up_Down(List<DadoBE> listCotacoes)
        {
            int n = 10;

            foreach (DadoBE dadoBE in listCotacoes.Skip(n))
            {
                double[] arronUpDown = ValorArron_Up_Down(dadoBE, n);
                //Verificar se existe tendencia nos valores arron
                if (arronUpDown[0] > 0.7 || arronUpDown[1] > 0.7)
                {
                    //verificar se o mercado está indeciso: arronUp perto de arronDown
                    //Transforma em um valor entre -100 e +100
                    double difUpDown = arronUpDown[1] - arronUpDown[0] * 100;
                    //Mercado indeciso:
                    if (Math.Abs(difUpDown) < 50)
                    {
                        dadoBE.AnaliseArron_Up_Down = new List<double>() { 0, 0 };
                    }
                    else
                    {
                        if (arronUpDown[0] > arronUpDown[1])
                        {
                            dadoBE.AnaliseArron_Up_Down = new List<double>() { 1, 0 };
                        }
                        else
                        {
                            dadoBE.AnaliseArron_Up_Down = new List<double>() { 0, 1 };
                        }
                    }
                }
                else
                    dadoBE.AnaliseArron_Up_Down = new List<double>() { 0, 0 };
                //if (arronUpDown <= 100 && williams >= 80)
                //{
                //    //Comprar..
                //    dadoBE.AnaliseWilliams_Percent_R_28P = new List<double>() { 1, 0 };
                //}
                //else if (williams <= 20 && williams >= 0)
                //{
                //    //Vender
                //    dadoBE.AnaliseWilliams_Percent_R_28P = new List<double>() { 0, 1 };
                //}
                //else
                //{
                //    dadoBE.AnaliseWilliams_Percent_R_28P = new List<double>() { 0, 0 };
                //}
            }
            return n;
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

        public static double ValorWiliams_Percent_R(DadoBE dadoBE, int periodo)
        {
            //http://stockcharts.com/school/doku.php?id=chart_school:technical_indicators:williams_r
            DadoBE aux = dadoBE;
            List<DadoBE> dadosBE = new List<DadoBE>();
            while (aux.Anterior != null && periodo > 0)
            {
                dadosBE.Add(aux.Anterior);
                periodo--;
                aux = aux.Anterior;
            }

            if (periodo > 0)
                return 0;
            else
            {
                double maior = dadosBE.Max(dado => dado.PrecoMaximo);
                double menor = dadosBE.Min(dado => dado.PrecoMinimo);
                double fechamentoAtual = dadoBE.Anterior.PrecoFechamento;
                double williams_R = (maior - fechamentoAtual) / (maior - menor) * 100;
                return williams_R;
            }
        }

        public static double[] ValorArron_Up_Down(DadoBE dadoBE, int periodo)
        {
            int periodoAux = periodo;
            //http://www.grafbolsa.com/help/maisit.html
            //http://apligraf.com.br/suporte/estudos/aroon/
            DadoBE aux = dadoBE;
            List<DadoBE> dadosBE = new List<DadoBE>();
            while (aux.Anterior != null && periodoAux > 0)
            {
                dadosBE.Add(aux.Anterior);
                periodoAux--;
                aux = aux.Anterior;
            }

            if (periodoAux > 0)
                return new double[] { 0, 0 };
            else
            {
                int indMaior = dadosBE.IndexOf(dadosBE.First(d => d.PrecoFechamento == (dadosBE.Max(dado => dado.PrecoFechamento))));
                int indMenor = dadosBE.IndexOf(dadosBE.First(d => d.PrecoFechamento == (dadosBE.Min(dado => dado.PrecoFechamento))));
                double arronUp = 1.0 / (periodo - 1) * indMaior;
                double arronDown = 1.0 / (periodo - 1) * indMenor;
                return new double[] { arronUp, arronDown };
            }
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

        public long VolumeNegociacao { get; set; }
    }
}
