/// <copyright file="NeuronLayer.cs">Copyright (c) 2016 All Rights Reserved</copyright>
/// <author>Chinedu Ozodi</author>
/// <date>12/12/2016</date>

using UnityEngine;
using System.Collections;

namespace NeuralNetwork
{
    /// <summary>
    /// A collection of neural nodes in a layer
    /// </summary>
    public struct NeuronLayer
    {
        /// <summary>
        /// number of neural nodes
        /// </summary>
        internal int numNeurons;

        /// <summary>
        /// list of neural nodes
        /// </summary>
        internal Neuron[] neurons;

        /// <summary>
        /// Creates neurons in one layer with number of specified nodes and inputs per node
        /// </summary>
        /// <param name="_numNeurons"></param>
        /// <param name="numInputsPerNeuron"></param>
        public NeuronLayer(int _numNeurons, int numInputsPerNeuron)
        {
            numNeurons = _numNeurons;

            neurons = new Neuron[numNeurons];

            for (int i = 0; i < numNeurons; i++)
            {
                neurons[i] = new Neuron(numInputsPerNeuron);
            }
        }
    }
}

