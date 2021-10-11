using System;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
[JsonObject(MemberSerialization.OptOut)]
public class ConvolutionalNeuron
{
	public const float MaxWeight = 1.0f;

	[JsonProperty("weights")]
	public float[,] Weights;

	public ConvolutionalNeuron(int weightsCountX, int weight)
	{
		if (weightsCount == 0)
			Weights = null;
		else
			Weights = new float[,];
	}

	public ConvolutionalNeuron(float[,] weights)
	{
		Weights = weights;
	}

	public ConvolutionalNeuron(Neuron neuron)
	{
		if (neuron.Weights == null)
			Weights = null;
		else
			Weights = (float[])neuron.Weights.Clone();
	}

	public ConvolutionalNeuron()
	{
		Weights = null;
	}

	public float Calculate(float[] input)
	{
		float sum = 0;

		if (Weights.Length != input.Length)
			throw new ArgumentException("Wrong neuron count");

		for (int i = 0; i < input.Length; i++)
		{
			sum += Weights[i] * input[i];
		}

		return Activation(sum);
	}

	private float Activation(float value)
	{
		return 2.0f / (1 + Mathf.Exp(-value)) - 1.0f;
	}

	public void SetRandomWeights()
	{
		if (Weights == null)
			return;

		for (int i = 0; i < Weights.Length; i++)
		{
			Weights[i] = Random.Range(-1.0f, 1.0f);
		}
	}

	public void IntroduceError(float errorCoefficient)
	{
		if (Weights == null)
			return;

		int randomWeightIndex = Random.Range(0, Weights.Length);

		float weight = Weights[randomWeightIndex];
		float weightDelta = MaxWeight * errorCoefficient;
		float newWeight = Mathf.Clamp(weight + weightDelta, -MaxWeight, MaxWeight);

		Weights[randomWeightIndex] = newWeight;
	}
}