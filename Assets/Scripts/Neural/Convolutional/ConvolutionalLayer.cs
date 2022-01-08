using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class ConvolutionalLayer
{
	[JsonProperty("neurons")] public ConvolutionalNeuron Mask;
	public Vector2Int Size;

	public float[,] PrevOutput;

	private ConvolutionalNeuralNetworkSettings _settings;

	private ConvolutionalLayer()
	{
	}

	public ConvolutionalLayer(ConvolutionalNeuralNetworkSettings settings, Vector2Int size)
	{
		_settings = settings;
		Size = size;
		Mask = new ConvolutionalNeuron(settings.FilterSize.x, settings.FilterSize.y);
	}

	public static ConvolutionalLayer First(ConvolutionalNeuralNetworkSettings settings, Vector2Int size) //first layer
	{
		ConvolutionalLayer layer = new ConvolutionalLayer();
		layer._settings = settings;
		layer.Size = size;
		layer.Mask = new ConvolutionalNeuron();

		return layer;
	}

	public ConvolutionalLayer(ConvolutionalLayer layer)
	{
		Mask = new ConvolutionalNeuron(layer.Mask);
		Size = layer.Size;
		_settings = layer._settings;
	}

	public float[,] Calculate(float[,] input)
	{
		float[,] output = new float[Size.x, Size.y];

		for (int i = 0; i < Size.x; i++)
		{
			for (int j = 0; j < Size.y; j++)
			{
				float result;

				if (Mask.Weights == null)
					result = input[i, j];
				else
					result = Mask.Calculate(input, i * (_settings.FilterSize.x - _settings.Overlap), j * (_settings.FilterSize.y - _settings.Overlap));

				output[i, j] = result;
			}
		}

		PrevOutput = output;

		return output;
	}

	public float IntroduceRandomError()
	{
		float randomError = UnityEngine.Random.Range(_settings.MinRandomErrorCoefficient, _settings.MaxRandomErrorCoefficient);

		bool isErrorNegative = UnityEngine.Random.value <= 0.5f;
		if (isErrorNegative)
			randomError *= -1;

		Mask.IntroduceError(randomError);

		return randomError;
	}

	public void SetInitialWeights()
	{
		Mask.SetInitialWeights();
	}

	public void RandomizeAllWeights()
	{
		Mask.SetRandomWeights();
	}
}