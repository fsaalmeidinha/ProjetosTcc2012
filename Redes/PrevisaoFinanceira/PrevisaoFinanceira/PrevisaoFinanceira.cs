using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBaseUtils;
using NeuronDotNet.Core;

namespace PrevisaoFinanceiraHelper
{
    public class PrevisaoFinanceira
    {
        public static List<double> PreverCotacaoAtivo(string papel)
        {
            List<DadosBE> dadosBE = DataBaseUtils.DataBaseUtils.RecuperarCotacoesAtivo(papel);
            List<double> dadosNormalizados = DataBaseUtils.DataBaseUtils.NormalizarDados(dadosBE.Skip(dadosBE.Count - 90).Take(60).Select(dado => (double)dado.PrecoAbertura).ToList(), papel);
            List<string> redes = RedeNeural_PrevisaoFinanceira.RNAssessor.ListarRedes(papel);
            Dictionary<int, string> redePorDia = CaptacaoMelhoresRedes.RNAssessor.SelecionarRedePorDia(papel, dadosNormalizados.Take(60).ToList());

            foreach (KeyValuePair<int, string> redeDoDia in redePorDia)
            {
                Network rede = RedeNeural_PrevisaoFinanceira.RNAssessor.RecuperarRedeNeural(redeDoDia.Value);
                int janelaEntrada = Convert.ToInt32(redeDoDia.Value.Split(new string[] { "_je", "_js" }, StringSplitOptions.RemoveEmptyEntries)[1]);
                //int janelaSaida = Convert.ToInt32(redeDoDia.Value.Split(new string[] { "_js", "_nn" }, StringSplitOptions.RemoveEmptyEntries)[1]);

                double previsao = rede.Run(dadosNormalizados.Skip(dadosNormalizados.Count - janelaEntrada).ToArray())[0];
                //Adiciona a previsao aos dados
                dadosNormalizados.Add(previsao);
            }

            return DataBaseUtils.DataBaseUtils.DesnormalizarDados(dadosNormalizados.Skip(60).Take(30).ToList(), papel);
        }
    }
}
