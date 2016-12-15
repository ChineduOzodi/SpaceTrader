/// <copyright file="Neuron.cs">Copyright (c) 2016 All Rights Reserved</copyright>
/// <author>Chinedu Ozodi</author>
/// <date>12/12/2016</date>

using UnityEngine;
using System.Collections;

namespace NeuralNetwork
{
    /// <summary>
    /// Basic Node class in network
    /// </summary>
    public struct Neuron
    {
        /// <summary>
        /// number of inputs into the neuron
        /// </summary>
        public int numInputs;

        /// <summary>
        /// weights for the neurons inputs, including the weight for the neuron itself
        /// </summary>
        public double[] weights;

        /// <summary>
        /// Creates neuron with number of inputs
        /// </summary>
        /// <param name="_numInputs">number of inputs</param>
        public Neuron(int _numInputs)
        {
            numInputs = _numInputs;

            weights = new double[numInputs + 1];

            for (int i = 0; i < numInputs + 1; i++)
            {
                weights[i] = Random.Range(-1.0f, 1.0f);
            }
        }
    }

}
