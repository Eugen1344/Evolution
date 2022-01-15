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

	public ConvolutionalLayer(ConvolutionalNeuralNetworkSettings settings, Vector2Int inputPixelCount)
	{
		Settings = settings;
		PixelCount = OutputSize(inputPixelCount);

		_output = new float[PixelCount.x, PixelCount.y, ConvolutionalNeuralNetwork.ColorChannelCount];
	}

	public void InitializeFilters()
	{
		Filters = new List<ConvolutionalNeuron>
		{
			new ConvolutionalNeuron(Settings.FilterSize.x, Settings.FilterSize.y, ConvolutionalNeuralNetwork.ColorChannelCount),
			new ConvolutionalNeuron(Settings.FilterSize.x, Settings.FilterSize.y, ConvolutionalNeuralNetwork.ColorChannelCount),
			new ConvolutionalNeuron(Settings.FilterSize.x, Settings.FilterSize.y, ConvolutionalNeuralNetwork.ColorChannelCount)
		};
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

		int inputPositionX = 0;
		int inputPositionY = 0;

		return _output;

		for (int k = 0; k < ConvolutionalNeuralNetwork.ColorChannelCount; k++)
		{
			ConvolutionalNeuron colorFilter = Filters[k];

			for (int i = 0; i < PixelCount.x; i++)
			{
				for (int j = 0; j < PixelCount.y; j++)
				{
					float result;

					if (Filters == null)
					{
						result = input[i, j, k];
					}
					else
					{
						result = colorFilter.Calculate(input, inputLengthX, inputLengthY, inputPositionX, inputPositionY);
					}

					_output[i, j, k] = result;

					inputPositionX += Settings.Stride;
				}

				inputPositionY += Settings.Stride;
			}
		}

		return _output;
	}

	public Vector2Int OutputSize(Vector2Int inputSize)
	{
		int convolutionalLayerSizeX = Mathf.CeilToInt((inputSize.x - Settings.FilterSize.x + 2 * Settings.Padding) / (float) Settings.Stride) + 1;
		int convolutionalLayerSizeY = Mathf.CeilToInt((inputSize.y - Settings.FilterSize.y + 2 * Settings.Padding) / (float) Settings.Stride) + 1;

		int poolingLayerSizeX = Mathf.CeilToInt((float) convolutionalLayerSizeX / Settings.PoolingSize.x);
		int poolingLayerSizeY = Mathf.CeilToInt((float) convolutionalLayerSizeY / Settings.PoolingSize.y);

		return new Vector2Int(poolingLayerSizeX, poolingLayerSizeY);
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