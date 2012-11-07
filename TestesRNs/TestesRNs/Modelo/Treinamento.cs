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
        internal static List<double> RecuperarValoresMaxMin(string papel, List<Versao> versoes, int tamanhoTendencia)
        {
            string nomeRede = RNHelper.RecuperarNomeRedeNeural(papel, versoes, tamanhoTendencia);
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
        public DateTime Data { get; set; }

        public static List<Treinamento> RecuperarTreinamentoRN(List<DadoBE> dadosBE, List<Versao> versoes, int tamanhoTendencia)
        {
            List<Treinamento> treinamentos = new List<Treinamento>();
            for (int i = 0; i < dadosBE.Count - 1; i++)
            {
                Treinamento treinamento = new Treinamento()
                    {
                        Data = dadosBE[i].DataGeracao
                    };

                foreach (Versao versao in versoes)
                {
                    switch (versao)
                    {
                        case Versao.V6001:
                            treinamento.Input.AddRange(dadosBE[i].ValorBollinger);
                            break;
                        case Versao.V6002:
                            treinamento.Input.AddRange(dadosBE[i].AnaliseMediaMovelSimples5Dias);
                            break;
                        case Versao.V6004:
                            treinamento.Input.AddRange(dadosBE[i].AnaliseWilliams_Percent_R_14P);
                            break;
                        case Versao.V6008:
                            treinamento.Input.AddRange(dadosBE[i].AnaliseWilliams_Percent_R_28P);
                            break;
                        case Versao.V6016:
                            treinamento.Input.AddRange(dadosBE[i].AnaliseArron_Up_Down);
                            break;
                        case Versao.V6032:
                            treinamento.Input.AddRange(dadosBE[i].DuracaoTendencias);
                            break;
                        default:
                            throw new Exception();
                            break;
                    }
                }

                //double variacao = Math.Abs(dadosBE[i].Anterior.PrecoFechamento - dadosBE[i].PrecoFechamento) / dadosBE[i].Anterior.PrecoFechamento;
                //if (dadosBE[i].Anterior.PrecoFechamento > dadosBE[i].PrecoFechamento)
                //    treinamento.Output.Add(0.5 - variacao);
                //else
                //    treinamento.Output.Add(0.5 + variacao);


                //if (dadosBE[i].Anterior.PrecoFechamento > dadosBE[i].PrecoFechamento)
                //    treinamento.Output.AddRange(new List<double>() { 0, 1 });
                //else
                //    treinamento.Output.AddRange(new List<double>() { 1, 0 });

                double variacao = 0;
                for (int j = i; j < i + tamanhoTendencia && j < dadosBE.Count; j++)
                {
                    if (dadosBE[j].Proximo != null)
                        variacao += dadosBE[j].Proximo.PrecoFechamento - dadosBE[j].PrecoFechamento;
                }

                //Tendencia de alta
                if (variacao > 0)
                {
                    treinamento.Output.AddRange(new List<double>() { 1, 0 });
                }
                //Tendencia de baixa
                else
                {
                    treinamento.Output.AddRange(new List<double>() { 0, 1 });
                }

                treinamentos.Add(treinamento);
            }

            return treinamentos.Where(trein => trein.Input.Any(inp => inp > 0)).ToList();
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

                treinamentoClone.Data = treinamento.Data;
                treinamentosRetornar.Add(treinamentoClone);
            }
            return treinamentosRetornar;
        }
        #region Normalizacao de entradas e saidas
        static Func<double, double, double, double> normalizar = (max, min, valor) => (valor - min) / (max - min);
        static Func<double, double, double, double> desnormalizar = (max, min, valor) => valor * (max - min) + min;

        public static List<Treinamento> NormalizarEntradasESaidas(List<Treinamento> treinamentos, string papel, List<Versao> versoes, int tamanhoTendencia, bool sobreescreverConfTexto = false)
        {
            List<Treinamento> treinamentosClone = Treinamento.ClonarTreinamentos(treinamentos);
            List<double> valoresNormalizacao = sobreescreverConfTexto ? null : RecuperarValoresMaxMin(papel, versoes, tamanhoTendencia);
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
                    //maxVal = maxVal > 3 * media ? media * 3 : maxVal;

                    minVal = treinamentosClone.Min(trein => trein.Input[indInput]);
                    minVal = minVal < 0 ? 0 : minVal;
                    //minVal = minVal < media / 3 ? media / 3 : minVal;
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
                    maxVal = treinamentosClone.Max(trein => trein.Output[indOutput]);
                    minVal = treinamentosClone.Min(trein => trein.Output[indOutput]);
                    minVal = minVal < 0 ? 0 : minVal;
                }

                treinamentosClone.ForEach(trein => trein.Output[indOutput] = normalizar(maxVal, minVal, trein.Output[indOutput]));

                valoresNormalizacaoOutputStr.Add(String.Format("{0}-{1}", maxVal.ToString(), minVal.ToString()));
            }

            string valoresInput = String.Join("&", valoresNormalizacaoInputStr);
            string valoresOutput = String.Join("&", valoresNormalizacaoOutputStr);
            if (sobreescreverConfTexto)
            {
                StreamWriter sw = new StreamWriter(diretorioRedes + "\\" + RNHelper.RecuperarNomeRedeNeural(papel, versoes, tamanhoTendencia) + "_ValoresMinMax.txt");
                sw.WriteLine(valoresInput);
                sw.WriteLine(valoresOutput);
                sw.Close();
            }

            return treinamentosClone;
        }

        public static List<Treinamento> DesnormalizarEntradasESaidas(List<Treinamento> treinamentos, string papel, List<Versao> versoes, int tamanhoTendencia)
        {
            List<Treinamento> treinamentosClone = Treinamento.ClonarTreinamentos(treinamentos);
            List<double> valoresMinMaxDesnormalizacao = RecuperarValoresMaxMin(papel, versoes, tamanhoTendencia);

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

        public static List<double> DesnormalizarSaidas(List<double> saidas, string papel, List<Versao> versoes, int tamanhoTendencia)
        {
            List<double> retorno = new List<double>();
            List<double> valoresMinMaxDesnormalizacao = RecuperarValoresMaxMin(papel, versoes, tamanhoTendencia);
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
        public static List<Treinamento> Normalizar(this List<Treinamento> treinamentos, string papel, List<Versao> versoes, int tamanhoTendencia, bool sobreescreverConfTexto = false)
        {
            return Treinamento.NormalizarEntradasESaidas(treinamentos, papel, versoes, tamanhoTendencia, sobreescreverConfTexto);
        }
        public static List<Treinamento> Desnormalizar(this List<Treinamento> treinamentos, string papel, List<Versao> versoes, int tamanhoTendencia)
        {
            return Treinamento.DesnormalizarEntradasESaidas(treinamentos, papel, versoes, tamanhoTendencia);
        }
    }
}
