using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class ConvolutionalNeuralNetwork
{
	[JsonProperty("layers")]
	public List<ConvolutionalLayer> NeuronLayers;
	[JsonProperty("settings")]
	public ConvolutionalNeuralNetworkSettings Settings;

	public ConvolutionalNeuralNetwork()
	{

	}

	public ConvolutionalNeuralNetwork(ConvolutionalNeuralNetworkSettings settings)
	{
		Settings = settings;

		NeuronLayers = new List<ConvolutionalLayer>();

		InitializeNeurons(settings);
	}

	public ConvolutionalNeuralNetwork(ConvolutionalNeuralNetwork network)
	{
		Settings = network.Settings;

		NeuronLayers = new List<ConvolutionalLayer>();

		InitializeNeurons(network);
	}

	public float[,] Calculate(float[,] input)
	{
		float[,] output = input;

		for (int i = 1; i < NeuronLayers.Count; i++)
		{
			output = NeuronLayers[i].Calculate(output);
		}

		return output;
	}

	public static ConvolutionalNeuralNetwork Initial(ConvolutionalNeuralNetworkSettings settings)
	{
		ConvolutionalNeuralNetwork network = new ConvolutionalNeuralNetwork(settings);
		network.SetInitialWeights();

		return network;
	}

	public static ConvolutionalNeuralNetwork Random(ConvolutionalNeuralNetworkSettings settings)
	{
		ConvolutionalNeuralNetwork network = new ConvolutionalNeuralNetwork(settings);
		network.RandomizeAllWeights();

		return network;
	}

	private void InitializeNeurons(ConvolutionalNeuralNetwork network)
	{
		foreach (ConvolutionalLayer layer in network.NeuronLayers)
		{
			ConvolutionalLayer copiedLayer = new ConvolutionalLayer(layer);

			NeuronLayers.Add(copiedLayer);
		}
	}

	private void InitializeNeurons(ConvolutionalNeuralNetworkSettings settings)
	{
		foreach (Vector2Int count in settings.NeuronsCount)
		{
			ConvolutionalLayer layer = new ConvolutionalLayer(count.x, count.y, settings.FilterSize.x, settings.FilterSize.y);

			NeuronLayers.Add(layer);
		}
	}

	public float IntroduceRandomError()
	{
		if (NeuronLayers.Count <= 1)
			return 0;

		int randomLayerIndex = UnityEngine.Random.Range(1, NeuronLayers.Count);
		ConvolutionalLayer randomLayer = NeuronLayers[randomLayerIndex];

		int randomNeuronIndexX = UnityEngine.Random.Range(0, randomLayer.NeuronsSizeX);
		int randomNeuronIndexY = UnityEngine.Random.Range(0, randomLayer.NeuronsSizeY);
		ConvolutionalNeuron randomNeuron = randomLayer.Neurons[randomNeuronIndexX, randomNeuronIndexY];

		float randomError = UnityEngine.Random.Range(Settings.MinRandomErrorCoefficient, Settings.MaxRandomErrorCoefficient);

		bool isErrorNegative = UnityEngine.Random.value < 0.5f;
		if (isErrorNegative)
			randomError *= -1;

		randomNeuron.IntroduceError(randomError);

		return randomError;
	}

	private void SetInitialWeights()
	{
		foreach (ConvolutionalLayer layer in NeuronLayers)
		{
			layer.SetInitialWeights();
		}
	}

	private void RandomizeAllWeights()
	{
		foreach (ConvolutionalLayer layer in NeuronLayers)
		{
			layer.RandomizeAllWeights();
		}
	}
}