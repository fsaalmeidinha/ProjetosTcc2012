using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using HelloWorld.Model;

namespace HelloWorld.RN
{
    public class CotacaoBovespaRN
    {
        static void Main(string[] args)
        {
            //LerCotacoes();
            LerCotacoesPorPapel("PETR4T");
        }

        public static List<Cotacao> LerCotacoesPorPapel(string papel, string directory = @"C:/Users/felipe.almeida1/Desktop/DadosTreinamento/Consumidos_Download/")
        {
            int id = 1;
            List<Cotacao> cotacoes = new List<Cotacao>();

            for (int ano = 2006; ano <= DateTime.Today.Year; ano++)
            {
                for (int mes = 1; mes <= 12; mes++)
                {
                    for (int dia = 1; dia <= 31; dia++)
                    {
                        string currentFile = String.Format(directory + "{0}_{1}/COTAHIST_D{2}{3}{0}.txt", ano, mes, dia.ToString("00"), mes.ToString("00"));

                        if (!System.IO.File.Exists(currentFile))
                        {
                            currentFile = String.Format(directory + "{0}_{1}/COTAHIST_D{2}{3}{0}.txt", ano, mes, dia.ToString("00"), mes);
                            if (!System.IO.File.Exists(currentFile))
                            {
                                currentFile = String.Format(directory + "{0}_{1}/COTAHIST_D{2}{3}{0}.txt", ano, mes, dia, mes.ToString("00"));
                                if (!System.IO.File.Exists(currentFile))
                                {
                                    currentFile = String.Format(directory + "{0}_{1}/COTAHIST_D{2}{3}{0}.txt", ano, mes, dia, mes);
                                    continue;
                                }
                            }
                        }

                        using (StreamReader sr = new StreamReader(currentFile))
                        {
                            string arquivo = sr.ReadToEnd();
                            arquivo = arquivo.Substring(0, arquivo.IndexOf("99COTAHIST.") - 1);//Remove o 'footer'
                            arquivo = arquivo.Substring(31, arquivo.Length - 31);
                            int indexInicioArquivo = arquivo.IndexOf('0') - 1;
                            arquivo = arquivo.Substring(indexInicioArquivo, arquivo.Length - indexInicioArquivo);

                            //arquivo = arquivo.Substring(245, arquivo.Length - (246 * 2));//Remove o 'cabeçolho' e o 'footer'
                            arquivo = arquivo.Replace("\r\n", "");
                            arquivo = arquivo.Replace("\n", "");
                            arquivo = arquivo.Replace("\r", "");

                            while (arquivo.Length > 0)
                            {
                                int indicePapel = arquivo.IndexOf(papel.ToString()) - 12;
                                if (indicePapel < 0)
                                    break;
                                arquivo = arquivo.Substring(indicePapel, arquivo.Length - indicePapel);

                                Cotacao cb = new Cotacao();
                                cb.Id = id++;
                                cb.Data = new DateTime(Convert.ToInt32(arquivo.Substring(2, 4)), Convert.ToInt32(arquivo.Substring(6, 2)), Convert.ToInt32(arquivo.Substring(8, 2)));
                                cb.Codigo = arquivo.Substring(12, 12).Trim(); //13 24
                                cb.Valor = (float)Convert.ToDouble(arquivo.Substring(56, 13).Insert(11, ","));
                                cotacoes.Add(cb);
                                break;
                                //arquivo = arquivo.Substring(245, arquivo.Length - 245);
                            }
                        }
                    }
                }
            }

            return cotacoes;
        }

        public static List<Cotacao> LerCotacoes(string directory = @"C:/Felipe/TCC/DadosTreinamento/Consumidos_Download/")
        {
            //@"C:\Felipe\TCC\DadosTreinamento\Consumidos_Download\2012_4\3\COTAHIST_D03042012.txt"
            //string path = directory + @"\2012_4\3\COTAHIST_D03042012.txt";

            int id = 1;
            List<Cotacao> cotacoes = new List<Cotacao>();

            for (int ano = 2006; ano <= DateTime.Today.Year; ano++)
            {
                for (int mes = 1; mes <= 12; mes++)
                {
                    for (int dia = 1; dia <= 31; dia++)
                    {

                        string currentFile = String.Format(directory + "{0}_{1}/COTAHIST_D{2}{3}{0}.txt", ano, mes, dia.ToString("00"), mes.ToString("00"));

                        if (!System.IO.File.Exists(currentFile))
                        {
                            currentFile = String.Format(directory + "{0}_{1}/COTAHIST_D{2}{3}{0}.txt", ano, mes, dia.ToString("00"), mes);
                            if (!System.IO.File.Exists(currentFile))
                            {
                                currentFile = String.Format(directory + "{0}_{1}/COTAHIST_D{2}{3}{0}.txt", ano, mes, dia, mes.ToString("00"));
                                if (!System.IO.File.Exists(currentFile))
                                {
                                    currentFile = String.Format(directory + "{0}_{1}/COTAHIST_D{2}{3}{0}.txt", ano, mes, dia, mes);
                                    continue;
                                }
                            }
                        }

                        using (StreamReader sr = new StreamReader(currentFile))
                        {
                            string arquivo = sr.ReadToEnd();
                            arquivo = arquivo.Substring(0, arquivo.IndexOf("99COTAHIST.") - 1);//Remove o 'footer'
                            arquivo = arquivo.Substring(31, arquivo.Length - 31);
                            int indexInicioArquivo = arquivo.IndexOf('0') - 1;
                            arquivo = arquivo.Substring(indexInicioArquivo, arquivo.Length - indexInicioArquivo);

                            //arquivo = arquivo.Substring(245, arquivo.Length - (246 * 2));//Remove o 'cabeçolho' e o 'footer'
                            arquivo = arquivo.Replace("\r\n", "");
                            arquivo = arquivo.Replace("\n", "");
                            arquivo = arquivo.Replace("\r", "");

                            while (arquivo.Length > 0)
                            {
                                Cotacao cb = new Cotacao();
                                //cb.TipoRegistro = Convert.ToInt16(arquivo.Substring(0, 2));
                                cb.Id = id++;
                                cb.Data = new DateTime(Convert.ToInt32(arquivo.Substring(2, 4)), Convert.ToInt32(arquivo.Substring(6, 2)), Convert.ToInt32(arquivo.Substring(8, 2)));
                                cb.Codigo = arquivo.Substring(12, 12).Trim(); //13 24
                                cb.Valor = (float)Convert.ToDouble(arquivo.Substring(56, 13).Insert(11, ","));
                                cotacoes.Add(cb);
                                arquivo = arquivo.Substring(245, arquivo.Length - 245);
                            }
                        }
                    }
                }
            }

            return cotacoes;
        }

        public static void RealimentarBase()
        {
            List<Cotacao> cotacoes = LerCotacoes();
            CotacoesBDEntities ent = new CotacoesBDEntities();
            ent.Cotacoes.ToList().ForEach(cot => ent.DeleteObject(cot));
            ent.SaveChanges();

            cotacoes.ForEach(cot => ent.AddToCotacoes(cot));
            ent.SaveChanges();
        }
    }
}
