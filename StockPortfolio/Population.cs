using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StockPortfolio
{
    internal class Population
    {
        public List<Individual> Individuals { get; set; }
        private Random random = new Random();
        private int populationSize = GlobalParametrs.POPULATION_SIZE;
        public Population()
        {
            Individuals = new List<Individual>();
        }

        public void InitializePopulation()
        {
            for (int i = 0; i < populationSize; i++)
            {
                byte Weight1 = GetRandomWeight();
                byte Weight2 = RandomWeightInRange(1, 100 - Weight1);
                byte Weight3 = (byte)(100 - Weight1 - Weight2);
                Individual individual = new Individual(Weight1, Weight2, Weight3);
                Individuals.Add(individual);
            }
        }
        public int StartSimulation()
        {
            int counter = 0;
            int generationNumber = 1;
            double prevBest = 0;
            double prevAvg = 0; 
            while (true)
            {
                Console.WriteLine($"Номер поколения: {generationNumber}\t");
                var newPopulation = new List<Individual>();
                
                var parentPool = new List<Individual>();
                prevBest = Individuals.Select(x => x.Fitness).Max();
                prevAvg = Individuals.Select(x => x.Fitness).Average();
                PrintInfo();

                //int count = (int)(Individuals.Count * GlobalParametrs.ParentPoolPercent);
                int count = GlobalParametrs.PARENTPOOL_SIZE;
                parentPool = MyRouletteSelect(Individuals, count);
                //parentPool = TournamentSelect(Individuals, count);
                /*
                var orderedIndividuals = Individuals.OrderByDescending(x => x.Fitness).ToList();

                for (int i = 0; i < count; i++)
                {
                    parentPool.Add(orderedIndividuals[i]);
                }
                */
                for (int i = 0; i < GlobalParametrs.PARENTPOOL_SIZE; i++)
                {
                    for (int j = 0; j < GlobalParametrs.PARENTPOOL_SIZE; j++)
                    {
                        if (i != j)
                        {
                            Individual newIndividual = Crossover(parentPool[i], parentPool[j]);
                            newPopulation.Add(newIndividual);
                        }
                    }
                    if (newPopulation.Count >= GlobalParametrs.POPULATION_SIZE) break;
                }
                /*
                while (newPopulation.Count < GlobalParametrs.POPULATION_SIZE)
                {
                    Individual newIndividual = Crossover(parentPool[random.Next(0, count-1)], parentPool[random.Next(0, count-1)]);
                    newPopulation.Add(newIndividual);
                }         */
                Individuals = newPopulation;
                generationNumber++;

                double bestGrowth = Individuals.Select(x => x.Fitness).Max() - prevBest;
                double avgGrowth = Individuals.Select(x => x.Fitness).Average() - prevAvg;
                if (bestGrowth < 0.02 && bestGrowth > -0.02) counter++;
                else counter = 0;

                if (counter > 10) break;
                else Console.SetCursorPosition(0, 0);
                //Console.WriteLine("===============================");
                //
            }
            Console.WriteLine("Решение найдено!");
            PrintIndividuals();
            return generationNumber;
        }
        private void PrintIndividuals()
        {
            foreach(var individual in Individuals) 
            {
                Console.WriteLine($"{individual.Fitness}\t{individual.Weight1}\t{individual.Weight2}\t{individual.Weight3}");
            }
        }
        private void PrintInfo()
        {
            Console.WriteLine($"Средняя fitness: {Individuals.Select(x => x.Fitness).Average()}");
            Console.WriteLine($"Лучшая fitness: {Individuals.Select(x => x.Fitness).Max()}");
        }
        public Individual DoubleCrossover(Individual parent1, Individual parent2)
        {
            int crossoverPoint1 = random.Next(1, 12);
            int crossoverPoint2 = random.Next(crossoverPoint1 + 1, 23);

            string parent1Chromosome = parent1.Chromosome.Genes;
            string parent2Chromosome = parent2.Chromosome.Genes;

            string childChromosome = parent1Chromosome.Substring(0, crossoverPoint1) +
                                     parent2Chromosome.Substring(crossoverPoint1, crossoverPoint2 - crossoverPoint1) +
                                     parent1Chromosome.Substring(crossoverPoint2);
            childChromosome = MutateGenes(childChromosome);

            byte weigth1 = ChromosomeHelper.BinaryToDecimal(childChromosome.Substring(0, 8));
            byte weigth2 = ChromosomeHelper.BinaryToDecimal(childChromosome.Substring(8, 8));
            byte weigth3 = ChromosomeHelper.BinaryToDecimal(childChromosome.Substring(16));

            AdjustValues(ref weigth1, ref weigth2, ref weigth3);

            return new Individual(weigth1, weigth2, weigth3);
        }
        public Individual Crossover(Individual parent1, Individual parent2)
        {
            int crossoverPoint1 = random.Next(1, 23);

            string parent1Chromosome = parent1.Chromosome.Genes;
            string parent2Chromosome = parent2.Chromosome.Genes;

            string childChromosome = parent1Chromosome.Substring(0, crossoverPoint1) +
                                     parent2Chromosome.Substring(crossoverPoint1);
            childChromosome = MutateGenes(childChromosome);

            byte weigth1 = ChromosomeHelper.BinaryToDecimal(childChromosome.Substring(0, 8));
            byte weigth2 = ChromosomeHelper.BinaryToDecimal(childChromosome.Substring(8, 8));
            byte weigth3 = ChromosomeHelper.BinaryToDecimal(childChromosome.Substring(16));

            AdjustValues(ref weigth1, ref weigth2, ref weigth3);

            return new Individual(weigth1, weigth2, weigth3);
        }
        private void AdjustValues(ref byte value1, ref byte value2, ref byte value3)
        {
            float sum = value1 + value2 + value3;
            value1 = Convert.ToByte((value1 * 100) / sum);
            value2 = Convert.ToByte((value2 * 100) / sum);
            value3 = Convert.ToByte((value3 * 100) / sum);
            byte difference = (byte)(100 - (value1 + value2 + value3));
            
            if (value1 == 0 || value2 == 0 || value3 == 0)
            {
                byte maxValue = Math.Max(value1, Math.Max(value2, value3));
                if (value1 == 0)
                    value1++;
                else if (value2 == 0)
                    value2++;
                else if (value3 == 0)
                    value3++;

                if (maxValue == value1)
                    value1 -= 1;
                else if (maxValue == value2)
                    value2 -= 1;
                else
                    value3 -= 1;
            }                                                /*
            if (sum == 100) return;
            if (difference != 0)
            {
                byte maxValue = Math.Max(value1, Math.Max(value2, value3));

                if (maxValue == value1)
                {
                    value1 += difference;
                }
                else if (maxValue == value2)
                {
                    value2 += difference;
                }
                else
                {
                    value3 += difference;
                }
            }              */
        }
        private string MutateGenes(string genes)
        {
            char[] chromosome = genes.ToCharArray();
            for (int i = 0; i < chromosome.Length; i++)
            {
                if (random.Next(0, 100) == 1)
                {
                    chromosome[i] = (chromosome[i] == '0') ? '1' : '0';
                }
            }

            return new string(chromosome);
        }
        private byte GetRandomWeight()
        {
            return (byte)random.Next(1, 98);
        }
        private byte RandomWeightInRange(int min, int max)
        {
            return (byte)random.Next(min, max);
        }
        public List<Individual> TournamentSelect(List<Individual> individuals, int count)
        {
            List<Individual> individualsForSelection = new List<Individual>(individuals);
            List<Individual> selectedParents = new List<Individual>();


            while (selectedParents.Count < count)
            {
                Individual participant1 = individualsForSelection[random.Next(individualsForSelection.Count)];
                Individual participant2 = individualsForSelection[random.Next(individualsForSelection.Count)];

                Individual winner = (participant1.Fitness > participant2.Fitness) ? participant1 : participant2;

                selectedParents.Add(winner);
            }

            return selectedParents;
        }
        public List<Individual> MyRouletteSelect(List<Individual> individuals, int count)
        {
            List<Individual> individualsForSelection = new List<Individual>(individuals);
            List<Individual> selectedParents = new List<Individual>();
            while(selectedParents.Count < count)
            {
                List<double> ratios = GetRatios(individualsForSelection);
                int index = RouletteWheelSelection(ratios.ToArray());
                selectedParents.Add(individualsForSelection[index]);
                individualsForSelection.RemoveAt(index);
                //Console.ReadLine();
            }
            /*
            Console.WriteLine("РОДИТЕЛЬСКИЙ ПУЛ");
            selectedParents = selectedParents.OrderByDescending(x => x.Fitness).ToList();
            foreach(var item in selectedParents)
            {
                Console.WriteLine(item.Fitness);
            }
            */
            return selectedParents;
        }
        public int RouletteWheelSelection(double[] probabilities)
        {
            double randomValue = random.NextDouble();
            double cumulativeProbability = 0;

            for (int i = 0; i < probabilities.Length; i++)
            {
                cumulativeProbability += probabilities[i];
                if (randomValue <= cumulativeProbability)
                {
                    return i;
                }
            }

            return probabilities.Length - 1;
        }
        private List<double> GetRatios(List<Individual> individuals)
        {
            List<double> ratios = new List<double>();
            double totalFitness = individuals.Select(x => x.Fitness).Sum();
            foreach (var individual in individuals)
            {
                //Console.WriteLine($"{individual.Fitness / totalFitness}");
                ratios.Add(individual.Fitness / totalFitness);
            }
            return ratios;
        }

        /*
        public List<Individual> RouletteSelect(List<Individual> population, int numberOfParents)
        {
            List<Individual> selectedParents = new List<Individual>();

            double totalFitness = 0;
            foreach (Individual individual in population)
            {
                totalFitness += individual.Fitness;
            }

            Random random = new Random();

            for (int i = 0; i < numberOfParents; i++)
            {
                double randomValue = random.NextDouble() * totalFitness;
                double currentSum = 0;

                foreach (Individual individual in population)
                {
                    currentSum += individual.Fitness;

                    if (randomValue < currentSum + 1e-10)
                    {
                        selectedParents.Add(individual);
                        break;
                    }
                }
            }

            return selectedParents;
        }

        private List<Individual> Roulette(int count)
        {
            List<Individual> individualsCopy = new List<Individual>(Individuals);
            List<Individual> newPopulation = new List<Individual>();
            Random random = new Random();

            for (int i = 0; i < count; i++)
            {
                List<double> roulette = new List<double>();
                double sumFitness = 0;

                foreach (Individual individ in individualsCopy)
                {
                    sumFitness += individ.Fitness;
                }

                foreach (Individual individ in individualsCopy)
                {
                    roulette.Add(individ.Fitness / sumFitness);
                }

                double randomNumber = random.NextDouble();

                double tempNum = 0;
                int selectedIndividualIndex = -1;

                for (int j = 0; j < roulette.Count(); j++)
                {
                    tempNum += roulette[j];

                    if (randomNumber <= tempNum)
                    {
                        selectedIndividualIndex = j;
                        break;
                    }
                }

                if (selectedIndividualIndex != -1)
                {
                    newPopulation.Add(individualsCopy[selectedIndividualIndex]);
                    individualsCopy.RemoveAt(selectedIndividualIndex);
                }
                else
                {
                    Console.WriteLine("Error in roulette selection");
                }

                if (individualsCopy.Count == 0)
                {
                    Console.WriteLine("ROULETTE_POPULATION_SIZE > POPULATION_SIZE");
                    break;
                }
            }

            return newPopulation;
        }
        */
    }
}
