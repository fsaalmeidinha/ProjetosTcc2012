using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestesRNs.RedeNeural;
using System.IO;
using System.Configuration;

namespace TestesRNs.Modelo
{
    public class Treinamento
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
        static Dictionary<string, List<double>> ValoresMaxMinGlobal = new Dictionary<string, List<double>>();
        internal static List<double> RecuperarValoresMaxMin(string papel, Versao versao)
        {
            string nomeRede = RNHelper.RecuperarNomeRedeNeural(papel, versao);
            if (ValoresMaxMinGlobal.ContainsKey(nomeRede))
            {
                return ValoresMaxMinGlobal[nomeRede];
            }

            string arquivo = Directory.GetFiles(diretorioRedes, "*.txt").FirstOrDefault(rn => rn.Contains(nomeRede + "_ValoresMinMax"));
            if (String.IsNullOrEmpty(arquivo))
                return null;

            StreamReader sr = new StreamReader(arquivo);
            string inputsStr = sr.ReadLine();
            string outputsStr = sr.ReadLine();
            sr.Close();

            List<double> valoresMinMax = new List<double>();
            foreach (string parDeValores in inputsStr.Split('&'))
            {
                string[] max_min = parDeValores.Split('-');
                valoresMinMax.Add(Convert.ToDouble(max_min[0]));
                valoresMinMax.Add(Convert.ToDouble(max_min[1]));
            }
            foreach (string parDeValores in outputsStr.Split('&'))
            {
                string[] max_min = parDeValores.Split('-');
                valoresMinMax.Add(Convert.ToDouble(max_min[0]));
                valoresMinMax.Add(Convert.ToDouble(max_min[1]));
            }

            ValoresMaxMinGlobal.Add(nomeRede, valoresMinMax);
            return valoresMinMax;
        }

        public Treinamento()
        {
            Input = new List<double>();
            Output = new List<double>();
        }
        public List<double> Input { get; set; }
        public List<double> Output { get; set; }

        public static List<Treinamento> RecuperarTreinamentoRN(List<DadoBE> dadosBE, Versao versao)
        {
            List<Treinamento> treinamentos = new List<Treinamento>();
            for (int i = 0; i < dadosBE.Count - 1; i++)
            {
                Treinamento treinamento = new Treinamento();
                if (versao == Versao.V601)
                    treinamento.Input.Add(dadosBE[i].PrecoFechamento);

                if (versao == Versao.V601 || versao == Versao.V604 || versao == Versao.V605)
                {
                    treinamento.Input.Add(dadosBE[i].ValorBollinger);
                }

                if (versao == Versao.V602 || versao == Versao.V603)
                {
                    treinamento.Input.AddRange(dadosBE[i].DuracaoTendencias);
                }

                if (versao == Versao.V601)
                    treinamento.Output.Add(dadosBE[i].Proximo.PrecoFechamento);
                else
                {
                    double variacao = Math.Abs(dadosBE[i].Anterior.PrecoFechamento - dadosBE[i].PrecoFechamento) / dadosBE[i].Anterior.PrecoFechamento;
                    if (dadosBE[i].Anterior.PrecoFechamento > dadosBE[i].PrecoFechamento)
                        treinamento.Output.Add(0.5 - variacao);
                    else
                        treinamento.Output.Add(0.5 + variacao);
                    //if (dadosBE[i].Anterior.PrecoFechamento > dadosBE[i].PrecoFechamento)
                    //    treinamento.Output.AddRange(new List<double>() { 0, 1 });
                    //else
                    //    treinamento.Output.AddRange(new List<double>() { 1, 0 });
                }

                treinamentos.Add(treinamento);
            }

            return treinamentos;
        }

        internal static List<Treinamento> ClonarTreinamentos(List<Treinamento> treinamentos)
        {
            List<Treinamento> treinamentosRetornar = new List<Treinamento>();
            foreach (Treinamento treinamento in treinamentos)
            {
                Treinamento treinamentoClone = new Treinamento();
                foreach (double valInput in treinamento.Input)
                {
                    treinamentoClone.Input.Add(valInput);
                }
                foreach (double valOutput in treinamento.Output)
                {
                    treinamentoClone.Output.Add(valOutput);
                }

                treinamentosRetornar.Add(treinamentoClone);
            }
            return treinamentosRetornar;
        }
        #region Normalizacao de entradas e saidas
        static Func<double, double, double, double> normalizar = (max, min, valor) => (valor - min) / (max - min);
        static Func<double, double, double, double> desnormalizar = (max, min, valor) => valor * (max - min) + min;

