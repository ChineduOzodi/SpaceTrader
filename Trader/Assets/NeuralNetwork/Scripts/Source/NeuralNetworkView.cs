/// <copyright file="NeuralNetworkView.cs">Copyright (c) 2016 All Rights Reserved</copyright>
/// <author>Chinedu Ozodi</author>
/// <date>12/12/2016</date>

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace NeuralNetwork
{
    public class NeuralNetworkView : MonoBehaviour
    {
        public GameObject lineInstance;
        public GameObject neuralNode;

        public List<double> selectedNeuralInputs;

        private int numInputs, numOutputs, numHiddenLayers, numNodeHiddenLayers;

        private GameObject parent;

        private SpriteRenderer[] inputObj;
        private SpriteRenderer[][] layersObj;
        private LineRenderer[][][] weightObj;

        /// <summary>
        /// Creates a representation of the nodal network given the appropriate information
        /// </summary>
        /// <param name="_numInputs"></param>
        /// <param name="_numOutputs"></param>
        /// <param name="_numHiddenLayers"></param>
        /// <param name="_numNodeHiddenLayers"></param>
        public void CreateNetwork(int _numInputs, int _numOutputs, int _numHiddenLayers, int _numNodeHiddenLayers)
        {
            numInputs = _numInputs;
            numOutputs = _numOutputs;
            numHiddenLayers = _numHiddenLayers;
            numNodeHiddenLayers = _numNodeHiddenLayers;

            //Destroy previous newtwork if any
            Destroy(parent);

            parent = new GameObject("NueralNetworkView");

            //Setup Nodes
            inputObj = new SpriteRenderer[numInputs];
            layersObj = new SpriteRenderer[numHiddenLayers + 1][];
            weightObj = new LineRenderer[numHiddenLayers + 1][][];


            weightObj[0] = new LineRenderer[numInputs][];

            int j = 5;

            for (int i = 0; i < numInputs; i++)
            {
                inputObj[i] = (Instantiate(neuralNode, parent.transform) as GameObject).GetComponent<SpriteRenderer>();

                inputObj[i].transform.position = new Vector3(0, i);
            }


            for (int i = 0; i < numHiddenLayers; i++)
            {
                //Set Hidden Layers
                layersObj[i] = new SpriteRenderer[numNodeHiddenLayers];
                weightObj[i] = new LineRenderer[numNodeHiddenLayers][];

                for (int k = 0; k < numNodeHiddenLayers; k++)
                {
                    layersObj[i][k] = (Instantiate(neuralNode, parent.transform) as GameObject).GetComponent<SpriteRenderer>();

                    layersObj[i][k].transform.position = new Vector3(j, k);

                    //Set Weights

                    if (0 == i)
                    {
                        //Set first hidden Node
                        weightObj[i][k] = new LineRenderer[numInputs];

                        for (int m = 0; m < numInputs; m++)
                        {
                            weightObj[i][k][m] = (Instantiate(lineInstance, parent.transform) as GameObject).GetComponent<LineRenderer>();

                            LineRenderer line = weightObj[i][k][m].GetComponent<LineRenderer>();
                            Vector3[] connection = new Vector3[2] { new Vector3(j, k), new Vector3(j - 5, m) };

                            line.SetPositions(connection);
                        }

                    }
                    else
                    {
                        weightObj[i][k] = new LineRenderer[numNodeHiddenLayers];

                        for (int m = 0; m < numNodeHiddenLayers; m++)
                        {
                            weightObj[i][k][m] = (Instantiate(lineInstance, parent.transform) as GameObject).GetComponent<LineRenderer>();

                            LineRenderer line = weightObj[i][k][m].GetComponent<LineRenderer>();
                            Vector3[] connection = new Vector3[2] { new Vector3(j, k), new Vector3(j - 5, m) };

                            line.SetPositions(connection);
                        }

                    }
                }

                j += 5;
            }

            //Set output layer

            layersObj[numHiddenLayers] = new SpriteRenderer[numOutputs];
            weightObj[numHiddenLayers] = new LineRenderer[numOutputs][];

            for (int k = 0; k < numOutputs; k++)
            {
                layersObj[numHiddenLayers][k] = (Instantiate(neuralNode, parent.transform) as GameObject).GetComponent<SpriteRenderer>();

                layersObj[numHiddenLayers][k].transform.position = new Vector3(j, k);

                // SetWeights
                weightObj[numHiddenLayers][k] = new LineRenderer[numNodeHiddenLayers];

                for (int m = 0; m < numNodeHiddenLayers; m++)
                {
                    weightObj[numHiddenLayers][k][m] = (Instantiate(lineInstance, parent.transform) as GameObject).GetComponent<LineRenderer>();

                    LineRenderer line = weightObj[numHiddenLayers][k][m].GetComponent<LineRenderer>();
                    Vector3[] connection = new Vector3[2] { new Vector3(j, k), new Vector3(j - 5, m) };

                    line.SetPositions(connection);
                }
            }
        }

        /// <summary>
        /// Creates the visual representation of what an input is doing to the neural network
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<double> NodeUpdate(List<double> input, NeuralNet nNetwork)
        {
            List<double> outputs = new List<double>();

            int weight = 0;

            if (input.Count != numInputs)
            {
                return outputs;
            }

            //Set Input Nodes
            for (int i = 0; i < input.Count; i++)
            {
                if (input[i] > 0)
                {
                    inputObj[i].color = Color.green * (float)(input[i]);
                    inputObj[i].transform.localScale = Vector3.one * ((float)input[i]);
                }
                else
                {
                    inputObj[i].color = Color.red * (float)(input[i] * -1);
                    inputObj[i].transform.localScale = Vector3.one * ((float)input[i] * -1);
                }
            }

            for (int i = 0; i < numHiddenLayers + 1; i++)
            {
                if (i > 0)
                {
                    input = outputs;
                }

                outputs = new List<double>();
                weight = 0;

                for (int j = 0; j < nNetwork.layers[i].numNeurons; j++)
                {
                    double netInput = 0;

                    int nNumInputs = nNetwork.layers[i].neurons[j].numInputs;

                    //For each Weight

                    for (int k = 0; k < nNumInputs; k++)
                    {
                        //Sum the weights and inputs

                        netInput += nNetwork.layers[i].neurons[j].weights[k] * input[weight];

                        //Set Lines
                        float line = (float)(nNetwork.layers[i].neurons[j].weights[k] * input[weight++]);

                        if (line > 0)
                        {
                            weightObj[i][j][k].SetColors(Color.green * line, Color.green * line);
                            weightObj[i][j][k].SetWidth(line * .25f, line * .25f);
                        }
                        else
                        {
                            weightObj[i][j][k].SetColors(Color.red * -line, Color.red * -line);
                            weightObj[i][j][k].SetWidth(-line * .25f, -line * .25f);
                        }

                    }

                    //Add bias

                    netInput += nNetwork.layers[i].neurons[j].weights[nNumInputs] * -1;

                    double sig = NeuralNet.Sigmoid(netInput, 10);
                    //Set Node Color
                    layersObj[i][j].color = (Color.green * (float)sig);
                    layersObj[i][j].transform.localScale = (Vector3.one * (float)sig);

                    outputs.Add(sig);

                    weight = 0;
                }

            }
            return outputs;
        }


    }



}
