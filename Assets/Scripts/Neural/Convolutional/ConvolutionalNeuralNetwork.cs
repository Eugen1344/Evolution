using System.Collections.Generic;
using Newtonsoft.Json;

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

	public float[] Calculate(float[] input)
	{
		float[] output = input;

		for (int i = 1; i < NeuronLayers.Count; i++)
		{
			output = NeuronLayers[i].Calculate(output);
		}

		return output;
	}

	public static ConvolutionalNeuralNetwork Random(ConvolutionalNeuralNetworkSettings settings)
	{
		ConvolutionalNeuralNetwork network = new ConvolutionalNeuralNetwork(settings);
		network.RandomizeAllWeights();

		return network;
	}

	private void InitializeNeurons(ConvolutionalNeuralNetwork network)
	{
		foreach (Layer layer in network.NeuronLayers)
		{
			Layer copiedLayer = new Layer(layer);

			NeuronLayers.Add(copiedLayer);
		}
	}

	private void InitializeNeurons(ConvolutionalNeuralNetworkSettings settings)
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

		float randomError = UnityEngine.Random.Range(Settings.MinRandomErrorCoefficient, Settings.MaxRandomErrorCoefficient);

		bool isErrorNegative = UnityEngine.Random.value < 0.5f;
		if (isErrorNegative)
			randomError *= -1;

		randomNeuron.IntroduceError(randomError);

		return randomError;
	}

	private void RandomizeAllWeights()
	{
		foreach (Layer layer in NeuronLayers)
		{
			layer.RandomizeAllWeights();
		}
	}
}