        public static List<Treinamento> NormalizarEntradasESaidas(List<Treinamento> treinamentos, string papel, Versao versao, bool sobreescreverConfTexto = false)
        {
            List<Treinamento> treinamentosClone = Treinamento.ClonarTreinamentos(treinamentos);
            List<double> valoresNormalizacao = sobreescreverConfTexto ? null : RecuperarValoresMaxMin(papel, versao);
            List<string> valoresNormalizacaoInputStr = new List<string>();
            List<string> valoresNormalizacaoOutputStr = new List<string>();

            int inputLength = treinamentosClone.First().Input.Count;
            int outputLength = treinamentosClone.First().Output.Count;
            for (int indInput = 0; indInput < inputLength; indInput++)
            {
                double maxVal = 0;
                double minVal = 0;
                if (valoresNormalizacao != null)
                {
                    maxVal = valoresNormalizacao[0];
                    minVal = valoresNormalizacao[1];
                    valoresNormalizacao = valoresNormalizacao.Skip(2).ToList();
                }
                else
                {
                    double media = treinamentosClone.Average(trein => trein.Input[indInput]);

                    maxVal = treinamentosClone.Max(trein => trein.Input[indInput]);
                    maxVal = maxVal > 3 * media ? media * 3 : maxVal;

                    minVal = treinamentosClone.Min(trein => trein.Input[indInput]);
                    minVal = minVal < 0 ? 0 : minVal;
                    minVal = minVal < media / 3 ? media / 3 : minVal;
                }

                treinamentosClone.ForEach(trein => trein.Input[indInput] = normalizar(maxVal, minVal, trein.Input[indInput]));
                treinamentosClone.ForEach(trein => trein.Input[indInput] = trein.Input[indInput] > 1 ? 1 : trein.Input[indInput]);
                treinamentosClone.ForEach(trein => trein.Input[indInput] = trein.Input[indInput] < 0 ? 0 : trein.Input[indInput]);

                valoresNormalizacaoInputStr.Add(String.Format("{0}-{1}", maxVal.ToString(), minVal.ToString()));
            }
            for (int indOutput = 0; indOutput < outputLength; indOutput++)
            {
                double maxVal = 0;
                double minVal = 0;
                if (valoresNormalizacao != null)
                {
                    maxVal = valoresNormalizacao[0];
                    minVal = valoresNormalizacao[1];
                    valoresNormalizacao = valoresNormalizacao.Skip(2).ToList();
                }
                else
                {
                    maxVal = treinamentosClone.Max(trein => trein.Input[indOutput]);
                    minVal = treinamentosClone.Min(trein => trein.Input[indOutput]);
                    minVal = minVal < 0 ? 0 : minVal;
                }

                treinamentosClone.ForEach(trein => trein.Output[indOutput] = normalizar(maxVal, minVal, trein.Output[indOutput]));

                valoresNormalizacaoOutputStr.Add(String.Format("{0}-{1}", maxVal.ToString(), minVal.ToString()));
            }

            string valoresInput = String.Join("&", valoresNormalizacaoInputStr);
            string valoresOutput = String.Join("&", valoresNormalizacaoOutputStr);
            if (sobreescreverConfTexto)
            {
                StreamWriter sw = new StreamWriter(diretorioRedes + "\\" + RNHelper.RecuperarNomeRedeNeural(papel, versao) + "_ValoresMinMax.txt");
                sw.WriteLine(valoresInput);
                sw.WriteLine(valoresOutput);
                sw.Close();
            }

            return treinamentosClone;
        }

        public static List<Treinamento> DesnormalizarEntradasESaidas(List<Treinamento> treinamentos, string papel, Versao versao)
        {
            List<Treinamento> treinamentosClone = Treinamento.ClonarTreinamentos(treinamentos);
            List<double> valoresMinMaxDesnormalizacao = RecuperarValoresMaxMin(papel, versao);

            //return dado * (max - min) + min;

            int inputLength = treinamentosClone.First().Input.Count;
            int outputLength = treinamentosClone.First().Output.Count;
            for (int indInput = 0; indInput < inputLength; indInput++)
            {
                double maxVal = valoresMinMaxDesnormalizacao[0];
                double minVal = valoresMinMaxDesnormalizacao[1];
                valoresMinMaxDesnormalizacao = valoresMinMaxDesnormalizacao.Skip(2).ToList();

                treinamentosClone.ForEach(trein => trein.Input[indInput] = desnormalizar(maxVal, minVal, trein.Input[indInput]));
            }
            for (int indOutput = 0; indOutput < outputLength; indOutput++)
            {
                double maxVal = valoresMinMaxDesnormalizacao[0];
                double minVal = valoresMinMaxDesnormalizacao[1];
                valoresMinMaxDesnormalizacao = valoresMinMaxDesnormalizacao.Skip(2).ToList();

                treinamentosClone.ForEach(trein => trein.Output[indOutput] = desnormalizar(maxVal, minVal, trein.Output[indOutput]));
            }

            return treinamentosClone;
        }

        public static List<double> DesnormalizarSaidas(List<double> saidas, string papel, Versao versao)
        {
            List<double> retorno = new List<double>();
            List<double> valoresMinMaxDesnormalizacao = RecuperarValoresMaxMin(papel, versao);
            valoresMinMaxDesnormalizacao = valoresMinMaxDesnormalizacao.Skip(valoresMinMaxDesnormalizacao.Count - (saidas.Count * 2)).ToList();

            for (int indOutput = 0; indOutput < saidas.Count; indOutput++)
            {
                double maxVal = valoresMinMaxDesnormalizacao[0];
                double minVal = valoresMinMaxDesnormalizacao[1];
                valoresMinMaxDesnormalizacao = valoresMinMaxDesnormalizacao.Skip(2).ToList();

                saidas.ForEach(saida => retorno.Add(desnormalizar(maxVal, minVal, saida)));
            }

            return retorno;
        }

        #endregion Normalizacao de indices
    }

    public static class Extensao
    {
        public static List<Treinamento> Normalizar(this List<Treinamento> treinamentos, string papel, Versao versao, bool sobreescreverConfTexto = false)
        {
            return Treinamento.NormalizarEntradasESaidas(treinamentos, papel, versao, sobreescreverConfTexto);
        }
        public static List<Treinamento> Desnormalizar(this List<Treinamento> treinamentos, string papel, Versao versao)
        {
            return Treinamento.DesnormalizarEntradasESaidas(treinamentos, papel, versao);
        }
    }
}
