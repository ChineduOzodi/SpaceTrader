/// <copyright file="NeuralNet.cs">Copyright (c) 2016 All Rights Reserved</copyright>
/// <author>Chinedu Ozodi</author>
/// <date>12/12/2016</date>


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NeuralNetwork
{
    public class NeuralNet
    {
        internal int numInputs, numOutputs, numHiddenLayers, numNeuronHiddenLayer;

        private int numWeights;

        /// <summary>
        /// Array of layers in the network, including the hidden layers and the output layer
        /// </summary>
        public NeuronLayer[] layers;

        /// <summary>
        /// Constructor for the neural network
        /// </summary>
        /// <param name="_numInputs">The number of input node that will be used</param>
        /// <param name="_numOutputs"> The number of ouput that will be used</param>
        /// <param name="_numHiddenLayers">The number of hidden layer to use, 1 is usually enough</param>
        /// <param name="_numNeuronHiddenLayer">The number of nodes in each hidden layer</param>
        public NeuralNet(int _numInputs, int _numOutputs, int _numHiddenLayers, int _numNeuronHiddenLayer)
        {
            numInputs = _numInputs;
            numOutputs = _numOutputs;
            numHiddenLayers = _numHiddenLayers;
            numNeuronHiddenLayer = _numNeuronHiddenLayer;

            CreateNet();
            GetNumWeights();
        }

        /// <summary>
        /// Creates the network
        /// </summary>
        internal void CreateNet()
        {
            //Sum the weights and inputs
            layers = new NeuronLayer[numHiddenLayers + 1];

            if (numHiddenLayers > 0)
            {
                layers[0] = new NeuronLayer(numNeuronHiddenLayer, numInputs);

                for (int k = 1; k < numHiddenLayers; k++)
                {
                    layers[k] = new NeuronLayer(numNeuronHiddenLayer, numNeuronHiddenLayer);
                }

                layers[numHiddenLayers] = new NeuronLayer(numOutputs, numNeuronHiddenLayer);
            }
            else
            {
                layers[0] = new NeuronLayer(numOutputs, numInputs);
            }
        }
        /// <summary>
        /// Returns the weights being used in the network, to allow for modification
        /// </summary>
        /// <returns>Array of weights</returns>
        public double[] GetWeights()
        {
            double[] weights = new double[numWeights];

            int count = 0;

            for (int i = 0; i < numHiddenLayers + 1; i++)
            {
                for (int j = 0; j < layers[i].numNeurons; j++)
                {

                    //For each Weight

                    for (int k = 0; k < layers[i].neurons[j].weights.Length; k++)
                    {
                        weights[count++] = layers[i].neurons[j].weights[k];
                    }

                }

            }

            return weights;
        }
        /// <summary>
        /// Counts the number of weights that has to be created for the neural network, including the weights for the hidden layer nodes
        /// </summary>
        /// <returns>The number of weights in neural network</returns>
        public int GetNumWeights()
        {
            int count = 0;

            for (int i = 0; i < numHiddenLayers + 1; i++)
            {
                for (int j = 0; j < layers[i].numNeurons; j++)
                {

                    //For each Weight

                    for (int k = 0; k < layers[i].neurons[j].weights.Length; k++)
                    {
                        count++;
                    }

                }

            }

            numWeights = count;
            return count;
        }
        /// <summary>
        /// Sets the weights to be used in network
        /// </summary>
        /// <param name="weights"></param>
        public void PutWeights(double[] weights)
        {

            if (weights.Length != GetNumWeights())
            {
                MonoBehaviour.print("Weight length different from neural network weight length");
                return;
            }
            int count = 0;

            for (int i = 0; i < numHiddenLayers + 1; i++)
            {
                for (int j = 0; j < layers[i].numNeurons; j++)
                {

                    //For each Weight

                    for (int k = 0; k < layers[i].neurons[j].weights.Length; k++)
                    {
                        layers[i].neurons[j].weights[k] = weights[count++];
                    }

                }

            }

            return;
        }
        /// <summary>
        /// Runs through the network to generate an output from your input
        /// </summary>
        /// <param name="inputs">List of inputs for the network, should match the number of inputs created for the network</param>
        /// <returns>List of ouputs matching the number of ouputs created for the network</returns>
        public List<double> Update(List<double> inputs)
        {
            List<double> outputs = new List<double>();

            int weight = 0;

            if (inputs.Count != numInputs)
            {
                return outputs;
            }

            for (int i = 0; i < numHiddenLayers + 1; i++)
            {
                if (i > 0)
                {
                    inputs = outputs;
                }

                outputs = new List<double>();
                weight = 0;

                for (int j = 0; j < layers[i].numNeurons; j++)
                {
                    double netInput = 0;

                    int nNumInputs = layers[i].neurons[j].numInputs;

                    //For each Weight

                    for (int k = 0; k < nNumInputs; k++)
                    {
                        //Sum the weights and inputs

                        netInput += layers[i].neurons[j].weights[k] * inputs[weight++];
                    }

                    //Add bias

                    netInput += layers[i].neurons[j].weights[nNumInputs] * -1;

                    outputs.Add(Sigmoid(netInput, 10));

                    weight = 0;
                }

            }
            return outputs;
        }

        /// <summary>
        /// Sigmoid function to be used in network calculations
        /// </summary>
        /// <param name="activation">The activation number from weight and nodes</param>
        /// <param name="response">How responsive the sigmoid should be to the numbers, 1 is usually okay</param>
        /// <returns>Output signal</returns>
        internal static double Sigmoid(double activation, double response)
        {
            return 1.0 / (1 + Mathf.Pow(Mathf.Epsilon, (float)(-activation / response)));
        }
    }
}
