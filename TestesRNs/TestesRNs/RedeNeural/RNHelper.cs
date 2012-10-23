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

        public static List<double[]> PreverAtivo(DateTime dataInicial, int dias, string nomeRN, out string papel, out Versao versao)
        {
            KeyValuePair<string, Versao> rn = RecuperarPapel_VersaoRedePorNome(nomeRN);
            papel = rn.Key;
            versao = rn.Value;
            return PreverAtivo(dataInicial, dias, rn.Key, rn.Value);
        }
        public static List<double[]> PreverAtivo(DateTime dataInicial, int dias, string papel, Versao versao)
        {
            List<DadoBE> dadosBE = DadoBE.PegarTodos(papel);
            DadoBE primeiroDadoPrever = dadosBE.Last(dado => dado.DataGeracao <= dataInicial);
            DadoBE dadoPrever = primeiroDadoPrever;
            List<DadoBE> dadosBEPrever = new List<DadoBE>();
            for (int i = 0; i < dias + 1; i++)
            {
                dadosBEPrever.Add(dadoPrever);
                dadoPrever = dadoPrever.Proximo;
            }

            //Recupera os treinamentos
            List<Treinamento> treinamentos = Treinamento.RecuperarTreinamentoRN(dadosBEPrever, versao).Normalizar(papel, versao);
            Network redeNeural = RecuperarRedeNeural(papel, versao);
            List<double[]> resultado = new List<double[]>();
            foreach (Treinamento treinamento in treinamentos)
            {
                List<double> outputPrevistoNormalizado = redeNeural.Run(treinamento.Input.ToArray()).ToList();
                List<double> outputPrevistoDesnormalizado = Treinamento.DesnormalizarSaidas(outputPrevistoNormalizado, papel, versao);
                //double outputPrevisto = DadoBE.DesnormalizarDado(outputPrevistoNormalizado[0], papel);
                //double outputEsperado = DadoBE.DesnormalizarDado(treinamento.Output[0], papel);
                //resultado.Add(new double[] { outputEsperado, outputPrevisto });
                resultado.Add(new double[] { Treinamento.DesnormalizarSaidas(treinamento.Output, papel, versao)[0], outputPrevistoDesnormalizado[0] });
            }

            //Func<double, double, double> tratarPercentual = (percent, valorDiaAnterior) => percent > 0.5 ?
            //    (valorDiaAnterior * (1 + Math.Abs(0.5 - percent))) :
            //    (valorDiaAnterior * (1 - Math.Abs(0.5 - percent)));
            Func<double, double, double> tratarPercentual = (percent, valorDiaAnterior) => valorDiaAnterior * (
                percent > 0.5 ? (1 + Math.Abs(0.5 - percent)) : (1 - Math.Abs(0.5 - percent))
                );

            switch (versao)
            {
                case Versao.V601:

                    break;
                default:
                    /*Tratar output percentual*/
                    DadoBE dadoBEPrevisao = primeiroDadoPrever;
                    for (int indPrevisao = 0; indPrevisao < dias; indPrevisao++)
                    {
                        resultado[indPrevisao][0] = tratarPercentual(resultado[indPrevisao][0], dadoBEPrevisao.Anterior.PrecoFechamento);
                        resultado[indPrevisao][1] = tratarPercentual(resultado[indPrevisao][1], dadoBEPrevisao.Anterior.PrecoFechamento);
                        dadoBEPrevisao = dadoBEPrevisao.Proximo;
                    }
                    /*Tratar output percentual*/
                    break;
            }

            return resultado;
        }

        internal static double TreinarRedeNeural(string papel, Versao versao)
        {
            List<DadoBE> dadosBE = DadoBE.PegarTodos(papel);
            if (versao == Versao.V603 || versao == Versao.V605)
            {
                dadosBE = dadosBE.Skip(dadosBE.Count / 10 * 7).ToList();
            }


            List<Treinamento> treinamentos = Treinamento.RecuperarTreinamentoRN(dadosBE, versao).Normalizar(papel, versao, true);
            //ignorar elementos finais, permitindo testes
            treinamentos = treinamentos.Take(treinamentos.Count / 8 * 7).ToList();

            string nomeRedeNeural = RecuperarNomeRedeNeural(papel, versao);
            int numeroNeuronios = 1;
            double taxaAprendizado = 0.1d;//0.25
            int ciclos = 10000000;//2000
            double erroMedio = RNCore.Treinar(nomeRedeNeural, treinamentos, numeroNeuronios, taxaAprendizado, ciclos);

            return erroMedio;
        }

        internal static string RecuperarNomeRedeNeural(string papel, Versao versao)
        {
            return String.Format("{0}_{1}", papel, Convert.ToInt32(versao));
        }

        private static Network RecuperarRedeNeural(string papel, Versao versao)
        {
            string nomeRede = RecuperarNomeRedeNeural(papel, versao);
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
