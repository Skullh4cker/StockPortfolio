using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPortfolio
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Population population = new Population();
            population.InitializePopulation();
            population.StartSimulation();

            Console.ReadLine();
        }
    }
}
