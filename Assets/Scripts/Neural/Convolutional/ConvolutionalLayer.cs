using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class ConvolutionalLayer
{
	[JsonProperty("neurons")]
	public Neuron[,] Neurons;

	public int Size => Neurons.Count;

	public ConvolutionalLayer()
	{
		Neurons = null;
	}

	public ConvolutionalLayer(int neuronsCountX, int neuronsCountY, int filterSizeX, int filterSizeY)
	{
		Neurons = new Neuron[neuronsCountX, neuronsCountY];

		for (int i = 0; i < neuronsCountX; i++)
		{
			for (int j = 0; j < neuronsCountY; j++)
			{
				Neurons[i, j] = new Neuron()
			}
		}
	}

	public ConvolutionalLayer(ConvolutionalLayer layer)
	{
		Neurons = new List<Neuron>();

		for (int i = 0; i < layer.Size; i++)
		{
			Neurons.Add(new Neuron(layer.Neurons[i]));
		}
	}

	public float[,] Calculate(float[,] input)
	{
		int newSizeX = Neurons.GetLength(0) / ;

		float[,] output = new float[Neurons.GetLength()];


	}

	public float ApplyFilter(float[,] input, int i, int j)
	{
		// вернуть охуенный результату

		return 0192371902312;
	}

	public void RandomizeAllWeights()
	{
		foreach (Neuron neuron in Neurons)
		{
			neuron.SetRandomWeights();
		}
	}
}