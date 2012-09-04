﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using DataBaseUtils.Model;

namespace DataBaseUtils
{
    public class DataBaseUtils
    {
        static int numeroDivisoesCrossValidation = 8;

        public static List<DadosBE> RecuperarCotacoesAtivo(string papel)
        {
            List<DadosBE> listCotacoes = new List<DadosBE>();
            DadosBE cotacao = null;
            DataTableReader dtr = null;

            try
            {
                dtr = RetornaDados(papel);

                while (dtr.Read())
                {
                    cotacao = new DadosBE();

                    cotacao.Id = (int)dtr["id"];
                    cotacao.NomeReduzido = dtr["nomeresumido"].ToString();
                    cotacao.DataGeracao = (DateTime)dtr["datageracao"];
                    cotacao.PrecoAbertura = (decimal)dtr["precoabertura"];
                    //Removido
                    //cotacao.PrecoAberturaNormalizado = (decimal)dtr["precoaberturaNormalizado"];
                    cotacao.CotacaoDolar = (decimal)dtr["valorDolar"];
                    cotacao.CotacaoDolarNormalizado = (decimal)NormalizarDado(Convert.ToDouble(cotacao.CotacaoDolar), "Dolar");
                    cotacao.EstacaoDoAno = RecuperarEstacaoDoAno(cotacao.DataGeracao);

                    listCotacoes.Add(cotacao);
                }
                TratarDesdobramento(listCotacoes);

                //Adiciona os valores normalizados
                listCotacoes.ForEach(cot => cot.ValorNormalizado = NormalizarDado(Convert.ToDouble(cot.PrecoAbertura), papel));
                //Valor para uso interno
                listCotacoes.ForEach(cot => cot.ValorNormalizadoPrevisto = cot.ValorNormalizado);
                //Valor para uso interno
                listCotacoes.ForEach(cot => cot.CotacaoDolarNormalizadoPrevisto = cot.CotacaoDolarNormalizado);

                //Atribui um valor bollinger de 0 a 1 para a cotação
                PreencherValorBollinger(listCotacoes);
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

        public static void PreencherValorBollinger(List<DadosBE> listCotacoes)
        {
            //Analisaremos periodos de 20 dias
            for (int i = 19; i < listCotacoes.Count; i++)
            {
                //Calcula a média dos 19 dias anteriores mais o dia atual e alimenta a propriedade "MediaMovel" do dado
                listCotacoes[i].MediaMovel = Convert.ToDouble(listCotacoes.Skip(i - 19).Take(20).Sum(cot => cot.PrecoAbertura) / 20);
                //Temos que calcular o desvio padrao da BandaCentral (MediaMovel), portanto isso só é possivel quando tivermos ao menos 20 médias móveis calculadas
                if (i >= 39)
                {
                    //Calculo das bandas http://www.investmax.com.br/iM/content.asp?contentid=660 PS: Fizemos * 2.2 para dar uma margem a mais
                    double bandaSuperior = listCotacoes[i].MediaMovel + 2.3 * Math.Sqrt(Math.Pow(listCotacoes.Skip(i - 19).Take(20).Sum(cot => (double)cot.PrecoAbertura - cot.MediaMovel), 2) / 20);
                    double bandaInferior = listCotacoes[i].MediaMovel - 2.3 * Math.Sqrt(Math.Pow(listCotacoes.Skip(i - 19).Take(20).Sum(cot => (double)cot.PrecoAbertura - cot.MediaMovel), 2) / 20);

                    //Ex: bandaSuperior = 10, bandaInferior = 2, cotacao = 4.8567, temos: (4.8567 - 2) * 1 / (10-2) = 0.3570875
                    listCotacoes[i].ValorBollinger = 1 / (bandaSuperior - bandaInferior) * ((double)listCotacoes[i].PrecoAbertura - bandaInferior);
                }


                //Desvio Padrao - http://pt.wikipedia.org/wiki/Desvio_padr%C3%A3o
            }

            //Seta o valor médio para as cotações em que não foi possivel fazer a analise bollinger
            listCotacoes.Take(40).ToList().ForEach(cot => cot.ValorBollinger = 0.5);
        }

        public static double RecuperarEstacaoDoAno(DateTime data)
        {
            return data.Subtract(new DateTime(data.Year, 1, 1)).TotalDays / 365.0;
            //dynamic datasEstacoes = new
            //{
            //    dataInicialOutono = new DateTime(data.Year, 3, 20),
            //    dataFinalOutono = new DateTime(data.Year, 6, 19),
            //    dataInicialInverno = new DateTime(data.Year, 6, 20),
            //    dataFinalInverno = new DateTime(data.Year, 9, 21),
            //    dataInicialPrimavera = new DateTime(data.Year, 9, 22),
            //    dataFinalPrimavera = new DateTime(data.Year, 12, 21),
            //    dataInicialVerao = new DateTime(data.Year, 12, 22),
            //    dataFinalVerao = new DateTime(data.Year + 1, 3, 19)
            //};

            ////OUTONO
            //if (DateTime.Compare(data, datasEstacoes.dataInicialOutono) >= 0 && DateTime.Compare(data, datasEstacoes.dataFinalOutono) <= 0)
            //{
            //    return 0;
            //}
            ////INVERNO
            //else if (DateTime.Compare(data, datasEstacoes.dataInicialInverno) >= 0 && DateTime.Compare(data, datasEstacoes.dataFinalInverno) <= 0)
            //{

            //}
            ////PRIMAVERA
            //else if (DateTime.Compare(data, datasEstacoes.dataInicialPrimavera) >= 0 && DateTime.Compare(data, datasEstacoes.dataFinalPrimavera) <= 0)
            //{

            //}
            ////VERÃO
            //else if (DateTime.Compare(data, datasEstacoes.dataInicialVerao) >= 0 && DateTime.Compare(data, datasEstacoes.dataFinalVerao) <= 0)
            //{

            //}
        }

        #region METODOS DE SELECAO DE DADOS

        public static List<DadosBE> SelecionarUltimosNDadosAntesDaDataDaPrevisao(List<DadosBE> dadosBE, DateTime dtPrevisao, int n)
        {
            List<DadosBE> dadosFiltradosDataInicial = dadosBE.Where(dado => dado.DataGeracao < dtPrevisao.Date).ToList();
            return dadosFiltradosDataInicial.Skip(dadosFiltradosDataInicial.Count - n).ToList();
        }

        public static List<Treinamento> SelecionarTreinamentos_V2(List<DadosBE> dadosBE, int janelaEntrada, int n)
        {
            if (n <= 0)
                n = 1;

            List<Treinamento> treinamentos = new List<Treinamento>();
            for (int divisao = 0; divisao < numeroDivisoesCrossValidation; divisao++)
            {
                for (int i = dadosBE.Count / numeroDivisoesCrossValidation * divisao; i < dadosBE.Count / numeroDivisoesCrossValidation * (divisao + 1); i += n)
                {
                    //i deve ter valor menor que o numero de elementos de dadosBE - a quantidade de elementos que serão selecionados (janelaentrada + 1[janela saida])
                    if (i >= dadosBE.Count - janelaEntrada)
                        break;
                    Treinamento treinamento = new Treinamento();
                    treinamento.DivisaoCrossValidation = divisao;
                    List<DadosBE> dadosBEInput = dadosBE.Skip(i).Take(janelaEntrada).ToList();

                    treinamento.Input = SelecionarInput_V2(dadosBEInput);

                    DadosBE dadoBEOutput = dadosBE.Skip(i + janelaEntrada).First();
                    treinamento.Output = new List<double>() { dadoBEOutput.ValorNormalizado, (double)dadoBEOutput.CotacaoDolarNormalizado };
                    treinamentos.Add(treinamento);
                }
            }

            //Comentado por enquanto //Pode não haver dados suficientes para terminar de preencher o output, gerando um output com menos dados do que o solicitado
            return treinamentos;     //.Where(trein => trein.Output.Count == janelaSaida).ToList();
        }

        /// <summary>
        /// Através dos dados BE que serão utilizados no INPUT, monta o array de double que será passado para a rede neural
        /// </summary>
        /// <param name="dadosBEInput"></param>
        /// <param name="selecionarDosDadosPrevistos">true se os dados do input devem ser selecionados dos valores previstos</param>
        /// <returns></returns>
        public static List<double> SelecionarInput_V2(List<DadosBE> dadosBEInput, bool selecionarDosDadosPrevistos = false)
        {
            //Adiciona as cotações do ativo
            List<double> input = null;

            if (selecionarDosDadosPrevistos)
                input = dadosBEInput.Select(dadoBE => (double)dadoBE.ValorNormalizadoPrevisto).ToList();
            else
                input = dadosBEInput.Select(dadoBE => (double)dadoBE.ValorNormalizado).ToList();

            //Adiciona a estação do ano
            input.Add(dadosBEInput.Last().EstacaoDoAno);
            //Adiciona o valor bollinger
            input.Add(dadosBEInput.Last().ValorBollinger);

            //Adiciona as cotações do dolar
            if (selecionarDosDadosPrevistos)
                input.Add((double)dadosBEInput.First().CotacaoDolarNormalizadoPrevisto);
            else
                input.Add((double)dadosBEInput.First().CotacaoDolarNormalizado);

            //A primeira cotação ja foi adicionada e a ultima será adicionada em seguida, por isso começamos em 5 e terminamos em COUNT() - 5
            for (int indDadoBE = 5; indDadoBE < dadosBEInput.Count - 5; indDadoBE++)
            {
                if (selecionarDosDadosPrevistos)
                    input.Add((double)dadosBEInput[indDadoBE].CotacaoDolarNormalizadoPrevisto);
                else
                    input.Add((double)dadosBEInput[indDadoBE].CotacaoDolarNormalizado);
            }

            if (selecionarDosDadosPrevistos)
                input.Add((double)dadosBEInput.Last().CotacaoDolarNormalizadoPrevisto);
            else
                input.Add((double)dadosBEInput.Last().CotacaoDolarNormalizado);

            return input;
        }

        /// <summary>
        /// Retorna uma lista de Treinamento com seus devidos inputs e outputs e o seu shift correspondente
        /// </summary>
        /// <param name="dados"></param>
        /// <param name="janelaEntrada">tamanho do input</param>
        /// <param name="janelaSaida">tamanho do output</param>
        /// <param name="n">quantidade de dados que serão pulados para cada iteração de treinamento (minimo 1)</param>
        /// <returns></returns>
        [Obsolete("É utilizado pela versao 1")]
        public static List<Treinamento> SelecionarTreinamentos(List<double> dados, int janelaEntrada, int janelaSaida, int n)
        {
            if (n <= 0)
                n = 1;

            List<Treinamento> treinamentos = new List<Treinamento>();
            for (int divisao = 0; divisao < numeroDivisoesCrossValidation; divisao++)
            {
                for (int i = dados.Count / numeroDivisoesCrossValidation * divisao; i < dados.Count / numeroDivisoesCrossValidation * (divisao + 1); i += n)
                {
                    Treinamento treinamento = new Treinamento();
                    treinamento.DivisaoCrossValidation = divisao;
                    treinamento.Input = dados.Skip(i).Take(janelaEntrada).ToList();
                    treinamento.Output = dados.Skip(i + janelaEntrada).Take(janelaSaida).ToList();
                    treinamentos.Add(treinamento);
                }
            }

            //Pode não haver dados suficientes para terminar de preencher o output, gerando um output com menos dados do que o solicitado
            return treinamentos.Where(trein => trein.Output.Count == janelaSaida).ToList();
        }

        /// <summary>
        /// Retorna uma lista de KeyValuePair, onde a chave são os valores de input e o seu valor correspondente são os valores de output
        /// </summary>
        /// <param name="dados"></param>
        /// <param name="janelaEntrada">tamanho do input</param>
        /// <param name="janelaSaida">tamanho do output</param>
        /// <param name="considerarSaidasComoEntradas">numero de dias que serao pulados</param>
        /// <returns></returns>
        [Obsolete("É utilizado pela versao 1")]
        public static List<KeyValuePair<double[], double[]>> SelecionarCotacoesPorJanelamentoPulandoNDias(List<double> dados, int janelaEntrada, int janelaSaida, int n)
        {
            if (n <= 0)
                n = 1;
            /*Cria um mapeamento de entradas para saida com o janelamento informado*/
            List<KeyValuePair<double[], double[]>> dadosPorJanelamento = new List<KeyValuePair<double[], double[]>>();
            for (int i = 0; i < dados.Count - (janelaEntrada + janelaSaida); i += n)
            {
                dadosPorJanelamento.Add(new KeyValuePair<double[], double[]>(dados.Skip(i).Take(janelaEntrada).ToArray(), dados.Skip(i + janelaEntrada).Take(janelaSaida).ToArray()));
            }
            /*Cria um mapeamento de entradas para saida com o janelamento informado*/

            return dadosPorJanelamento;
        }

        #endregion METODOS DE SELECAO DE DADOS

        #region DESDOBRAMENTO

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

        #endregion DESDOBRAMENTO

        #region NORMALIZAÇÂO

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

        #endregion NORMALIZAÇÂO

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

        #region RN_V3

        /// <summary>
        /// Explicação em: Redes\03_09_RedeNeural_PrevisaoFinanceira_v3\Metodos_Indices\Percentual_Volume_Negociacoes.txt
        /// </summary>
        /// <param name="listCotacoes"></param>
        /// <returns></returns>
        public static void PreencherPercentualTotalNegociacoes(List<DadosBE> listCotacoes)
        {
            //Deve ser maior do que 20
            int nDiasAnalisePercentual = 30;
            //Alimenta os 29 primeiro dias com o valor intermediário, que não deve influenciar muito a rede neural
            listCotacoes.Take(nDiasAnalisePercentual - 1).ToList().ForEach(cot => cot.PercentualTotalNegociacoes = 0.5);
            for (int i = 0; i < listCotacoes.Count - nDiasAnalisePercentual; i++)
            {
                List<DadosBE> cotacoesAnaliseAtual = listCotacoes.Skip(i).Take(nDiasAnalisePercentual).ToList();

                double min = Convert.ToDouble(cotacoesAnaliseAtual.OrderBy(cot => cot.TotalNegociacoes).Take(3).Average(cot => cot.TotalNegociacoes));
                double max = Convert.ToDouble(cotacoesAnaliseAtual.OrderByDescending(cot => cot.TotalNegociacoes).Take(3).Average(cot => cot.TotalNegociacoes));
                double med = Convert.ToDouble(cotacoesAnaliseAtual.Average(cot => cot.TotalNegociacoes));
                double hoje = Convert.ToDouble(cotacoesAnaliseAtual.Last().TotalNegociacoes);

                if (hoje < med)
                {
                    double aux = 0.5 / (med - min);
                    cotacoesAnaliseAtual.Last().PercentualTotalNegociacoes = 0.5 - ((med - hoje) * aux);
                }
                else if (hoje > med)
                {
                    double aux = 0.5 / (max - med);
                    cotacoesAnaliseAtual.Last().PercentualTotalNegociacoes = 0.5 + ((hoje - med) * aux);
                }
                else
                    cotacoesAnaliseAtual.Last().PercentualTotalNegociacoes = 0.5;
            }
        }

        /// <summary>
        /// Explicação em: Redes\03_09_RedeNeural_PrevisaoFinanceira_v3\Metodos_Indices\Medias_Moveis.txt
        /// </summary>
        /// <param name="listCotacoes"></param>
        /// <returns></returns>
        public static void PreencherPontuacaoMediaMovel(List<DadosBE> listCotacoes)
        {
            List<double> cotacoes = listCotacoes.Select(cot => cot.ValorNormalizado).ToList();
            int m20 = 20;
            int m10 = 10;
            int m5 = 5;
            for (int indAtual = m20; indAtual < listCotacoes.Count; indAtual++)
            {
                List<double> medM20 = new List<double>();
                List<double> medM10 = new List<double>();
                List<double> medM5 = new List<double>();

                //preenche m20
                for (int contadorMedMovel = 0; contadorMedMovel < m20; contadorMedMovel++)
                {
                    int skip = indAtual - m20;
                    List<DadosBE> cotacoesAnaliseAtual = listCotacoes.Skip(skip).Take(m20).ToList();
                    medM20.Add(cotacoes.Average());
                }

                //preenche m10
                for (int contadorMedMovel = 0; contadorMedMovel < m10; contadorMedMovel++)
                {
                    int skip = indAtual - m10;
                    List<DadosBE> cotacoesAnaliseAtual = listCotacoes.Skip(skip).Take(m10).ToList();
                    medM10.Add(cotacoes.Average());
                }

                //preenche m50
                for (int contadorMedMovel = 0; contadorMedMovel < m5; contadorMedMovel++)
                {
                    int skip = indAtual - m5;
                    List<DadosBE> cotacoesAnaliseAtual = listCotacoes.Skip(skip).Take(m5).ToList();
                    medM5.Add(cotacoes.Average());
                }

                //Seleciona apenas o tamanho de m5 (o menor)
                medM20 = medM20.Skip(m20 - m5).ToList();
                //Seleciona apenas o tamanho de m5 (o menor)
                medM10 = medM10.Skip(m10 - m5).ToList();

                //Para verificar todos os casos, utilizaremos o primeiro e o ultimo dado de cada média móvel
                double pontuacaoMediasMoveis = 0;

                //Verifica o caso 1: (0.5 pontos) - M5 deve cruzar M10 para cima
                if (medM5.First() < medM10.First() && medM5.Last() > medM10.Last())
                {
                    pontuacaoMediasMoveis += 0.5;
                }
                //Verifica o caso 2: (0.25 pontos) - M5 deve cruzar M20
                if ((medM5.First() < medM20.First() && medM5.Last() > medM20.Last())
                || (medM5.First() > medM20.First() && medM5.Last() < medM20.Last()))
                {
                    pontuacaoMediasMoveis += 0.25;
                }
                //Verifica o caso 3: (0.25 pontos) - M10 deve cruzar M20
                if ((medM10.First() < medM20.First() && medM10.Last() > medM20.Last())
                || (medM10.First() > medM20.First() && medM10.Last() < medM20.Last()))
                {
                    pontuacaoMediasMoveis += 0.25;
                }

                //Atualiza o valor da pontuação da média móvel
                listCotacoes[indAtual].PontuacaoMediasMoveis = pontuacaoMediasMoveis;
            }
        }

        #endregion RN_V3
    }
}
