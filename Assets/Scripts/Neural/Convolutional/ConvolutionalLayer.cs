using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class ConvolutionalLayer
{
	[JsonProperty("neurons")] public ConvolutionalNeuron Mask;
	public Vector2Int PixelCount;

	public float[,] PrevOutput;

	public ConvolutionalNeuralNetworkSettings Settings;

	private ConvolutionalLayer()
	{
	}

	public ConvolutionalLayer(ConvolutionalNeuralNetworkSettings settings, Vector2Int pixelCount)
	{
		Settings = settings;
		PixelCount = pixelCount;
		Mask = new ConvolutionalNeuron(settings.FilterSize.x, settings.FilterSize.y);
	}

	public static ConvolutionalLayer First(ConvolutionalNeuralNetworkSettings settings, Vector2Int pixelCount)
	{
		ConvolutionalLayer layer = new ConvolutionalLayer();
		layer.Settings = settings;
		layer.PixelCount = pixelCount;
		layer.Mask = new ConvolutionalNeuron();

		return layer;
	}

	public ConvolutionalLayer(ConvolutionalLayer layer)
	{
		Mask = new ConvolutionalNeuron(layer.Mask);
		PixelCount = layer.PixelCount;
		Settings = layer.Settings;
	}

	public float[,] Calculate(float[,] input)
	{
		float[,] output = new float[PixelCount.x, PixelCount.y];

		for (int i = 0; i < PixelCount.x; i++)
		{
			for (int j = 0; j < PixelCount.y; j++)
			{
				float result;

				if (Mask.Weights == null)
					result = input[i, j];
				else
					result = Mask.Calculate(input, i * (Settings.FilterSize.x - Settings.Overlap), j * (Settings.FilterSize.y - Settings.Overlap));

				output[i, j] = result;
			}
		}

		PrevOutput = output;

		return output;
	}

	public float IntroduceRandomError()
	{
		float randomError = UnityEngine.Random.Range(Settings.MinRandomErrorCoefficient, Settings.MaxRandomErrorCoefficient);

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