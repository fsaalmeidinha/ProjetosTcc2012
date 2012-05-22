using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataBaseUtils.Model
{
    public class Treinamento
    {
        public Treinamento()
        {
            Input = new List<double>();
            Output = new List<double>();
        }
        public List<double> Input { get; set; }
        public List<double> Output { get; set; }
        public int DivisaoCrossValidation { get; set; }
    }
}
