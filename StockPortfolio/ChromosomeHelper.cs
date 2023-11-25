using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPortfolio
{
    internal class ChromosomeHelper
    {
        public static string EncodeIndividualToChromosome(Individual individual)
        {
            string weight1Binary = DecimalToBinary(individual.Weight1);
            string weight2Binary = DecimalToBinary(individual.Weight2);
            string weight3Binary = DecimalToBinary(individual.Weight3);

            string chromosome = weight1Binary + weight2Binary + weight3Binary;

            return chromosome;
        }

        public static Individual DecodeChromosomeToIndividual(string chromosome)
        {
            string weight1Binary = chromosome.Substring(0, 8);
            string weight2Binary = chromosome.Substring(8, 8);
            string weight3Binary = chromosome.Substring(16, 8);

            byte weight1 = BinaryToDecimal(weight1Binary);
            byte weight2 = BinaryToDecimal(weight2Binary);
            byte weight3 = BinaryToDecimal(weight3Binary);

            return new Individual(weight1, weight2, weight3);
        }

        public static string DecimalToBinary(byte decimalNumber)
        {
            return Convert.ToString(decimalNumber, 2).PadLeft(8, '0');
        }

        public static byte BinaryToDecimal(string binaryNumber)
        {
            return Convert.ToByte(binaryNumber, 2);
        }
    }
}
