using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class Layer
{
	[JsonProperty("neurons")]
	public List<Neuron> Neurons;

	public int Size => Neurons.Count;

	public Layer()
	{
		Neurons = null;
	}

	public Layer(int neuronsCount, int prevLayerNeuronsCount)
	{
		Neurons = new List<Neuron>();

		for (int i = 0; i < neuronsCount; i++)
		{
			Neurons.Add(new Neuron(prevLayerNeuronsCount));
		}
	}

	public Layer(Layer layer)
	{
		Neurons = new List<Neuron>();

		for (int i = 0; i < layer.Size; i++)
		{
			Neurons.Add(new Neuron(layer.Neurons[i]));
		}
	}

	public float[] Calculate(float[] input)
	{
		float[] output = new float[Neurons.Count];

		for (int i = 0; i < output.Length; i++)
		{
			output[i] = Neurons[i].Calculate(input);
		}

		return output;
	}

	public void RandomizeAllWeights()
	{
		foreach (Neuron neuron in Neurons)
		{
			neuron.SetRandomWeights();
		}
	}
}