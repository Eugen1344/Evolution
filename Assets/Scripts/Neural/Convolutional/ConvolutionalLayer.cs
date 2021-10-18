using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class ConvolutionalLayer
{
	[JsonProperty("neurons")]
	public ConvolutionalNeuron[,] Neurons;

	public int NeuronsSizeX => Neurons.GetLength(0);
	public int NeuronsSizeY => Neurons.GetLength(1);

	public readonly int FilterSizeX;
	public readonly int FilterSizeY;

	public ConvolutionalLayer()
	{
		Neurons = null;
	}

	public ConvolutionalLayer(int neuronsCountX, int neuronsCountY, int filterSizeX, int filterSizeY)
	{
		Neurons = new ConvolutionalNeuron[neuronsCountX, neuronsCountY];
		FilterSizeX = filterSizeX;
		FilterSizeY = filterSizeY;

		for (int i = 0; i < neuronsCountX; i++)
		{
			for (int j = 0; j < neuronsCountY; j++)
			{
				Neurons[i, j] = new ConvolutionalNeuron(filterSizeX, filterSizeY);
			}
		}
	}

	public ConvolutionalLayer(ConvolutionalLayer layer)
	{
		Neurons = new ConvolutionalNeuron[layer.NeuronsSizeX, layer.NeuronsSizeY];

		for (int i = 0; i < NeuronsSizeX; i++)
		{
			for (int j = 0; j < NeuronsSizeY; j++)
			{
				Neurons[i, j] = new ConvolutionalNeuron(layer.Neurons[i, j]);
			}
		}
	}

	public float[,] Calculate(float[,] input)
	{
		int newSizeX = Mathf.CeilToInt((float)NeuronsSizeX / FilterSizeX);
		int newSizeY = Mathf.CeilToInt((float)NeuronsSizeY / FilterSizeY);

		float[,] output = new float[newSizeX, newSizeY];

		for (int i = 0; i < newSizeX; i++)
		{
			for (int j = 0; j < newSizeY; j++)
			{
				output[i, j] = Neurons[i, j].Calculate(input, i * FilterSizeX, j * FilterSizeY);
			}
		}

		return output;
	}

	public void SetInitialWeights()
	{
		for (int i = 0; i < NeuronsSizeX; i++)
		{
			for (int j = 0; j < NeuronsSizeY; j++)
			{
				Neurons[i, j].SetInitialWeights();
			}
		}
	}

	public void RandomizeAllWeights()
	{
		for (int i = 0; i < NeuronsSizeX; i++)
		{
			for (int j = 0; j < NeuronsSizeY; j++)
			{
				Neurons[i, j].SetRandomWeights();
			}
		}
	}
}