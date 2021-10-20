using System;
using Newtonsoft.Json;

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

	public float[,] PrevOutput;

	public ConvolutionalLayer()
	{
	}

	public ConvolutionalLayer(int neuronsCountX, int neuronsCountY) //first layer
	{
		Neurons = new ConvolutionalNeuron[neuronsCountX, neuronsCountY];

		for (int i = 0; i < neuronsCountX; i++)
		{
			for (int j = 0; j < neuronsCountY; j++)
			{
				Neurons[i, j] = new ConvolutionalNeuron();
			}
		}
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
		FilterSizeX = layer.FilterSizeX;
		FilterSizeY = layer.FilterSizeY;

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
		float[,] output = new float[NeuronsSizeX, NeuronsSizeY];

		for (int i = 0; i < NeuronsSizeX; i++)
		{
			for (int j = 0; j < NeuronsSizeY; j++)
			{
				ConvolutionalNeuron neuron = Neurons[i, j];
				float result;

				if (neuron.Weights == null)
					result = input[i, j];
				else
					result = neuron.Calculate(input, i * FilterSizeX, j * FilterSizeY);

				output[i, j] = result;
			}
		}

		PrevOutput = output;

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