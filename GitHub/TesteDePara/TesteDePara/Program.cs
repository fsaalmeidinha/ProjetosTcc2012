using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConverterTabela;
using System.Collections;

namespace TesteDePara
{
    class Program
    {
        static void Main(string[] args)
        {
            Converter dePara = new Converter();
            ICollection<object> list = new List<object>();

            list = dePara.DePara("petr4");

            Console.Read();

        }
    }
}
