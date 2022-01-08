using System;
using Newtonsoft.Json;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class ConvolutionalLayer
{
	[JsonProperty("neurons")] public ConvolutionalNeuron[,] Mask;

	public int NeuronsSizeX => Mask.GetLength(0);
	public int NeuronsSizeY => Mask.GetLength(1);

	public float[,] PrevOutput;

	private ConvolutionalNeuralNetworkSettings _settings;

	private ConvolutionalLayer()
	{
	}

	public ConvolutionalLayer(int neuronsCountX, int neuronsCountY, ConvolutionalNeuralNetworkSettings settings)
	{
		Mask = new ConvolutionalNeuron[neuronsCountX, neuronsCountY];
		_settings = settings;

		for (int i = 0; i < neuronsCountX; i++)
		{
			for (int j = 0; j < neuronsCountY; j++)
			{
				Mask[i, j] = new ConvolutionalNeuron(settings.FilterSize.x, settings.FilterSize.y);
			}
		}
	}

	public static ConvolutionalLayer First(int neuronsCountX, int neuronsCountY, ConvolutionalNeuralNetworkSettings settings) //first layer
	{
		ConvolutionalLayer layer = new ConvolutionalLayer();
		layer._settings = settings;
		layer.Mask = new ConvolutionalNeuron[neuronsCountX, neuronsCountY];

		for (int i = 0; i < neuronsCountX; i++)
		{
			for (int j = 0; j < neuronsCountY; j++)
			{
				layer.Mask[i, j] = new ConvolutionalNeuron();
			}
		}

		return layer;
	}

	public ConvolutionalLayer(ConvolutionalLayer layer)
	{
		Mask = new ConvolutionalNeuron[layer.NeuronsSizeX, layer.NeuronsSizeY];
		_settings = layer._settings;

		for (int i = 0; i < NeuronsSizeX; i++)
		{
			for (int j = 0; j < NeuronsSizeY; j++)
			{
				Mask[i, j] = new ConvolutionalNeuron(layer.Mask[i, j]);
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
				ConvolutionalNeuron mask = Mask[i, j];
				float result;

				if (mask.Weights == null)
					result = input[i, j];
				else
					result = mask.Calculate(input, i * (_settings.FilterSize.x - _settings.Overlap), j * (_settings.FilterSize.y - _settings.Overlap));

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
				Mask[i, j].SetInitialWeights();
			}
		}
	}

	public void RandomizeAllWeights()
	{
		for (int i = 0; i < NeuronsSizeX; i++)
		{
			for (int j = 0; j < NeuronsSizeY; j++)
			{
				Mask[i, j].SetRandomWeights();
			}
		}
	}
}