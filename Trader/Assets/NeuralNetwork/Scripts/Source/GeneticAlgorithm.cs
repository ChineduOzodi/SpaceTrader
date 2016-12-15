/// <copyright file="GeneticAlgorithm.cs">Copyright (c) 2016 All Rights Reserved</copyright>
/// <author>Chinedu Ozodi</author>
/// <date>12/12/2016</date>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NeuralNetwork
{
    /// <summary>
    /// The genetic algorythm that mutates the weights in the neural network
    /// </summary>
    public class GeneticAlgorithm
    {
        /// <summary>
        /// List of the populations genomes/neural weights
        /// </summary>
        public List<Genome> population;

        private int popSize;
        private int chromoLength;
        private double totalFitness = 0;

        public double bestFitness, averageFitness = 0;
        public double worstFitness = 9999999999999999;

        public int fittestGenomeIndex = 0;

        /// <summary>
        /// Mutation rate
        /// </summary>
        public static double mutationRate = .2;
        /// <summary>
        /// Crossover rate
        /// </summary>
        public static double crossOverRate = .7;

        /// <summary>
        /// The generation count
        /// </summary>
        public int generation;

        /// <summary>
        /// Creates the gene alg class with desired population size, mutation rate, crossover rate, and number of weight
        /// </summary>
        /// <param name="_popSize">Size of population to test</param>
        /// <param name="mutRat"> mutation rate</param>
        /// <param name="crossRate">crossover rate</param>
        /// <param name="numWeights">number of weights in gene, gotten from the GetNumWeights function in the NeuralNet class</param>
        public GeneticAlgorithm(int _popSize, double mutRat, double crossRate, int numWeights)
        {
            mutationRate = mutRat;
            crossOverRate = crossRate;
            popSize = _popSize;
            chromoLength = numWeights;

            population = new List<Genome>();

            for (int i = 0; i < popSize; i++)
            {
                population.Add(new Genome(new double[chromoLength]));

                for (int j = 0; j < chromoLength; j++)
                {
                    population[i].weights[j] = Random.Range(-1f, 1f);


                }
            }
        }

        /// <summary>
        /// Crosses over parent one and two at a random indixes
        /// </summary>
        /// <param name="mum"></param>
        /// <param name="dad"></param>
        /// <param name="baby1"></param>
        /// <param name="baby2"></param>
        public static void Crossover(double[] mum, double[] dad, ref double[] baby1, ref double[] baby2)
        {
            if (Random.Range(0f, 1f) > crossOverRate || mum == dad)
            {
                baby1 = mum;
                baby2 = dad;

                return;
            }

            int cp = Random.Range(0, mum.Length - 1);
            //print("crossover at " + cp);
            //Create the offspring

            for (int i = 0; i < cp; i++)
            {
                baby1[i] = mum[i];
                baby2[i] = dad[i];
            }
            for (int i = cp; i < mum.Length; i++)
            {
                baby1[i] = dad[i];
                baby2[i] = mum[i];
            }

            return;
        }

        /// <summary>
        /// Mutates points in the weight list/chromosome randomly
        /// </summary>
        /// <param name="chromo">weight list</param>
        public static void Mutate(ref double[] chromo)
        {
            for (int i = 0; i < chromo.Length; i++)
            {
                float randFloat = Random.Range(0f, 1f);
                if (randFloat < mutationRate)
                {
                    randFloat = Random.Range(-.1f, .1f);
                    //print("mutation");
                    chromo[i] += randFloat;

                    if (chromo[i] > 1)
                        chromo[i] = 1;
                    else if (chromo[i] < 0)
                        chromo[i] = 0;
                }
            }
        }

        private Genome GetChromoRoulette()
        {
            double slice = Random.Range(0f, 1f) * totalFitness;

            Genome chosen = new Genome();

            if (totalFitness <= 0) //If none of them are fit, creates a new batch of random genes
            {
                chosen = new Genome(new double[chromoLength]);

                for (int j = 0; j < chromoLength; j++)
                {
                    chosen.weights[j] = Random.Range(-1f, 1f);


                }

                System.Console.Write("No winners");
                return chosen;

            }



            double fitnessSoFar = 0;

            for (int i = 0; i < popSize; i++)
            {
                fitnessSoFar += population[i].fitness;

                if (fitnessSoFar >= slice)
                {
                    chosen = population[i];
                    break;
                }
            }

            return chosen;
        }

        public void CalculateBestWorstAvTot()
        {
            totalFitness = 0;

            double highestSoFar = 0;
            double lowestSoFar = 99999999999;

            for (int i = 0; i < popSize; i++)
            {
                //updates fittest if necessary
                if (population[i].fitness > highestSoFar)
                {
                    highestSoFar = population[i].fitness;

                    fittestGenomeIndex = i;

                    bestFitness = highestSoFar;
                }

                //update Worst
                if (population[i].fitness < lowestSoFar)
                {
                    lowestSoFar = population[i].fitness;

                    worstFitness = lowestSoFar;
                }

                totalFitness += population[i].fitness;
            }

            averageFitness = totalFitness / popSize;

        }

        private void Reset()
        {
            totalFitness = 0;
            bestFitness = 0;
            worstFitness = 9999999999;
            averageFitness = 0;
        }


        /// <summary>
        /// Creates a new generation population with the old generation population, running crossovers and mutations randomly. Make sure each genome has a 0 to positive fitness score.
        /// </summary>
        /// <param name="oldPop"></param>
        /// <returns>new population genome</returns>
        public List<Genome> Epoch(List<Genome> oldPop)
        {

            population = oldPop;


            CalculateBestWorstAvTot();

            //Create fitness bias
            population.Add(population[fittestGenomeIndex]);
            population.Add(population[fittestGenomeIndex]);
            population.Add(population[fittestGenomeIndex]);

            List<Genome> newPop = new List<Genome>();

            while (newPop.Count < popSize)
            {
                Genome mum = GetChromoRoulette();
                Genome dad = GetChromoRoulette();

                double[] baby1 = new double[chromoLength];
                double[] baby2 = new double[chromoLength];

                Crossover(mum.weights, dad.weights, ref baby1, ref baby2);

                Mutate(ref baby1);
                Mutate(ref baby2);

                newPop.Add(new Genome(baby1));
                newPop.Add(new Genome(baby2));

            }
            Reset();
            population = newPop;
            generation++;
            return population;
        }

    }
}
