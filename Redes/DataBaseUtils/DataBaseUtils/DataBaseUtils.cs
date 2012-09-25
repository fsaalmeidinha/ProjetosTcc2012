using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using DataBaseUtils.Model;
using System.Reflection;

namespace DataBaseUtils
{
    public class DataBaseUtils
    {
        //Número de treinamentos que devem ser ignorados pois não tem todos os índices calculados
        static int _treinamentosIniciaisIgnorar;
        static int treinamentosIniciaisIgnorar
        {
            get { return _treinamentosIniciaisIgnorar; }
            set
            {
                if (value > _treinamentosIniciaisIgnorar)
                    _treinamentosIniciaisIgnorar = value;
            }
        }

        public DataBaseUtils()
        {
            //"PercentualCrescimentoValorAtivo", "ValorNormalizado", "ValorBollinger", "Pontuacao3MediasMoveis", "PercentualTotalNegociacoesMediaNDias", "PercentualTotalNegociacoes", "PercentualCrescimentoDolar", "PercentualCrescimentoValorAtivoMediaNDias", "PercentualDesviosPadroesEmRelacaoNDias", "DiaSemana", "PercentualValorAtivo_Max_Min_Med"
        }
        static int numeroDivisoesCrossValidation = 8;

        /*
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
        */

