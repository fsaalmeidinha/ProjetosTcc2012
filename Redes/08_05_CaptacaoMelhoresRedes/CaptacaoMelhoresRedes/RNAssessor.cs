using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedeNeural_PrevisaoFinanceira;
using NeuronDotNet.Core;
using CaptacaoMelhoresRedes.Model;
using DataBaseUtils;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CaptacaoMelhoresRedes
{
    public class RNAssessor
    {
        private static string diretorioRedesCaptacao
        {
            get
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["DiretorioRedesCaptacao"]))
                    return ConfigurationManager.AppSettings["DiretorioRedesCaptacao"];
                else
                    return System.IO.Directory.GetCurrentDirectory() + "\\RedesCaptacao\\";
            }
        }

        public void TreinarRedesCaptacao()
        {
            string papel = "PETR4";

            //Lista todas as configurações de rede para o papel
            List<string> redes = RedeNeural_PrevisaoFinanceira.RNAssessor.ListarRedes(papel);
            ConfiguracaoCaptacaoRedes configuracaoCaptacao = new ConfiguracaoCaptacaoRedes();

            //Recupera os dados do papel
            List<DadosBE> dadosBE = DataBaseUtils.DataBaseUtils.RecuperarCotacoesAtivo(papel);
            configuracaoCaptacao.Dados = DataBaseUtils.DataBaseUtils.NormalizarDados(dadosBE.Select(dadoBE => (double)dadoBE.PrecoAbertura).ToList(), papel);
            configuracaoCaptacao.Papel = papel;

            //Adicionar cada uma das redes ao treinamento de captação
            foreach (string nomeRede in redes)
            {
                Network redeNeuralPrevisaoFinanceira = RedeNeural_PrevisaoFinanceira.RNAssessor.RecuperarRedeNeural(nomeRede);
                RedePrevisaoFinanceira rpf = new RedePrevisaoFinanceira();
                rpf.NomeRede = nomeRede;
                rpf.JanelaEntrada = Convert.ToInt32(nomeRede.Split(new string[] { "_je", "_js" }, StringSplitOptions.RemoveEmptyEntries)[1]);
                rpf.JanelaSaida = Convert.ToInt32(nomeRede.Split(new string[] { "_js", "_nn" }, StringSplitOptions.RemoveEmptyEntries)[1]);
                rpf.RedeNeuralPrevisaoFinanceira = redeNeuralPrevisaoFinanceira;

                configuracaoCaptacao.RedesPrevisao.Add(rpf);
            }

            //Treina a rede de captação para a identificação da melhor dere por dia para o papel
            CaptacaoMelhoresRedesRN.Treinar(configuracaoCaptacao);
        }

        /// <summary>
        /// Diz para cada dia de previsão qual a rede neural que deve ser utilizada
        /// </summary>
        /// <param name="papel"></param>
        /// <param name="dadosNormalizados"></param>
        /// <returns></returns>
        public static Dictionary<int, string> SelecionarRedePorDia(string papel, List<double> dadosNormalizados)
        {
            //Lista todas as configurações de rede para o papel
            List<string> redes = RedeNeural_PrevisaoFinanceira.RNAssessor.ListarRedes(papel);

            Dictionary<int, string> redePorDia = new Dictionary<int, string>();

            for (int dia = 0; dia <= 29; dia++)
            {
                string nomeRede = CaptacaoMelhoresRedesRN.RecuperarNomeRede(papel, dia);
                Network rede = RecuperarRedeNeural(nomeRede);
                double[] output = rede.Run(dadosNormalizados.ToArray());
                redePorDia.Add(dia, redes[output.ToList().IndexOf(output.Max())]);
            }

            return redePorDia;
        }

        public static Network RecuperarRedeNeural(string nomeRede)
        {
            using (Stream stream = File.Open(diretorioRedesCaptacao + nomeRede + ".ndn", FileMode.Open))
            {
                IFormatter formatter = new BinaryFormatter();
                return (Network)formatter.Deserialize(stream);
            }
        }
    }
}
