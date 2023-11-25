using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPortfolio
{
    internal class Individual
    {
        public byte Weight1 { get; set; }
        public byte Weight2 { get; set; }
        public byte Weight3 { get; set; }
        public double Fitness { get; set; }
        public Chromosome Chromosome { get; set; }

        public Individual(byte weight1, byte weight2, byte weight3)
        {
            Weight1 = weight1;
            Weight2 = weight2;
            Weight3 = weight3;
            Fitness = CalculateFitness();
            string chromosomeString = ChromosomeHelper.EncodeIndividualToChromosome(this);
            Chromosome = new Chromosome(chromosomeString);
        }
        public double CalculateFitness()
        {
            double fitness = (((double)Weight1 / 100) * GlobalParametrs.E1) + (((double)Weight2 / 100) * GlobalParametrs.E2) + (((double)Weight3 / 100) * GlobalParametrs.E3);
            return fitness;
        }
        public static double GetBestFitness()
        {
            double fitness = (((double)1 / 100) * GlobalParametrs.E1) + (((double)1 / 100) * GlobalParametrs.E2) + (((double)98 / 100) * GlobalParametrs.E3);
            return fitness;
        }
    }
}
