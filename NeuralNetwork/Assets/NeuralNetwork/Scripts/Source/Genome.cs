/// <copyright file="Genome.cs">Copyright (c) 2016 All Rights Reserved</copyright>
/// <author>Chinedu Ozodi</author>
/// <date>12/12/2016</date>

using UnityEngine;
using System.Collections;

namespace NeuralNetwork
{
    /// <summary>
    /// Houses the weight information for the neural network
    /// </summary>
    public struct Genome
    {
        /// <summary>
        /// weight information for the neural network
        /// </summary>
        internal double[] weights;

        /// <summary>
        /// fitness level of the current weight information
        /// </summary>
        public double fitness;

        /// <summary>
        /// Weght struct constructor, with default fitness of zero
        /// </summary>
        /// <param name="_weights"> double array of the weights to be used</param>
        /// <param name="_fitness"> fitness of the set of weights</param>
        public Genome(double[] _weights, double _fitness = 0)
        {
            fitness = _fitness;
            weights = _weights;
        }

        public static bool operator <(Genome g1, Genome g2)
        {
            return (g1.fitness < g2.fitness);
        }
        public static bool operator >(Genome g1, Genome g2)
        {
            return (g1.fitness > g2.fitness);
        }

    }
}

