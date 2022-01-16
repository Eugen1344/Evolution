using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class ConvolutionalNeuralNetwork
{
	public const int ColorChannelCount = 3;

	public AbstractConvolutionalLayer FirstLayer;
	[JsonProperty("layers")] public List<ConvolutionalLayer> ConvolutionalLayers;
	public List<PoolingLayer> PoolingLayers;
	[JsonProperty("settings")] public ConvolutionalNeuralNetworkSettings Settings;

	public ConvolutionalNeuralNetwork()
	{
	}

	public ConvolutionalNeuralNetwork(ConvolutionalNeuralNetworkSettings settings)
	{
		Settings = settings;

		ConvolutionalLayers = new List<ConvolutionalLayer>();
		PoolingLayers = new List<PoolingLayer>();

		InitializeNeurons(settings);
	}

	public ConvolutionalNeuralNetwork(ConvolutionalNeuralNetwork network)
	{
		Settings = network.Settings;

		ConvolutionalLayers = new List<ConvolutionalLayer>();
		PoolingLayers = new List<PoolingLayer>();

		InitializeNeurons(network);
	}

	private void InitializeNeurons(ConvolutionalNeuralNetworkSettings settings)
	{
		FirstLayer = new AbstractConvolutionalLayer(settings, Settings.InputPixelCount);

		Vector2Int prevPixelCount = settings.InputPixelCount;

		for (int i = 0; i < settings.LayerCount; i++)
		{
			ConvolutionalLayer layer = new ConvolutionalLayer(settings, prevPixelCount);
			ConvolutionalLayers.Add(layer);

			PoolingLayer poolingLayer = new PoolingLayer(settings, layer.OutputPixelCount);
			PoolingLayers.Add(poolingLayer);

			prevPixelCount = poolingLayer.OutputPixelCount;
		}
	}

	private void InitializeNeurons(ConvolutionalNeuralNetwork network)
	{
		FirstLayer = new AbstractConvolutionalLayer(network.Settings, Settings.InputPixelCount);

		Vector2Int prevPixelCount = Settings.InputPixelCount;

		for (int i = 0; i < Settings.LayerCount; i++)
		{
			ConvolutionalLayer layer = network.ConvolutionalLayers[i];

			ConvolutionalLayer copiedLayer = new ConvolutionalLayer(layer, Settings, prevPixelCount);
			ConvolutionalLayers.Add(copiedLayer);

			PoolingLayer poolingLayer = new PoolingLayer(network.Settings, copiedLayer.OutputPixelCount);
			PoolingLayers.Add(poolingLayer);

			prevPixelCount = poolingLayer.OutputPixelCount;
		}
	}

	public float[,,] Calculate(float[,,] input)
	{
		float[,,] output = FirstLayer.Calculate(input);

		for (int i = 0; i < ConvolutionalLayers.Count; i++)
		{
			output = ConvolutionalLayers[i].Calculate(output);
			output = PoolingLayers[i].Calculate(output);
		}

		return output;
	}

	public float IntroduceRandomError()
	{
		int randomLayerIndex = UnityEngine.Random.Range(0, ConvolutionalLayers.Count);
		ConvolutionalLayer randomLayer = ConvolutionalLayers[randomLayerIndex];

		return randomLayer.IntroduceRandomError();
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

	private void SetInitialWeights()
	{
		foreach (ConvolutionalLayer layer in ConvolutionalLayers)
		{
			layer.SetInitialWeights();
		}
	}

	private void RandomizeAllWeights()
	{
		foreach (ConvolutionalLayer layer in ConvolutionalLayers)
		{
			layer.RandomizeAllWeights();
		}
	}

	public int GetOutputLayerNeuronCount()
	{
		PoolingLayer lastLayer = PoolingLayers[^1];

		return lastLayer.OutputPixelCount.x * lastLayer.OutputPixelCount.y * ColorChannelCount;
	}
}