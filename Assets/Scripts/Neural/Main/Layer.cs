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

	public Layer(int neuronsCount, int prevLayerNeuronsCount, Func<float, float> neuronActivationFunction)
	{
		Neurons = new List<Neuron>();

		for (int i = 0; i < neuronsCount; i++)
		{
			Neurons.Add(new Neuron(prevLayerNeuronsCount, neuronActivationFunction));
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
	
	public void InitAfterDeserialization(Func<float,float> activationFunction)
	{
		foreach (Neuron neuron in Neurons)
		{
			neuron.InitAfterDeserialization(activationFunction);
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