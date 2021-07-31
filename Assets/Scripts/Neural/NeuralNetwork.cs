using System;
using System.Collections.Generic;

[Serializable]
public class NeuralNetwork
{
	public NeuralNetworkSettings Settings;

	public List<Neuron[]> Neurons;

	public NeuralNetwork(NeuralNetworkSettings settings)
	{
		Settings = settings;

		Neurons = new List<Neuron[]>();

		InitializeNeurons(settings);
	}

	public NeuralNetwork(NeuralNetwork network)
	{
		Settings = network.Settings;

		Neurons = new List<Neuron[]>();

		InitializeNeurons(network);
	}

	public float[] Calculate(float[] input)
	{
		//float[] output = CalculateLayer(Neurons[0], input);
		float[] output = input;

		for (int i = 1; i < Neurons.Count; i++)
		{
			output = CalculateLayer(Neurons[i], output);
		}

		return output;
	}

	private float[] CalculateLayer(Neuron[] layer, float[] input)
	{
		float[] output = new float[layer.Length];

		for (int i = 0; i < output.Length; i++)
		{
			output[i] = layer[i].Calculate(input);
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
		foreach (Neuron[] originalLayer in network.Neurons)
		{
			Neuron[] copiedLayer = new Neuron[originalLayer.Length];

			for (int j = 0; j < copiedLayer.Length; j++)
			{
				copiedLayer[j] = new Neuron(originalLayer[j]);
			}

			Neurons.Add(copiedLayer);
		}
	}

	private void InitializeNeurons(NeuralNetworkSettings settings)
	{
		int prevLayerCount = 0;

		foreach (int count in Settings.NeuronsCount)
		{
			Neuron[] layer = new Neuron[count];

			for (int i = 0; i < layer.Length; i++)
			{
				if (prevLayerCount != 0)
				{
					layer[i] = new Neuron(prevLayerCount);
				}
				else
				{
					layer[i] = new Neuron();
				}
			}

			Neurons.Add(layer);

			prevLayerCount = count;
		}
	}

	public void IntroduceRandomError()
	{
		if (Neurons.Count <= 1)
			return;

		int randomLayerIndex = UnityEngine.Random.Range(1, Neurons.Count);
		Neuron[] randomLayer = Neurons[randomLayerIndex];

		int randomNeuronIndex = UnityEngine.Random.Range(0, randomLayer.Length);
		Neuron randomNeuron = randomLayer[randomNeuronIndex];

		randomNeuron.IntroduceRandomError(UnityEngine.Random.Range(Settings.MinRandomErrorCoefficient, Settings.MaxRandomErrorCoefficient));
	}

	private void RandomizeAllWeights()
	{
		foreach (Neuron[] layer in Neurons)
		{
			foreach (Neuron neuron in layer)
			{
				neuron.SetRandomWeights();
			}
		}
	}
}