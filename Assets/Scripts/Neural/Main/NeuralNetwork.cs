using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class NeuralNetwork
{
	[JsonProperty("layers")] public List<Layer> NeuronLayers;
	[JsonProperty("threshold")] public float ActivationThreshold;
	[JsonProperty("settings")] public NeuralNetworkSettings Settings;

	public NeuralNetwork()
	{
	}

	public NeuralNetwork(NeuralNetworkSettings settings)
	{
		Settings = settings;
		
		NeuronLayers = new List<Layer>();

		ActivationThreshold = Settings.ActivationThreshold;

		InitializeNeurons(settings);
	}

	public NeuralNetwork(NeuralNetwork network)
	{
		Settings = network.Settings;

		NeuronLayers = new List<Layer>();

		ActivationThreshold = network.ActivationThreshold;

		InitializeNeurons(network);
	}

	public float[] Calculate(float[] input)
	{
		float[] output = input;

		for (int i = 1; i < NeuronLayers.Count; i++)
		{
			bool isLastLayer = i >= NeuronLayers.Count - 1;
			output = NeuronLayers[i].Calculate(output, Settings.ActivationThreshold, isLastLayer);
		}

		return output;
	}

	public static NeuralNetwork Random(NeuralNetworkSettings settings)
	{
		NeuralNetwork network = new NeuralNetwork(settings);
		network.RandomizeAllWeights();

		return network;
	}

	private void InitializeNeurons(NeuralNetwork network)
	{
		foreach (Layer layer in network.NeuronLayers)
		{
			Layer copiedLayer = new Layer(layer);

			NeuronLayers.Add(copiedLayer);
		}
	}

	private void InitializeNeurons(NeuralNetworkSettings settings)
	{
		int prevLayerCount = 0;

		foreach (int count in settings.NeuronsCount)
		{
			Layer layer = new Layer(count, prevLayerCount);

			NeuronLayers.Add(layer);

			prevLayerCount = count;
		}
	}

	public float IntroduceRandomError()
	{
		if (NeuronLayers.Count <= 1)
			return 0;

		int randomLayerIndex = UnityEngine.Random.Range(1, NeuronLayers.Count);
		Layer randomLayer = NeuronLayers[randomLayerIndex];

		int randomNeuronIndex = UnityEngine.Random.Range(0, randomLayer.Size);
		Neuron randomNeuron = randomLayer.Neurons[randomNeuronIndex];

		float randomNeuronError = UnityEngine.Random.Range(Settings.MinRandomErrorCoefficient, Settings.MaxRandomErrorCoefficient);
		float randomThresholdError = Settings.ActivationThresholdError;

		bool isErrorNegative = UnityEngine.Random.value <= 0.5f;
		if (isErrorNegative)
		{
			randomNeuronError *= -1;
			randomThresholdError *= -1;
		}

		randomNeuron.IntroduceError(randomNeuronError);
		ActivationThreshold = Mathf.Clamp(ActivationThreshold + randomThresholdError, 0, 1);

		return randomNeuronError;
	}

	private void RandomizeAllWeights()
	{
		foreach (Layer layer in NeuronLayers)
		{
			layer.RandomizeAllWeights();
		}
	}
}