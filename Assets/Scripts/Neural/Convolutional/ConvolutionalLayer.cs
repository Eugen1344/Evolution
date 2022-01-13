using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class ConvolutionalLayer
{
	[JsonProperty("filters")] public List<ConvolutionalNeuron> Filters;
	public Vector2Int PixelCount;

	public float[,,] PrevOutput => _output;

	public ConvolutionalNeuralNetworkSettings Settings;

	private float[,,] _output;

	private ConvolutionalLayer()
	{
	}

	public ConvolutionalLayer(ConvolutionalNeuralNetworkSettings settings, Vector2Int pixelCount)
	{
		Settings = settings;
		PixelCount = pixelCount;

		Filters = new List<ConvolutionalNeuron>
		{
			new ConvolutionalNeuron(settings.FilterSize.x, settings.FilterSize.y, ConvolutionalNeuralNetwork.ColorChannelCount),
			new ConvolutionalNeuron(settings.FilterSize.x, settings.FilterSize.y, ConvolutionalNeuralNetwork.ColorChannelCount),
			new ConvolutionalNeuron(settings.FilterSize.x, settings.FilterSize.y, ConvolutionalNeuralNetwork.ColorChannelCount)
		};

		_output = new float[PixelCount.x, PixelCount.y, ConvolutionalNeuralNetwork.ColorChannelCount];
	}

	public static ConvolutionalLayer First(ConvolutionalNeuralNetworkSettings settings, Vector2Int pixelCount)
	{
		ConvolutionalLayer layer = new ConvolutionalLayer();
		layer.Settings = settings;
		layer.PixelCount = pixelCount;
		layer.Filters = null;
		layer._output = new float[pixelCount.x, pixelCount.y, ConvolutionalNeuralNetwork.ColorChannelCount];

		return layer;
	}

	public ConvolutionalLayer(ConvolutionalLayer layer)
	{
		if (layer.Filters == null)
		{
			Filters = null;
		}
		else
		{
			Filters = new List<ConvolutionalNeuron>();

			foreach (ConvolutionalNeuron neuron in layer.Filters)
			{
				Filters.Add(new ConvolutionalNeuron(neuron));
			}
		}

		PixelCount = layer.PixelCount;
		Settings = layer.Settings;
		_output = new float[PixelCount.x, PixelCount.y, ConvolutionalNeuralNetwork.ColorChannelCount];
	}

	public float[,,] Calculate(float[,,] input)
	{
		int inputLengthX = input.GetLength(0);
		int inputLengthY = input.GetLength(1);
		
		for (int i = 0; i < PixelCount.x; i++)
		{
			for (int j = 0; j < PixelCount.y; j++)
			{
				for (int k = 0; k < ConvolutionalNeuralNetwork.ColorChannelCount; k++)
				{
					float result;

					if (Filters == null)
					{
						result = input[i, j, k];
					}
					else
					{
						ConvolutionalNeuron colorFilter = Filters[k];
						result = colorFilter.Calculate(input, inputLengthX, inputLengthY, i * (Settings.FilterSize.x - Settings.Overlap), j * (Settings.FilterSize.y - Settings.Overlap));
					}

					_output[i, j, k] = result;
				}
			}
		}

		return _output;
	}

	public float IntroduceRandomError()
	{
		if (Filters == null)
			return 0;

		float randomError = Random.Range(Settings.MinRandomErrorCoefficient, Settings.MaxRandomErrorCoefficient);

		bool isErrorNegative = Random.value <= 0.5f;
		if (isErrorNegative)
			randomError *= -1;

		int randomFilterIndex = Random.Range(0, Filters.Count);

		ConvolutionalNeuron randomFilter = Filters[randomFilterIndex];
		randomFilter.IntroduceError(randomError);

		return randomError;
	}

	public void SetInitialWeights()
	{
		if (Filters == null)
			return;

		foreach (ConvolutionalNeuron neuron in Filters)
		{
			neuron.SetInitialWeights();
		}
	}

	public void RandomizeAllWeights()
	{
		if (Filters == null)
			return;

		foreach (ConvolutionalNeuron neuron in Filters)
		{
			neuron.SetRandomWeights();
		}
	}
}