        public static List<DadosBE> RecuperarCotacoesAtivo(string papel)
        {
            List<DadosBE> listCotacoes = new List<DadosBE>();
            DataTableReader dtr = null;

            try
            {
                dtr = RetornaDados(papel);

                while (dtr.Read())
                {
                    DadosBE cotacao = new DadosBE();

                    cotacao.Id = (int)dtr["id"];
                    cotacao.NomeReduzido = dtr["nomeresumido"].ToString();
                    cotacao.PrecoAbertura = (decimal)dtr["precoabertura"];
                    cotacao.DataGeracao = (DateTime)dtr["datageracao"];
                    cotacao.CotacaoDolar = (decimal)dtr["valorDolar"];
                    cotacao.PrecoMaximo = (decimal)dtr["PRECOMAX"];
                    cotacao.PrecoMinimo = (decimal)dtr["PRECOMIN"];
                    cotacao.PrecoMedio = (decimal)dtr["PRECOMED"];
                    cotacao.TotalNegociacoes = (int)dtr["TOTALNEGO"];
                    cotacao.QuantidadeTotalNegociacoes = (int)dtr["QUANTIDADETOTALNEGO"];
                    cotacao.ValorTotalNegociacoes = (decimal)dtr["VALORTOTALNEGO"];

                    //Removido
                    //cotacao.PrecoAberturaNormalizado = (decimal)dtr["precoaberturaNormalizado"];
                    cotacao.CotacaoDolarNormalizado = (decimal)NormalizarDado(Convert.ToDouble(cotacao.CotacaoDolar), "Dolar");
                    cotacao.EstacaoDoAno = RecuperarEstacaoDoAno(cotacao.DataGeracao);

                    listCotacoes.Add(cotacao);
                }
                //Ordena pela data
                listCotacoes = listCotacoes.OrderBy(cot => cot.DataGeracao).ToList();
                TratarDesdobramento(listCotacoes);

                //Adiciona os valores normalizados
                listCotacoes.ForEach(cot => cot.ValorNormalizado = NormalizarDado(Convert.ToDouble(cot.PrecoAbertura), papel));
                //Valor para uso interno
                listCotacoes.ForEach(cot => cot.ValorNormalizadoPrevisto = cot.ValorNormalizado);
                //Valor para uso interno
                listCotacoes.ForEach(cot => cot.CotacaoDolarNormalizadoPrevisto = cot.CotacaoDolarNormalizado);

                //Preenche os valores dos dias seguintes
                for (int i = 0; i < listCotacoes.Count - 1; i++)
                {
                    listCotacoes[i].PrecoFechamento = Convert.ToDouble(listCotacoes[i + 1].PrecoAbertura);
                    listCotacoes[i].PrecoFechamentoNormalizado = Convert.ToDouble(listCotacoes[i + 1].ValorNormalizado);
                }
                //Preenche os valores dos dias seguintes
                for (int i = 0; i < listCotacoes.Count - 1; i++)
                {
                    listCotacoes[i].ValorDiaSeguinte = Convert.ToDouble(listCotacoes[i + 1].PrecoAbertura);
                    listCotacoes[i].ValorNormalizadoDiaSeguinte = Convert.ToDouble(listCotacoes[i + 1].ValorNormalizado);
                }

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
                listCotacoes[i].MediaMovel = Convert.ToDouble(listCotacoes.Skip(i - 19).Take(20).Sum(cot => cot.PrecoFechamento) / 20);
                //Temos que calcular o desvio padrao da BandaCentral (MediaMovel), portanto isso só é possivel quando tivermos ao menos 20 médias móveis calculadas
                if (i >= 39)
                {
                    //Calculo das bandas http://www.investmax.com.br/iM/content.asp?contentid=660 PS: Fizemos * 5.0 para dar uma margem a mais
                    double bandaSuperior = listCotacoes[i].MediaMovel + 5 * Math.Sqrt(Math.Pow(listCotacoes.Skip(i - 19).Take(20).Sum(cot => (double)cot.PrecoFechamento - cot.MediaMovel), 2) / 20);
                    double bandaInferior = listCotacoes[i].MediaMovel - 5 * Math.Sqrt(Math.Pow(listCotacoes.Skip(i - 19).Take(20).Sum(cot => (double)cot.PrecoFechamento - cot.MediaMovel), 2) / 20);

                    //Ex: bandaSuperior = 10, bandaInferior = 2, cotacao = 4.8567, temos: (4.8567 - 2) * 1 / (10-2) = 0.3570875
                    listCotacoes[i].ValorBollinger = 1 / (bandaSuperior - bandaInferior) * ((double)listCotacoes[i].PrecoFechamento - bandaInferior);
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
                        listCotacoes[j].PrecoMinimo /= desdobramento;
                        listCotacoes[j].PrecoMaximo /= desdobramento;
                        listCotacoes[j].PrecoMedio /= desdobramento;
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
        /// Explicação em: Redes\03_09_RedeNeural_PrevisaoFinanceira_v3\Metodos_Indices\Pontuacao3MediasMoveis.doc
        /// </summary>
        /// <param name="listCotacoes"></param>
        /// <returns></returns>
        public static void PreencherPontuacao3MediasMoveis(List<DadosBE> listCotacoes)
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
                    int skip = (indAtual + contadorMedMovel) - m20;
                    List<double> cotacoesAnaliseAtual = cotacoes.Skip(skip).Take(m20).ToList();
                    medM20.Add(cotacoesAnaliseAtual.Average());
                }

                //preenche m10
                for (int contadorMedMovel = 0; contadorMedMovel < m10; contadorMedMovel++)
                {
                    int skip = (indAtual + contadorMedMovel) - m10;
                    List<double> cotacoesAnaliseAtual = cotacoes.Skip(skip).Take(m10).ToList();
                    medM10.Add(cotacoesAnaliseAtual.Average());
                }

                //preenche m50
                for (int contadorMedMovel = 0; contadorMedMovel < m5; contadorMedMovel++)
                {
                    int skip = (indAtual + contadorMedMovel) - m5;
                    List<double> cotacoesAnaliseAtual = cotacoes.Skip(skip).Take(m5).ToList();
                    medM5.Add(cotacoesAnaliseAtual.Average());
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
                listCotacoes[indAtual].Pontuacao3MediasMoveis = pontuacaoMediasMoveis;
            }
        }

        /// <summary>
        /// Explicação em: Redes\03_09_RedeNeural_PrevisaoFinanceira_v3\Metodos_Indices\PercentualTotalNegociacoesMediaNDias.doc
        /// </summary>
        /// <param name="listCotacoes"></param>
        /// <returns></returns>
        public static void PreencherPercentualTotalNegociacoesMediaNDias(List<DadosBE> listCotacoes)
        {
            //Deve ser maior do que 10
            int nDiasAnalisePercentual = 15;
            //Alimenta os 14 primeiro dias com o valor intermediário, que não deve influenciar muito a rede neural (os 14 primeiros dias não podem ser calculados)
            listCotacoes.Take(nDiasAnalisePercentual - 1).ToList().ForEach(cot => cot.PercentualTotalNegociacoesMediaNDias = 0.5);
            for (int i = 0; i < listCotacoes.Count - nDiasAnalisePercentual; i++)
            {
                List<DadosBE> cotacoesAnaliseAtual = listCotacoes.Skip(i).Take(nDiasAnalisePercentual).ToList();

                double min = Convert.ToDouble(cotacoesAnaliseAtual.OrderBy(cot => cot.QuantidadeTotalNegociacoes).Take(3).Average(cot => cot.QuantidadeTotalNegociacoes));
                double max = Convert.ToDouble(cotacoesAnaliseAtual.OrderByDescending(cot => cot.QuantidadeTotalNegociacoes).Take(3).Average(cot => cot.QuantidadeTotalNegociacoes));
                double med = Convert.ToDouble(cotacoesAnaliseAtual.Average(cot => cot.QuantidadeTotalNegociacoes));
                double hoje = Convert.ToDouble(cotacoesAnaliseAtual.Last().QuantidadeTotalNegociacoes);

                if (hoje < med)
                {
                    //0.3 ao invés de 0.5 para caso o valor atual seja maior do que a média
                    double aux = 0.3 / (med - min);
                    cotacoesAnaliseAtual.Last().PercentualTotalNegociacoesMediaNDias = 0.5 - ((med - hoje) * aux);
                }
                else if (hoje > med)
                {
                    //0.3 ao invés de 0.5 para caso o valor atual seja maior do que a média
                    double aux = 0.3 / (max - med);
                    cotacoesAnaliseAtual.Last().PercentualTotalNegociacoesMediaNDias = 0.5 + ((hoje - med) * aux);
                }
                else
                    cotacoesAnaliseAtual.Last().PercentualTotalNegociacoesMediaNDias = 0.5;
            }
        }

        /// <summary>
        /// Explicação em: Redes\03_09_RedeNeural_PrevisaoFinanceira_v3\Metodos_Indices\PercentualTotalNegociacoes.doc
        /// </summary>
        /// <param name="listCotacoes"></param>
        /// <returns></returns>
        public static void PreencherPercentualTotalNegociacoes(List<DadosBE> listCotacoes)
        {
            if (listCotacoes.Count == 0)
                return;

            //Numero maximo de vezes que o dia anterior pode ser maior ou menor do que o dia de hoje. Por ex: ontem: 100, hoje: 250. Portanto hoje é 1.5 vezes maior do que ontem..
            int maxVezes = 10;

            //Alimenta o primeiro dia com 0.5 que não deve influenciar muito na RN (o primeiro dia nao pode ser calculado)
            listCotacoes.First().PercentualTotalNegociacoes = 0.5;
            for (int i = 1; i < listCotacoes.Count; i++)
            {
                if (listCotacoes[i].QuantidadeTotalNegociacoes == 0 || listCotacoes[i - 1].QuantidadeTotalNegociacoes == 0)
                    continue;

                //Verifica qual é maior
                if (listCotacoes[i].QuantidadeTotalNegociacoes > listCotacoes[i - 1].QuantidadeTotalNegociacoes)
                {
                    double mult = (double)listCotacoes[i].QuantidadeTotalNegociacoes / (double)listCotacoes[i - 1].QuantidadeTotalNegociacoes;
                    mult -= 1;//Calcula quantas vezes hoje é maior do que ontem
                    //Multiplica 0.5 dividido pela quantidade de vezes que um valor pode ser maior do que o outro ('maxVezes') pela quantidade de vezes que hoje realmente é maior do que ontem. Soma esse valor a 0.5.
                    listCotacoes[i].PercentualTotalNegociacoes = 0.5 + (0.5 / maxVezes * mult);
                }
                else
                {
                    double mult = (double)listCotacoes[i - 1].QuantidadeTotalNegociacoes / (double)listCotacoes[i].QuantidadeTotalNegociacoes;
                    mult -= 1;//Calcula quantas vezes ontem é maior do que hoje
                    //Multiplica 0.5 dividido pela quantidade de vezes que um valor pode ser maior do que o outro ('maxVezes') pela quantidade de vezes que hoje realmente é maior do que ontem. Subtrai esse valor de 0.5.
                    listCotacoes[i].PercentualTotalNegociacoes = 0.5 - (0.5 / maxVezes * mult);
                }
            }
        }

        /// <summary>
        /// Explicação em: Redes\03_09_RedeNeural_PrevisaoFinanceira_v3\Metodos_Indices\PercentualCrescimentoDolar.doc
        /// </summary>
        /// <param name="listCotacoes"></param>
        public static void PreencherPercentualCrescimentoDolar(List<DadosBE> listCotacoes)
        {
            if (listCotacoes.Count == 0)
                return;

            //Valor máximo que o dólar pode destoar do dia anterior, 1.3 vezes maior ou 1.3 vezes menor
            double maxCrescimento = 0.3;

            //O primeiro valor não pode ser preenchido, portanto é setado como 0.5
            listCotacoes.First().PercentualCrescimentoDolar = 0.5;
            for (int indDolar = 1; indDolar < listCotacoes.Count; indDolar++)
            {
                if (listCotacoes[indDolar].CotacaoDolarNormalizado == 0 || listCotacoes[indDolar - 1].CotacaoDolarNormalizado == 0)
                    continue;

                if (listCotacoes[indDolar].CotacaoDolarNormalizado > listCotacoes[indDolar - 1].CotacaoDolarNormalizado)
                {
                    double valCresc = Convert.ToDouble(listCotacoes[indDolar].CotacaoDolarNormalizado / listCotacoes[indDolar - 1].CotacaoDolarNormalizado);
                    valCresc -= 1;
                    listCotacoes[indDolar].PercentualCrescimentoDolar = 0.5 + (valCresc / maxCrescimento * 0.5);
                }
                else
                {
                    double valCresc = Convert.ToDouble(listCotacoes[indDolar - 1].CotacaoDolarNormalizado / listCotacoes[indDolar].CotacaoDolarNormalizado);
                    valCresc -= 1;
                    listCotacoes[indDolar].PercentualCrescimentoDolar = 0.5 - (valCresc / maxCrescimento * 0.5);
                }
            }
        }

        /// <summary>
        /// Explicação em: Redes\03_09_RedeNeural_PrevisaoFinanceira_v3\Metodos_Indices\PercentualCrescimentoValorAtivoMediaNDias.doc
        /// </summary>
        /// <param name="listCotacoes"></param>
        /// <returns></returns>
        public static void PreencherPercentualCrescimentoValorAtivoMediaNDias(List<DadosBE> listCotacoes)
        {
            //Deve ser maior do que 10
            int nDiasAnalisePercentual = 15;
            //Alimenta os n-1 primeiro dias com o valor intermediário, que não deve influenciar muito a rede neural (os n-1 primeiros dias não podem ser calculados)
            listCotacoes.Take(nDiasAnalisePercentual - 1).ToList().ForEach(cot => cot.PercentualCrescimentoValorAtivoMediaNDias = 0.5);
            for (int i = 0; i < listCotacoes.Count - nDiasAnalisePercentual; i++)
            {
                List<DadosBE> cotacoesAnaliseAtual = listCotacoes.Skip(i).Take(nDiasAnalisePercentual).ToList();

                double min = Convert.ToDouble(cotacoesAnaliseAtual.OrderBy(cot => cot.ValorNormalizado).Take(3).Average(cot => cot.ValorNormalizado));
                double max = Convert.ToDouble(cotacoesAnaliseAtual.OrderByDescending(cot => cot.ValorNormalizado).Take(3).Average(cot => cot.ValorNormalizado));
                double med = Convert.ToDouble(cotacoesAnaliseAtual.Average(cot => cot.ValorNormalizado));
                double hoje = Convert.ToDouble(cotacoesAnaliseAtual.Last().ValorNormalizado);

                if (hoje < med)
                {
                    //0.3 ao invés de 0.5 para caso o valor atual seja maior do que a média
                    double aux = 0.3 / (med - min);
                    cotacoesAnaliseAtual.Last().PercentualCrescimentoValorAtivoMediaNDias = 0.5 - ((med - hoje) * aux);
                }
                else if (hoje > med)
                {
                    //0.3 ao invés de 0.5 para caso o valor atual seja maior do que a média
                    double aux = 0.3 / (max - med);
                    cotacoesAnaliseAtual.Last().PercentualCrescimentoValorAtivoMediaNDias = 0.5 + ((hoje - med) * aux);
                }
                else
                    cotacoesAnaliseAtual.Last().PercentualCrescimentoValorAtivoMediaNDias = 0.5;
            }
        }

        /// <summary>
        /// Explicação em: Redes\03_09_RedeNeural_PrevisaoFinanceira_v3\Metodos_Indices\PercentualCrescimentoValorAtivo.doc
        /// </summary>
        /// <param name="listCotacoes"></param>
        /// <returns></returns>
        public static void PreencherPercentualCrescimentoValorAtivo(List<DadosBE> listCotacoes, double versao = 3)
        {
            if (listCotacoes.Count == 0)
                return;

            int qtdPercentuaisAtivo = 4;
            if (versao == 3.7)
                qtdPercentuaisAtivo = 2;
            if (versao == 3.8)
                qtdPercentuaisAtivo = 10;
            if (versao == 4.01)
                qtdPercentuaisAtivo = 1;

            //Valor máximo que a cotação do ativo pode destoar do dia anterior, 1.5 vezes maior ou 1.3 vezes menor
            double maxCrescimento = 0.5;

            foreach (DadosBE dadoBE in listCotacoes)
            {
                //Instancia os 'qtdPercentuaisAtivo' valores
                dadoBE.PercentualCrescimentoValorAtivo = new List<double>(new double[qtdPercentuaisAtivo]);
            }

            for (int indAtivo = 1; indAtivo < listCotacoes.Count; indAtivo++)
            {
                if (listCotacoes[indAtivo].ValorNormalizado == 0 || listCotacoes[indAtivo - 1].ValorNormalizado == 0)
                    continue;

                if (listCotacoes[indAtivo].ValorNormalizado > listCotacoes[indAtivo - 1].ValorNormalizado)
                {
                    double valCresc = Convert.ToDouble(listCotacoes[indAtivo].ValorNormalizado / listCotacoes[indAtivo - 1].ValorNormalizado);
                    valCresc -= 1;
                    listCotacoes[indAtivo].PercentualCrescimentoValorAtivo[qtdPercentuaisAtivo - 1] = 0.5 + (valCresc / maxCrescimento * 0.5);
                }
                else
                {
                    double valCresc = Convert.ToDouble(listCotacoes[indAtivo - 1].ValorNormalizado / listCotacoes[indAtivo].ValorNormalizado);
                    valCresc -= 1;
                    listCotacoes[indAtivo].PercentualCrescimentoValorAtivo[qtdPercentuaisAtivo - 1] = 0.5 - (valCresc / maxCrescimento * 0.5);
                }
            }

            //Preenche os 'qtdPercentuaisAtivo' valores anteriores
            for (int indAtivo = 0; indAtivo < listCotacoes.Count; indAtivo++)
            {
                //Os 'qtdPercentuaisAtivo -1' primeiros dados não podem ser preenchidos com os valores dos dias anteriores, pq não tem os 'qtdPercentuaisAtivo - 1' dias anteriores 
                if (indAtivo < qtdPercentuaisAtivo - 1)
                {
                    listCotacoes[indAtivo].PercentualCrescimentoValorAtivo.ForEach(percent => percent = 0.5);
                }
                else
                {
                    for (int indAtivoAnterior = 1; indAtivoAnterior < qtdPercentuaisAtivo; indAtivoAnterior++)
                    {
                        listCotacoes[indAtivo].PercentualCrescimentoValorAtivo[(qtdPercentuaisAtivo - indAtivoAnterior) - 1] = listCotacoes[indAtivo - indAtivoAnterior].PercentualCrescimentoValorAtivo.Last();
                    }
                }
            }
        }

        /// <summary>
        /// Explicação em: Redes\03_09_RedeNeural_PrevisaoFinanceira_v3\Metodos_Indices\PercentualDesviosPadroesEmRelacaoNDias.doc
        /// </summary>
        /// <param name="listCotacoes"></param>
        /// <returns></returns>
        public static void PreencherPercentualDesviosPadroesEmRelacaoNDias(List<DadosBE> listCotacoes)
        {
            //Deve ser maior do que 10
            int nDiasAnaliseDesvio = 15;
            //Quantidade de vezes que o desvio de ontem para hoje pode ser maior do que o desvio padrao dos N dias
            int maxDiferencaDesvio = 15;

            //Alimenta os n-1 primeiro dias com o valor intermediário, que não deve influenciar muito a rede neural (os n-1 primeiros dias não podem ser calculados)
            listCotacoes.Take(nDiasAnaliseDesvio - 1).ToList().ForEach(cot => cot.PercentualCrescimentoValorAtivoMediaNDias = 0.5);
            for (int i = 0; i < listCotacoes.Count - nDiasAnaliseDesvio; i++)
            {
                List<DadosBE> cotacoesAnaliseAtual = listCotacoes.Skip(i).Take(nDiasAnaliseDesvio).ToList();

                double desvioPadrao = 0;
                for (int indCotAtual = 1; indCotAtual < cotacoesAnaliseAtual.Count; indCotAtual++)
                {
                    desvioPadrao += Convert.ToDouble(Math.Abs(cotacoesAnaliseAtual[indCotAtual].ValorNormalizado - cotacoesAnaliseAtual[indCotAtual - 1].ValorNormalizado));
                }

                //Divide por 'nDiasAnaliseDesvio -1' pq só conseguimos calcular o desvio de n-1 dias ('d2' - 'd1', 'd3' - 'd2',...'dn' - 'dn-1')
                desvioPadrao /= (nDiasAnaliseDesvio - 1);

                double desvioHoje_Relacao_Ontem = Convert.ToDouble(Math.Abs(cotacoesAnaliseAtual[nDiasAnaliseDesvio - 1].ValorNormalizado - cotacoesAnaliseAtual[nDiasAnaliseDesvio - 2].ValorNormalizado));
                if (desvioHoje_Relacao_Ontem == 0)
                {
                    cotacoesAnaliseAtual.Last().PercentualDesviosPadroesEmRelacaoNDias = 0.5;
                    continue;
                }

                if (desvioHoje_Relacao_Ontem > desvioPadrao)
                {
                    cotacoesAnaliseAtual.Last().PercentualDesviosPadroesEmRelacaoNDias =
                        0.5 + ((desvioHoje_Relacao_Ontem / desvioPadrao - 1) * 0.5 / maxDiferencaDesvio);
                }
                else
                {
                    cotacoesAnaliseAtual.Last().PercentualDesviosPadroesEmRelacaoNDias =
                        0.5 - ((desvioPadrao / desvioHoje_Relacao_Ontem - 1) * 0.5 / maxDiferencaDesvio);
                }
            }
        }

        /// <summary>
        /// Preenche o dia da semana do DadoBE
        /// </summary>
        /// <param name="listCotacoes"></param>
        public static void PreencherDiaSemana(List<DadosBE> listCotacoes)
        {
            Func<int, double> value = indDia => Convert.ToDouble(1.0 / 6.0 * indDia);

            Dictionary<DayOfWeek, double> dic = new Dictionary<DayOfWeek, double>();
            dic.Add(DayOfWeek.Friday, value(0));
            dic.Add(DayOfWeek.Monday, value(1));
            dic.Add(DayOfWeek.Saturday, value(2));
            dic.Add(DayOfWeek.Sunday, value(3));
            dic.Add(DayOfWeek.Thursday, value(4));
            dic.Add(DayOfWeek.Tuesday, value(5));
            dic.Add(DayOfWeek.Wednesday, value(6));

            listCotacoes.ForEach(cot => cot.DiaSemana = dic[cot.DataGeracao.DayOfWeek]);
        }

        /// <summary>
        /// Explicação em: Redes\03_09_RedeNeural_PrevisaoFinanceira_v3\Metodos_Indices\PercentualValorAtivo_Max_Min_Med.doc
        /// </summary>
        /// <param name="listCotacoes"></param>
        /// <returns></returns>
        public static void PreencherPercentualValorAtivo_Max_Min_Med(List<DadosBE> listCotacoes)
        {
            //NESTE UTILIZAMOS O PREÇO DE ABERTURA POIS OS VALORES MAX,MIN,MEDIO SAO EM RELAÇÃO AO PREÇO DE ABERTURA
            foreach (DadosBE dadoBE in listCotacoes)
            {
                double min = Convert.ToDouble(dadoBE.PrecoMinimo);
                double max = Convert.ToDouble(dadoBE.PrecoMaximo);
                double med = Convert.ToDouble(dadoBE.PrecoMedio);
                double valFechamento = Convert.ToDouble(dadoBE.PrecoAbertura);

                if (valFechamento < med)
                {
                    //0.3 ao invés de 0.5 para caso o valor atual seja maior do que a média
                    double aux = 0.3 / (med - min);
                    dadoBE.PercentualValorAtivo_Max_Min_Med = 0.5 - ((med - valFechamento) * aux);
                }
                else if (valFechamento > med)
                {
                    //0.3 ao invés de 0.5 para caso o valor atual seja maior do que a média
                    double aux = 0.3 / (max - med);
                    dadoBE.PercentualValorAtivo_Max_Min_Med = 0.5 + ((valFechamento - med) * aux);
                }
                else
                    dadoBE.PercentualValorAtivo_Max_Min_Med = 0.5;
            }
        }

        /// <summary>
        /// Preenche os indices que alimentarão a RN_V3
        /// </summary>
        /// <param name="listCotacoes"></param>
        public static void PreencherIndicesRN_V3(List<DadosBE> listCotacoes, double versao)
        {
            PreencherPontuacao3MediasMoveis(listCotacoes);
            PreencherPercentualTotalNegociacoesMediaNDias(listCotacoes);
            PreencherPercentualTotalNegociacoes(listCotacoes);
            PreencherPercentualCrescimentoDolar(listCotacoes);
            PreencherPercentualCrescimentoValorAtivoMediaNDias(listCotacoes);
            PreencherPercentualCrescimentoValorAtivo(listCotacoes, versao);
            PreencherPercentualDesviosPadroesEmRelacaoNDias(listCotacoes);
            PreencherDiaSemana(listCotacoes);
            PreencherPercentualValorAtivo_Max_Min_Med(listCotacoes);
            //Já está sendo preenchido na recuperação dos dados
            PreencherValorBollinger(listCotacoes);
        }

        /// <summary>
        /// Seleciona os treinamentos da versão 3
        /// </summary>
        /// <param name="dadosBE"></param>
        /// <returns></returns>
        public static List<Treinamento> SelecionarTreinamentos_V3(List<DadosBE> dadosBE, double versao)
        {
            if (dadosBE.Count == 0)
                return null;

            //Preenche os indices da RN_V3
            PreencherIndicesRN_V3(dadosBE, versao);

            List<Treinamento> treinamentos = new List<Treinamento>();
            //A cada 'qtdRegistrosPorDivisao' registros, o numero do cross validation deve ser incrementado de 1
            int qtdRegistrosPorDivisao = dadosBE.Count / numeroDivisoesCrossValidation;

            for (int indDadoBE = 0; indDadoBE < dadosBE.Count; indDadoBE++)
            {
                int valCrossValidation = indDadoBE / qtdRegistrosPorDivisao;
                //Corrige o numero da divisao cross validation pois os ultimos registros podem estar errados devido ao arredondamento
                valCrossValidation = valCrossValidation == numeroDivisoesCrossValidation ? numeroDivisoesCrossValidation - 1 : valCrossValidation;

                Treinamento treinamento = TransformarDadoBE_Em_Treinamento_RNV3(dadosBE[indDadoBE], versao);
                treinamento.DivisaoCrossValidation = valCrossValidation;

                treinamentos.Add(treinamento);
            }
            int ind = 0;
            List<double> inputs = treinamentos.Select(trein => trein.Input[ind]).ToList();
            List<double> acimas = inputs.Where(inp => inp > 1).ToList();
            List<double> abaixos = inputs.Where(inp => inp < 0).ToList();
            ind++;

            return treinamentos;
        }

        public static Treinamento TransformarDadoBE_Em_Treinamento_RNV3(DadosBE dadoBE, double versao)
        {
            Treinamento treinamento = new Treinamento();
            treinamento.Input = new List<double>();

            //Adiciona cada um dos percentuais dos n dias anteriores e do dia da cotação
            dadoBE.PercentualCrescimentoValorAtivo.ForEach(percent => treinamento.Input.Add(percent));
            treinamento.Input.Add(dadoBE.ValorNormalizado);
            if (versao == 3)
            {
                treinamento.Input.Add(dadoBE.ValorBollinger);
                treinamento.Input.Add(dadoBE.Pontuacao3MediasMoveis);
                treinamento.Input.Add(dadoBE.PercentualTotalNegociacoesMediaNDias);
                treinamento.Input.Add(dadoBE.PercentualTotalNegociacoes);
                treinamento.Input.Add(dadoBE.PercentualCrescimentoDolar);
                treinamento.Input.Add(dadoBE.PercentualCrescimentoValorAtivoMediaNDias);
                treinamento.Input.Add(dadoBE.PercentualDesviosPadroesEmRelacaoNDias);
                treinamento.Input.Add(dadoBE.DiaSemana);
                treinamento.Input.Add(dadoBE.PercentualValorAtivo_Max_Min_Med);
            }
            if (versao == 3.2)
            {
                treinamento.Input.Add(dadoBE.ValorBollinger);
                //treinamento.Input.Add(dadoBE.Pontuacao3MediasMoveis);
                //treinamento.Input.Add(dadoBE.PercentualTotalNegociacoesMediaNDias);
                treinamento.Input.Add(dadoBE.PercentualTotalNegociacoes);
                treinamento.Input.Add(dadoBE.PercentualCrescimentoDolar);
                //treinamento.Input.Add(dadoBE.PercentualCrescimentoValorAtivoMediaNDias);
                treinamento.Input.Add(dadoBE.PercentualDesviosPadroesEmRelacaoNDias);
                //treinamento.Input.Add(dadoBE.DiaSemana);
                treinamento.Input.Add(dadoBE.PercentualValorAtivo_Max_Min_Med);
            }
            if (versao == 3.3)
            {
                treinamento.Input.Add(dadoBE.ValorBollinger);
                ////treinamento.Input.Add(dadoBE.Pontuacao3MediasMoveis);
                ////treinamento.Input.Add(dadoBE.PercentualTotalNegociacoesMediaNDias);
                //treinamento.Input.Add(dadoBE.PercentualTotalNegociacoes);
                //treinamento.Input.Add(dadoBE.PercentualCrescimentoDolar);
                ////treinamento.Input.Add(dadoBE.PercentualCrescimentoValorAtivoMediaNDias);
                treinamento.Input.Add(dadoBE.PercentualDesviosPadroesEmRelacaoNDias);
                ////treinamento.Input.Add(dadoBE.DiaSemana);
                treinamento.Input.Add(dadoBE.PercentualValorAtivo_Max_Min_Med);
            }
            if (versao == 3.4)
            {
                treinamento.Input.Add(dadoBE.ValorBollinger);
                ////treinamento.Input.Add(dadoBE.Pontuacao3MediasMoveis);
                ////treinamento.Input.Add(dadoBE.PercentualTotalNegociacoesMediaNDias);
                //treinamento.Input.Add(dadoBE.PercentualTotalNegociacoes);
                //treinamento.Input.Add(dadoBE.PercentualCrescimentoDolar);
                ////treinamento.Input.Add(dadoBE.PercentualCrescimentoValorAtivoMediaNDias);
                treinamento.Input.Add(dadoBE.PercentualDesviosPadroesEmRelacaoNDias);
                ////treinamento.Input.Add(dadoBE.DiaSemana);
                treinamento.Input.Add(dadoBE.PercentualValorAtivo_Max_Min_Med);
            }
            if (versao == 3.5)
            {
                treinamento.Input.Add(dadoBE.ValorBollinger);
                ////treinamento.Input.Add(dadoBE.Pontuacao3MediasMoveis);
                ////treinamento.Input.Add(dadoBE.PercentualTotalNegociacoesMediaNDias);
                //treinamento.Input.Add(dadoBE.PercentualTotalNegociacoes);
                //treinamento.Input.Add(dadoBE.PercentualCrescimentoDolar);
                //treinamento.Input.Add(dadoBE.PercentualCrescimentoValorAtivoMediaNDias);
                treinamento.Input.Add(dadoBE.PercentualDesviosPadroesEmRelacaoNDias);
                ////treinamento.Input.Add(dadoBE.DiaSemana);
                //treinamento.Input.Add(dadoBE.PercentualValorAtivo_Max_Min_Med);
            }
            if (versao == 3.6)
            {//Apenas tiramos o ValorBollinger para ver se houve alguma diferença
                //treinamento.Input.Add(dadoBE.ValorBollinger);
                ////treinamento.Input.Add(dadoBE.Pontuacao3MediasMoveis);
                ////treinamento.Input.Add(dadoBE.PercentualTotalNegociacoesMediaNDias);
                //treinamento.Input.Add(dadoBE.PercentualTotalNegociacoes);
                //treinamento.Input.Add(dadoBE.PercentualCrescimentoDolar);
                //treinamento.Input.Add(dadoBE.PercentualCrescimentoValorAtivoMediaNDias);
                treinamento.Input.Add(dadoBE.PercentualDesviosPadroesEmRelacaoNDias);
                ////treinamento.Input.Add(dadoBE.DiaSemana);
                //treinamento.Input.Add(dadoBE.PercentualValorAtivo_Max_Min_Med);
            }
            if (versao == 3.7)
            {//Com todos os índices possíveis para previsao em cima de previsao, mas agora com 2 percentuais em relação aos dias anteriores ao invés de 4
                treinamento.Input.Add(dadoBE.ValorBollinger);
                ////treinamento.Input.Add(dadoBE.Pontuacao3MediasMoveis);
                ////treinamento.Input.Add(dadoBE.PercentualTotalNegociacoesMediaNDias);
                //treinamento.Input.Add(dadoBE.PercentualTotalNegociacoes);
                //treinamento.Input.Add(dadoBE.PercentualCrescimentoDolar);
                treinamento.Input.Add(dadoBE.PercentualCrescimentoValorAtivoMediaNDias);
                treinamento.Input.Add(dadoBE.PercentualDesviosPadroesEmRelacaoNDias);
                ////treinamento.Input.Add(dadoBE.DiaSemana);
                //treinamento.Input.Add(dadoBE.PercentualValorAtivo_Max_Min_Med);
            }
            if (versao == 3.8)
            {//Com todos os índices possíveis para previsao em cima de previsao, mas agora com 10 percentuais em relação aos dias anteriores ao invés de 4
                treinamento.Input.Add(dadoBE.ValorBollinger);
                ////treinamento.Input.Add(dadoBE.Pontuacao3MediasMoveis);
                ////treinamento.Input.Add(dadoBE.PercentualTotalNegociacoesMediaNDias);
                //treinamento.Input.Add(dadoBE.PercentualTotalNegociacoes);
                //treinamento.Input.Add(dadoBE.PercentualCrescimentoDolar);
                treinamento.Input.Add(dadoBE.PercentualCrescimentoValorAtivoMediaNDias);
                treinamento.Input.Add(dadoBE.PercentualDesviosPadroesEmRelacaoNDias);
                ////treinamento.Input.Add(dadoBE.DiaSemana);
                //treinamento.Input.Add(dadoBE.PercentualValorAtivo_Max_Min_Med);
            }
            if (versao == 3.9)
            {
                treinamento.Input.Add(dadoBE.ValorBollinger);
                ////treinamento.Input.Add(dadoBE.Pontuacao3MediasMoveis);
                ////treinamento.Input.Add(dadoBE.PercentualTotalNegociacoesMediaNDias);
                //treinamento.Input.Add(dadoBE.PercentualTotalNegociacoes);
                //treinamento.Input.Add(dadoBE.PercentualCrescimentoDolar);
                treinamento.Input.Add(dadoBE.PercentualCrescimentoValorAtivoMediaNDias);
                //treinamento.Input.Add(dadoBE.PercentualDesviosPadroesEmRelacaoNDias);
                ////treinamento.Input.Add(dadoBE.DiaSemana);
                //treinamento.Input.Add(dadoBE.PercentualValorAtivo_Max_Min_Med);
            }
            if (versao == 4.01)
            {
                treinamento.Input.Add(dadoBE.ValorBollinger);
                ////treinamento.Input.Add(dadoBE.Pontuacao3MediasMoveis);
                ////treinamento.Input.Add(dadoBE.PercentualTotalNegociacoesMediaNDias);
                //treinamento.Input.Add(dadoBE.PercentualTotalNegociacoes);
                //treinamento.Input.Add(dadoBE.PercentualCrescimentoDolar);
                treinamento.Input.Add(dadoBE.PercentualCrescimentoValorAtivoMediaNDias);
                //treinamento.Input.Add(dadoBE.PercentualDesviosPadroesEmRelacaoNDias);
                ////treinamento.Input.Add(dadoBE.DiaSemana);
                //treinamento.Input.Add(dadoBE.PercentualValorAtivo_Max_Min_Med);
            }
            if (versao == 4.02)
            {
                treinamento.Input.Add(dadoBE.ValorBollinger);
                treinamento.Input.Add(dadoBE.Pontuacao3MediasMoveis);
                ////treinamento.Input.Add(dadoBE.PercentualTotalNegociacoesMediaNDias);
                //treinamento.Input.Add(dadoBE.PercentualTotalNegociacoes);
                //treinamento.Input.Add(dadoBE.PercentualCrescimentoDolar);
                //treinamento.Input.Add(dadoBE.PercentualCrescimentoValorAtivoMediaNDias);
                treinamento.Input.Add(dadoBE.PercentualDesviosPadroesEmRelacaoNDias);
                treinamento.Input.Add(dadoBE.DiaSemana);
                //treinamento.Input.Add(dadoBE.PercentualValorAtivo_Max_Min_Med);
            }
            if (versao == 4.03)
            {
                treinamento.Input.Add(dadoBE.ValorBollinger);
                ////treinamento.Input.Add(dadoBE.Pontuacao3MediasMoveis);
                ////treinamento.Input.Add(dadoBE.PercentualTotalNegociacoesMediaNDias);
                //treinamento.Input.Add(dadoBE.PercentualTotalNegociacoes);
                //treinamento.Input.Add(dadoBE.PercentualCrescimentoDolar);
                ////treinamento.Input.Add(dadoBE.PercentualCrescimentoValorAtivoMediaNDias);
                treinamento.Input.Add(dadoBE.PercentualDesviosPadroesEmRelacaoNDias);
                ////treinamento.Input.Add(dadoBE.DiaSemana);
                treinamento.Input.Add(dadoBE.PercentualValorAtivo_Max_Min_Med);
            }
            if (versao == 4.04)
            {
                treinamento.Input.Add(dadoBE.ValorBollinger);
                ////treinamento.Input.Add(dadoBE.Pontuacao3MediasMoveis);
                ////treinamento.Input.Add(dadoBE.PercentualTotalNegociacoesMediaNDias);
                //treinamento.Input.Add(dadoBE.PercentualTotalNegociacoes);
                //treinamento.Input.Add(dadoBE.PercentualCrescimentoDolar);
                ////treinamento.Input.Add(dadoBE.PercentualCrescimentoValorAtivoMediaNDias);
                treinamento.Input.Add(dadoBE.PercentualDesviosPadroesEmRelacaoNDias);
                ////treinamento.Input.Add(dadoBE.DiaSemana);
                treinamento.Input.Add(dadoBE.PercentualValorAtivo_Max_Min_Med);
                treinamento.Input.Add(1);//Valor Bias
            }

            treinamento.Output = new List<double>() { dadoBE.ValorNormalizadoDiaSeguinte };

            return treinamento;
        }

        #endregion RN_V3

        #region RN_V5

        //dicIndicesRN.Add(5.01, new List<string>() { "PercentualCrescimentoValorAtivo" });
        //dicIndicesRN.Add(5.02, new List<string>() { "ValorNormalizado" });
        //dicIndicesRN.Add(5.03, new List<string>() { "ValorBollinger" });
        //dicIndicesRN.Add(5.04, new List<string>() { "Pontuacao3MediasMoveis" });
        //dicIndicesRN.Add(5.05, new List<string>() { "PercentualTotalNegociacoesMediaNDias" });
        //dicIndicesRN.Add(5.06, new List<string>() { "PercentualTotalNegociacoes" });
        //dicIndicesRN.Add(5.07, new List<string>() { "PercentualCrescimentoDolar" });
        //dicIndicesRN.Add(5.08, new List<string>() { "PercentualCrescimentoValorAtivoMediaNDias" });
        //dicIndicesRN.Add(5.09, new List<string>() { "PercentualDesviosPadroesEmRelacaoNDias" });
        //dicIndicesRN.Add(5.10, new List<string>() { "DiaSemana", "PercentualValorAtivo_Max_Min_Med" });
        //dicIndicesRN.Add(5.11, new List<string>() { "PercentualValorAtivo_Max_Min_Med" });
        static Dictionary<double, List<string>> _dicIndicesRN;
        static Dictionary<double, List<string>> dicIndicesRN
        {
            get
            {
                if (_dicIndicesRN == null)
                {
                    _dicIndicesRN = new Dictionary<double, List<string>>();
                    _dicIndicesRN.Add(5.01, new List<string>() { "PercentualCrescimentoValorAtivo" });
                    _dicIndicesRN.Add(5.02, new List<string>() { "ValorBollinger" });
                    _dicIndicesRN.Add(5.03, new List<string>() { "Pontuacao3MediasMoveis" });
                    _dicIndicesRN.Add(5.04, new List<string>() { "PercentualTotalNegociacoesMediaNDias" });
                    _dicIndicesRN.Add(5.05, new List<string>() { "PercentualTotalNegociacoes" });
                    _dicIndicesRN.Add(5.06, new List<string>() { "PercentualCrescimentoDolar" });
                    _dicIndicesRN.Add(5.07, new List<string>() { "PercentualCrescimentoValorAtivoMediaNDias" });
                    _dicIndicesRN.Add(5.08, new List<string>() { "PercentualDesviosPadroesEmRelacaoNDias" });
                    _dicIndicesRN.Add(5.09, new List<string>() { "DiaSemana" });
                    _dicIndicesRN.Add(5.10, new List<string>() { "PercentualValorAtivo_Max_Min_Med" });
                    _dicIndicesRN.Add(5.11, new List<string>() { "DN_Maior_D0" });
                }
                return _dicIndicesRN;
            }
        }

        public static void Preencher_DN_Maior_D0(List<DadosBE> listCotacoes, double versao)
        {
            int n = 4;
            treinamentosIniciaisIgnorar = n - 1;

            for (int indCot = n - 1; indCot < listCotacoes.Count; indCot++)
            {
                List<double> valores = new List<double>();
                for (int indN = indCot - 3; indN <= indCot; indN++)
                {
                    valores.Add(Convert.ToDouble(listCotacoes[indN].PrecoAbertura));
                }
                List<bool> valoresBool = RecuperaValoresBooleanos(valores);
                List<int> arrayValores = RecuperarArrayTransformacaoBoolInt(valoresBool);
                listCotacoes[indCot].DN_Maior_D0 = arrayValores;
            }
        }
        static List<bool> RecuperaValoresBooleanos(List<double> valores)
        {
            List<bool> valoresBool = new List<bool>();
            for (int indValX = valores.Count - 1; indValX > 0; indValX--)
            {
                for (int indValY = indValX - 1; indValY >= 0; indValY--)
                {
                    valoresBool.Add(valores[indValX] > valores[indValY]);
                }
            }
            return valoresBool;
        }
        static List<int> RecuperarArrayTransformacaoBoolInt(List<bool> valoresBool)
        {
            Func<int, int> getIntFromBoolInd = ind => Convert.ToInt32(Math.Pow(2, ind));
            int valor = 0;
            for (int i = 0; i < valoresBool.Count; i++)
            {
                if (valoresBool[i])
                    valor += getIntFromBoolInd(i);
            }

            int numComb = Convert.ToInt32(Math.Pow(2, valoresBool.Count));
            List<int> arrayRetorno = new List<int>();
            for (int i = 0; i < numComb; i++)
            {
                if (i == valor)
                    arrayRetorno.Add(1);
                else
                    arrayRetorno.Add(0);
            }

            return arrayRetorno;
        }

        /// <summary>
        /// Preenche os indices que alimentarão a RN_V5
        /// </summary>
        /// <param name="listCotacoes"></param>
        public static void PreencherIndicesRN_V5(List<DadosBE> listCotacoes, double versao)
        {
            PreencherIndicesRN_V3(listCotacoes, versao);
            Preencher_DN_Maior_D0(listCotacoes, versao);
        }

        /// <summary>
        /// Seleciona os treinamentos da versão 5
        /// </summary>
        /// <param name="dadosBE"></param>
        /// <returns></returns>
        public static List<Treinamento> SelecionarTreinamentos_V5(List<DadosBE> dadosBE, double versao)
        {
            if (dadosBE.Count == 0)
                return null;

            //Reinicializa a variável para descobrir quantos treinamentos iniciais devem ser ignorados
            _treinamentosIniciaisIgnorar = 0;
            //Preenche os indices da RN_V5
            PreencherIndicesRN_V5(dadosBE, versao);


            List<Treinamento> treinamentos = new List<Treinamento>();
            //A cada 'qtdRegistrosPorDivisao' registros, o numero do cross validation deve ser incrementado de 1
            int qtdRegistrosPorDivisao = dadosBE.Count / numeroDivisoesCrossValidation;

            for (int indDadoBE = _treinamentosIniciaisIgnorar; indDadoBE < dadosBE.Count; indDadoBE++)
            {
                int valCrossValidation = indDadoBE / qtdRegistrosPorDivisao;
                //Corrige o numero da divisao cross validation pois os ultimos registros podem estar errados devido ao arredondamento
                valCrossValidation = valCrossValidation == numeroDivisoesCrossValidation ? numeroDivisoesCrossValidation - 1 : valCrossValidation;

                Treinamento treinamento = TransformarDadoBE_Em_Treinamento_RNV5(dadosBE[indDadoBE], versao);
                treinamento.DivisaoCrossValidation = valCrossValidation;

                treinamentos.Add(treinamento);
            }
            int ind = 0;
            List<double> inputs = treinamentos.Select(trein => trein.Input[ind]).ToList();
            List<double> acimas = inputs.Where(inp => inp > 1).ToList();
            List<double> abaixos = inputs.Where(inp => inp < 0).ToList();
            ind++;

            return treinamentos.Skip(treinamentosIniciaisIgnorar).ToList();
        }

        public static Treinamento TransformarDadoBE_Em_Treinamento_RNV5(DadosBE dadoBE, double versao)
        {
            Treinamento treinamento = new Treinamento();
            treinamento.Input = new List<double>();
            treinamento.Input.Add(dadoBE.ValorNormalizado);

            if (dicIndicesRN[versao].Contains("PercentualCrescimentoValorAtivo"))
                //Adiciona cada um dos percentuais dos n dias anteriores e do dia da cotação
                dadoBE.PercentualCrescimentoValorAtivo.ForEach(percent => treinamento.Input.Add(percent));

            if (dicIndicesRN[versao].Contains("ValorBollinger"))
                treinamento.Input.Add(dadoBE.ValorBollinger);

            if (dicIndicesRN[versao].Contains("Pontuacao3MediasMoveis"))
                treinamento.Input.Add(dadoBE.Pontuacao3MediasMoveis);

            if (dicIndicesRN[versao].Contains("PercentualTotalNegociacoesMediaNDias"))
                treinamento.Input.Add(dadoBE.PercentualTotalNegociacoesMediaNDias);

            if (dicIndicesRN[versao].Contains("PercentualTotalNegociacoes"))
                treinamento.Input.Add(dadoBE.PercentualTotalNegociacoes);

            if (dicIndicesRN[versao].Contains("PercentualCrescimentoDolar"))
                treinamento.Input.Add(dadoBE.PercentualCrescimentoDolar);

            if (dicIndicesRN[versao].Contains("PercentualCrescimentoValorAtivoMediaNDias"))
                treinamento.Input.Add(dadoBE.PercentualCrescimentoValorAtivoMediaNDias);

            if (dicIndicesRN[versao].Contains("PercentualDesviosPadroesEmRelacaoNDias"))
                treinamento.Input.Add(dadoBE.PercentualDesviosPadroesEmRelacaoNDias);

            if (dicIndicesRN[versao].Contains("DiaSemana"))
                treinamento.Input.Add(dadoBE.DiaSemana);

            if (dicIndicesRN[versao].Contains("PercentualValorAtivo_Max_Min_Med"))
                treinamento.Input.Add(dadoBE.PercentualValorAtivo_Max_Min_Med);

            if (dicIndicesRN[versao].Contains("DN_Maior_D0"))
                dadoBE.DN_Maior_D0.ForEach(val => treinamento.Input.Add(val));

            treinamento.Output = new List<double>() { dadoBE.ValorNormalizadoDiaSeguinte };

            return treinamento;
        }

        #endregion RN_V5
    }
}
