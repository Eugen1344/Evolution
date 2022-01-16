using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class ConvolutionalLayer : AbstractConvolutionalLayer
{
	[JsonProperty("filters")] public List<ConvolutionalNeuron> Filters;

	protected ConvolutionalLayer()
	{
		
	}
	
	public ConvolutionalLayer(ConvolutionalNeuralNetworkSettings settings, Vector2Int inputPixelCount) : base(settings, inputPixelCount)
	{
		Filters = new List<ConvolutionalNeuron>
		{
			new ConvolutionalNeuron(Settings.FilterSize.x, Settings.FilterSize.y, ConvolutionalNeuralNetwork.ColorChannelCount),
			new ConvolutionalNeuron(Settings.FilterSize.x, Settings.FilterSize.y, ConvolutionalNeuralNetwork.ColorChannelCount),
			new ConvolutionalNeuron(Settings.FilterSize.x, Settings.FilterSize.y, ConvolutionalNeuralNetwork.ColorChannelCount)
		};
	}

	public ConvolutionalLayer(ConvolutionalLayer layer, ConvolutionalNeuralNetworkSettings settings, Vector2Int inputSize) : base(settings, inputSize)
	{
		Filters = new List<ConvolutionalNeuron>();

		foreach (ConvolutionalNeuron neuron in layer.Filters)
		{
			Filters.Add(new ConvolutionalNeuron(neuron));
		}
	}

	public override float[,,] Calculate(float[,,] input)
	{
		for (int k = 0; k < ConvolutionalNeuralNetwork.ColorChannelCount; k++)
		{
			int inputPositionX = 0;
			ConvolutionalNeuron colorFilter = Filters[k];

			for (int i = 0; i < OutputPixelCount.x; i++)
			{
				int inputPositionY = 0;

				for (int j = 0; j < OutputPixelCount.y; j++)
				{
					float result = colorFilter.Calculate(input, _inputPixelCount.x, _inputPixelCount.y, inputPositionX, inputPositionY);

					_output[i, j, k] = result;

					inputPositionY += Settings.Stride;
				}

				inputPositionX += Settings.Stride;
			}
		}

		return _output;
	}

	public override Vector2Int OutputSize(Vector2Int inputSize)
	{
		int convolutionalLayerSizeX = Mathf.CeilToInt((inputSize.x - Settings.FilterSize.x + 2 * Settings.Padding) / (float) Settings.Stride) + 1;
		int convolutionalLayerSizeY = Mathf.CeilToInt((inputSize.y - Settings.FilterSize.y + 2 * Settings.Padding) / (float) Settings.Stride) + 1;

		return new Vector2Int(convolutionalLayerSizeX, convolutionalLayerSizeY);
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