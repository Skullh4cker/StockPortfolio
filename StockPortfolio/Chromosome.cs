using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPortfolio
{
    internal class Chromosome
    {
        public string Genes { get; set; }
        public Chromosome(string genes)
        {
            Genes = genes;
        }
    }
}
