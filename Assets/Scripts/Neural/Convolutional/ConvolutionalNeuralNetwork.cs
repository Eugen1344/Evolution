﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class ConvolutionalNeuralNetwork
{
	public const int ColorChannelCount = 3;

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

	public float[,,] Calculate(float[,,] input)
	{
		float[,,] output = input;

		for (int i = 0; i < NeuronLayers.Count; i++)
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
		for (int i = 0; i < network.Settings.NeuronsCount.Length; i++)
		{
			ConvolutionalLayer layer = network.NeuronLayers[i];
			ConvolutionalLayer copiedLayer = new ConvolutionalLayer(layer);
			Vector2Int count = network.Settings.NeuronsCount[i];
			copiedLayer.PixelCount = count;
			copiedLayer.Settings = network.Settings;

			NeuronLayers.Add(copiedLayer);
		}
	}

	private void InitializeNeurons(ConvolutionalNeuralNetworkSettings settings)
	{
		for (int i = 0; i < settings.NeuronsCount.Length; i++)
		{
			Vector2Int count = settings.NeuronsCount[i];
			ConvolutionalLayer layer = i == 0 ? ConvolutionalLayer.First(settings, count) : new ConvolutionalLayer(settings, count);

			NeuronLayers.Add(layer);
		}
	}

	public float IntroduceRandomError()
	{
		if (NeuronLayers.Count <= 1)
			return 0;

		int randomLayerIndex = UnityEngine.Random.Range(1, NeuronLayers.Count);
		ConvolutionalLayer randomLayer = NeuronLayers[randomLayerIndex];

		return randomLayer.IntroduceRandomError();
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

	public int GetOutputLayerNeuronCount()
	{
		if (NeuronLayers == null || NeuronLayers.Count == 0)
			return 0;

		ConvolutionalLayer lastLayer = NeuronLayers[^1];

		return lastLayer.PixelCount.x * lastLayer.PixelCount.y;
	}
}