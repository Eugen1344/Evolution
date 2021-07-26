using System;
using System.Collections.Generic;

[Serializable]
public class NeuralNetwork
{
	public List<Neuron[]> Neurons;

	private NeuralNetworkSettings _settings;

	public NeuralNetwork(NeuralNetworkSettings settings)
	{
		_settings = settings;

		Neurons = new List<Neuron[]>();

		InitializeNeurons();
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

	private void InitializeNeurons()
	{
		int prevLayerCount = 0;

		foreach (int count in _settings.NeuronsCount)
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