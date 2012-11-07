using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AnaliseTecnica.RN;
using AnaliseTecnica.Modelo;

namespace TomadaDecisaoView
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            PreencherGVResumo();
        }

        private void PreencherGVResumo()
        {
            List<ResultadoTomadaDecisao> resultados = TomadaDecisao.TomarDecisoes(new DateTime(2000, 1, 1), new DateTime(2013, 1, 1));
            foreach (ResultadoTomadaDecisao resultado in resultados)
            {
                ((Label)this.Controls.Find("lblTotalNeg" + resultado.Papel, true).First()).Text = resultado.TotalNegociacoesDeCompra.ToString();
                ((Label)this.Controls.Find("lblTotalAcertos" + resultado.Papel, true).First()).Text = resultado.TotalAcertos.ToString();
                ((GroupBox)this.Controls.Find("gb" + resultado.Papel, true).First()).Text = String.Format("{0} ({1} - {2}) - {3} dados", resultado.Papel, resultado.DataInicial.ToShortDateString(), resultado.DataFinal.ToShortDateString(), resultado.TotalDados);

                Label percentAcerto = ((Label)this.Controls.Find("lblPercentualAcerto" + resultado.Papel, true).First());
                percentAcerto.Text = resultado.PercentualAcerto.ToString("N2") + "%";
                if (resultado.PercentualAcerto > 50)
                    percentAcerto.ForeColor = Color.Blue;
                else
                    percentAcerto.ForeColor = Color.Red;

                Label percentualGanhoPerda = ((Label)this.Controls.Find("lblpercentualGanhoPerda" + resultado.Papel, true).First());
                percentualGanhoPerda.Text = resultado.PercentualGanhoPerda.ToString("N2") + "%";
                if (resultado.PercentualGanhoPerda > 0)
                    percentualGanhoPerda.ForeColor = Color.Blue;
                else
                    percentualGanhoPerda.ForeColor = Color.Red;

                Label percentualMedioganhoPerda = ((Label)this.Controls.Find("lblPercentualMedioganhoPerda" + resultado.Papel, true).First());
                percentualMedioganhoPerda.Text = resultado.PercentualMedioGanhoPerda.ToString("N2") + "%";
                if (resultado.PercentualMedioGanhoPerda > 0)
                    percentualMedioganhoPerda.ForeColor = Color.Blue;
                else
                    percentualMedioganhoPerda.ForeColor = Color.Red;
            }
        }
    }
}
