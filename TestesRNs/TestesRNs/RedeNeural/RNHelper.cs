using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestesRNs.Modelo;
using NeuronDotNet.Core;
using System.IO;
using System.Configuration;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace TestesRNs.RedeNeural
{
    public class RNHelper
    {
        private static int numeroValidacoes = 8;
        private static string diretorioRedes
        {
            get
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["DiretorioRedes"]))
                    return ConfigurationManager.AppSettings["DiretorioRedes"];
                else
                    return System.IO.Directory.GetCurrentDirectory();
            }
        }

        static Dictionary<string, KeyValuePair<List<Versao>, int>> configuracaoPorPapel = new Dictionary<string, KeyValuePair<List<Versao>, int>>();

        private static void PreencherConfiguracoes()
        {
            if (configuracaoPorPapel.Count > 0)
                return;
            //BVSP - Tam.Tend:10-ValorBollinger; Williams_Percent_R_14P; Williams_Percent_R_28P
            configuracaoPorPapel.Add("BVSP", new KeyValuePair<List<Versao>, int>(new List<Versao>() { Versao.V6001, Versao.V6004, Versao.V6008 }, 10));
            //ETER3 - Tam.Tend:9-ValorBollinger
            configuracaoPorPapel.Add("ETER3", new KeyValuePair<List<Versao>, int>(new List<Versao>() { Versao.V6001 }, 9));
            //GOLL4 - Tam.Tend:6-ValorBollinger; Arron_Up_Down; DuracaoTendencias
            configuracaoPorPapel.Add("GOLL4", new KeyValuePair<List<Versao>, int>(new List<Versao>() { Versao.V6001, Versao.V6016, Versao.V6032 }, 6));

            //SEGUNDA..//NATU3 - Tam.Tend:6-ValorBollinger; MediaMovelSimples5Dias; Williams_Percent_R_28P; DuracaoTendencias
            configuracaoPorPapel.Add("NATU3", new KeyValuePair<List<Versao>, int>(new List<Versao>() { Versao.V6001, Versao.V6002, Versao.V6008, /*Versao.V6016,*/ Versao.V6032 }, 6));
            ////NATU3 - Tam.Tend:8-Arron_Up_Down
            //configuracaoPorPapel.Add("NATU3", new KeyValuePair<List<Versao>, int>(new List<Versao>() { Versao.V6016 }, 8));

            //PETR4 - Tam.Tend:10-ValorBollinger; Williams_Percent_R_28P
            configuracaoPorPapel.Add("PETR4", new KeyValuePair<List<Versao>, int>(new List<Versao>() { Versao.V6001, Versao.V6008 }, 10));

            //SEGUNDA..//VALE5 - Tam.Tend:7-Arron_Up_Down
            configuracaoPorPapel.Add("VALE5", new KeyValuePair<List<Versao>, int>(new List<Versao>() { Versao.V6008 }, 10));
            ////VALE5 - Tam.Tend:7-Arron_Up_Down
            //configuracaoPorPapel.Add("VALE5", new KeyValuePair<List<Versao>, int>(new List<Versao>() { Versao.V6016 }, 7));
        }

        public static ResumoPrevisao PreverAtivo(string papel)
        {
            PreencherConfiguracoes();


            List<Versao> versoesPapel = configuracaoPorPapel[papel].Key;
            int tamanhoTendencia = configuracaoPorPapel[papel].Value;

            List<DadoBE> dadosBE = DadoBE.PegarTodos(papel);
            dadosBE = dadosBE.Skip(dadosBE.Count / 8 * 7).ToList();
            DateTime dtInicial = dadosBE.First().DataGeracao;
            DateTime dtFinal = dadosBE.Last().DataGeracao;

            ResumoPrevisao resumoPrevisao = new ResumoPrevisao()
            {
                Papel = papel,
                TamanhoTendencia = configuracaoPorPapel[papel].Value,
                Versoes = configuracaoPorPapel[papel].Key,
                DataInicial = dadosBE.First().DataGeracao,
                DataFinal = dadosBE.Last().DataGeracao
            };

            //Recupera os treinamentos
            List<Treinamento> treinamentos = Treinamento.RecuperarTreinamentoRN(dadosBE, versoesPapel, tamanhoTendencia).Normalizar(papel, versoesPapel, tamanhoTendencia);
            Network redeNeural = RecuperarRedeNeural(papel, versoesPapel, tamanhoTendencia);

            foreach (DadoBE dadoBE in dadosBE)
            {
                Treinamento treinamento = treinamentos.FirstOrDefault(trein => trein.Data == dadoBE.DataGeracao);
                ResultadoPrevisao resultadoPrevisao = ResultadoPrevisao.Nenhuma;
                if (treinamento != null)
                {
                    double[] output = redeNeural.Run(treinamento.Input.ToArray());

                    //Rede diz que o valor vai subir..
                    if (output[0] > output[1])
                    {
                        if (treinamento.Output[0] > treinamento.Output[1])
                            resultadoPrevisao = ResultadoPrevisao.SubirOK;
                        else
                            resultadoPrevisao = ResultadoPrevisao.SubirNOK;
                    }
                    //Rede diz que o valor vai subir..
                    else
                    {
                        if (treinamento.Output[0] < treinamento.Output[1])
                            resultadoPrevisao = ResultadoPrevisao.DescerOK;
                        else
                            resultadoPrevisao = ResultadoPrevisao.DescerNOK;
                    }
                }

                resumoPrevisao.Previsoes.Add(new TestesRNs.Modelo.ResumoPrevisao.Previsao() { ValorAtivo = dadoBE.PrecoFechamento, ResultadoPrevisao = resultadoPrevisao, Data = dadoBE.DataGeracao });
            }
            //foreach (Treinamento treinamento in treinamentos)
            //{
            //    List<double> outputPrevistoNormalizado = redeNeural.Run(treinamento.Input.ToArray()).ToList();
            //    List<double> outputPrevistoDesnormalizado = Treinamento.DesnormalizarSaidas(outputPrevistoNormalizado, papel, versoesPapel, tamanhoTendencia);
            //    //double outputPrevisto = DadoBE.DesnormalizarDado(outputPrevistoNormalizado[0], papel);
            //    //double outputEsperado = DadoBE.DesnormalizarDado(treinamento.Output[0], papel);
            //    //resultado.Add(new double[] { outputEsperado, outputPrevisto });
            //    resultado.Add(new double[] { Treinamento.DesnormalizarSaidas(treinamento.Output, papel, versoesPapel, tamanhoTendencia)[0], outputPrevistoDesnormalizado[0] });
            //}

            return resumoPrevisao;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="papel"></param>
        /// <param name="versoes"></param>
        /// <param name="tamanhoTendencia">Quantos dias após a compra, iremos revender o ativo</param>
        /// <returns></returns>
        internal static Relatorio TreinarRedeNeural(string papel, List<Versao> versoes, int tamanhoTendencia)
        {
            List<DadoBE> dadosBE = DadoBE.PegarTodos(papel);
            List<Treinamento> treinamentos = Treinamento.RecuperarTreinamentoRN(dadosBE, versoes, tamanhoTendencia).Normalizar(papel, versoes, tamanhoTendencia, true);
            Relatorio relatorio = new Relatorio(papel, versoes, tamanhoTendencia, dadosBE.Min(dado => dado.DataGeracao), dadosBE.Max(dado => dado.DataGeracao), dadosBE.Count);
            for (int divCrossValidation = 0; divCrossValidation < numeroValidacoes; divCrossValidation++)
            {
                List<Treinamento> dadosTeste = treinamentos.Skip(treinamentos.Count / 8 * divCrossValidation).Take(treinamentos.Count / 8).ToList();
                List<Treinamento> dadosTreinamento = treinamentos.Where(trein => !dadosTeste.Any(teste => teste.Data.Equals(trein.Data))).ToList();

                string nomeRedeNeural = RecuperarNomeRedeNeural(papel, versoes, tamanhoTendencia);
                int numeroNeuronios = 0;//30;
                double taxaAprendizado = 0.25d;//0.25
                int ciclos = 10000000;//2000
                double acertoTreinamento = RNCore.Treinar(nomeRedeNeural, treinamentos, numeroNeuronios, taxaAprendizado, ciclos);

                double percentualAcerto = RecuperarErroTendencias(papel, versoes, dadosTeste, tamanhoTendencia);
                relatorio.AdicionarAoRelatorio(dadosTeste, percentualAcerto, tamanhoTendencia);
                //double percentual = SimularTomadasDeDecisoes(dadosBE, treinamentosTendencias, papel, versao);
            }
            return relatorio;
        }
        public static double RecuperarErroTendencias(string papel, List<Versao> Versoes, List<Treinamento> treinamentosTendencias, int tamanhoTendencia)
        {
            Network redeNeural = RecuperarRedeNeural(papel, Versoes, tamanhoTendencia);
            int numeroAcertos = 0;
            foreach (Treinamento treinamento in treinamentosTendencias)
            {
                double[] previsao = redeNeural.Run(treinamento.Input.ToArray());

                if (RNCore.ValoresMaximosNoMesmoIndice(previsao, treinamento.Output.ToArray()))
                {
                    numeroAcertos++;
                }
                //if (RNCore.ValoresMaximosNoMesmoIndice(treinamento.Input.ToArray(), treinamento.Output.ToArray()))
                //{
                //    numeroAcertos++;
                //}
            }

            return 100.00 / treinamentosTendencias.Count * numeroAcertos;
        }
        /*public static double SimularTomadasDeDecisoes(List<DadoBE> dadosBE, List<Treinamento> treinamentos, string papel, Versao versao)
        {
            Network redeNeural = RecuperarRedeNeural(papel, versao);
            double percentualGanhoPerda = 0;
            int numCompras = 0;
            foreach (Treinamento treinamento in treinamentos)
            {
                //double[] previsao = redeNeural.Run(treinamento.Input.ToArray());
                //if (previsao[0] > previsao[1])
                //{
                numCompras++;
                DadoBE dadoBEComprar = dadosBE.First(dado => dado.DataGeracao == treinamento.Data);
                DadoBE dadoBEVender = dadoBEComprar;
                //Será vendido daqui 5 dias
                for (int i = 0; i < 5; i++)
                {
                    if (dadoBEVender.Proximo == null)
                        break;
                    dadoBEVender = dadoBEVender.Proximo;
                }

                double variacaoEmReais = Math.Abs(dadoBEComprar.PrecoFechamento - dadoBEVender.PrecoFechamento);
                double percentualVariacao_G_P = variacaoEmReais / dadoBEComprar.PrecoFechamento;
                if (dadoBEComprar.PrecoFechamento < dadoBEVender.PrecoFechamento)
                    percentualGanhoPerda += variacaoEmReais;
                else
                    percentualGanhoPerda -= variacaoEmReais;
                //}
            }
            return percentualGanhoPerda;
        }*/

        internal static string RecuperarNomeRedeNeural(string papel, List<Versao> versoes, int tamanhoTendencia)
        {
            return String.Format("{0}_{1}_tamtend{2}", papel, versoes.Sum(ver => Convert.ToInt32(ver)), tamanhoTendencia);
        }

        internal static Network RecuperarRedeNeural(string papel, List<Versao> versoes, int tamanhoTendencia)
        {
            string nomeRede = RecuperarNomeRedeNeural(papel, versoes, tamanhoTendencia);
            using (Stream stream = File.Open(diretorioRedes + nomeRede + ".ndn", FileMode.Open))
            {
                IFormatter formatter = new BinaryFormatter();
                return (Network)formatter.Deserialize(stream);
            }
        }

        public static List<string> PegarTodosNomesRedes()
        {
            return Directory.GetFiles(diretorioRedes).Select(nomeRN => nomeRN.Split('\\').Last()).ToList();
        }
        private static KeyValuePair<string, Versao> RecuperarPapel_VersaoRedePorNome(string nomeRN)
        {
            string[] valores = nomeRN.Replace(".ndn", "").Split('_');
            return new KeyValuePair<string, Versao>(valores[0], (Versao)Convert.ToInt32(valores[1]));
        }
    }
}
