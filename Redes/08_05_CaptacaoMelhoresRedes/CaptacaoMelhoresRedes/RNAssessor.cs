using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedeNeural_PrevisaoFinanceira;
using NeuronDotNet.Core;
using CaptacaoMelhoresRedes.Model;
using DataBaseUtils;

namespace CaptacaoMelhoresRedes
{
    public class RNAssessor
    {
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

        //public Dictionary<int,string> SelecionarRedePorDia(string papel, List<>)
        //{

        //}
    }
}
