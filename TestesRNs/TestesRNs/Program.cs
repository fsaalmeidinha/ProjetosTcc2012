using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestesRNs.Modelo;
using TestesRNs.RedeNeural;

namespace TestesRNs
{
    class Program
    {
        static void Main(string[] args)
        {
            string papel = "PETR4";//PETR4,ETER3,NATU3
            Versao versao = Versao.V604;
            //DateTime dataInicialPrevisao = new DateTime(2011, 09, 10);
            //int qtdDiasPrever = 60;

            //List<DadoBE> dadosBE = DadoBE.PegarTodos("PETR4");
            //List<Treinamento> treinamentos = Treinamento.RecuperarTreinamentoRN(dadosBE, TestesRNs.Modelo.Treinamento.Versao.V6);


            double erroMedio = RNHelper.TreinarRedeNeural(papel, versao);



            //List<double[]> previsao = RNHelper.PreverAtivo(papel, dataInicialPrevisao, qtdDiasPrever, versao);
            //string res = String.Join(" ; ", previsao.Select(prev => String.Format("({0} - {1}) \n", prev[0], prev[1])));
        }
    }
}
