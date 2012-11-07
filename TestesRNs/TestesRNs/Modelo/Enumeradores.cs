using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TestesRNs.Modelo
{
    public enum Versao
    {
        [DescriptionAttribute("ValorBollinger")]
        V6001 = 6001,
        [DescriptionAttribute("MediaMovelSimples5Dias")]
        V6002 = 6002,
        [DescriptionAttribute("Williams_Percent_R_14P")]
        V6004 = 6004,
        [DescriptionAttribute("Williams_Percent_R_28P")]
        V6008 = 6008,
        [DescriptionAttribute("Arron_Up_Down")]
        V6016 = 6016,
        [DescriptionAttribute("DuracaoTendencias")]
        V6032 = 6032
    }
}
