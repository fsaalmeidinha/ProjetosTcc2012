﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RelatorioRNs.Model
{
    public class Relatorio
    {
        public Relatorio()
        {
            RelatoriosDia = new List<RelatorioDia>();
        }

        public string Papel { get; set; }
        public string NomeRN { get; set; }
        public List<RelatorioDia> RelatoriosDia { get; set; }
        public double ErroAcumulado { get; set; }
        public double ErroMedio { get; set; }
        public string AcompanhouTendencia { get; set; }

        public class RelatorioDia
        {
            public double Real { get; set; }
            public double Previsto { get; set; }
            public double Erro { get; set; }
            public bool AcompanhouTendencia { get; set; }
        }
    }
}
