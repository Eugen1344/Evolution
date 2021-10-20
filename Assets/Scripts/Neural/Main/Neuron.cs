using System;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
[JsonObject(MemberSerialization.OptOut)]
public class Neuron
{
	public const float MaxWeight = 1.0f;

	[JsonProperty("weights")]
	public float[] Weights;

	public Neuron(int weightsCount)
	{
		if (weightsCount == 0)
			Weights = null;
		else
			Weights = new float[weightsCount];
	}

	public Neuron(float[] weights)
	{
		Weights = weights;
	}

	public Neuron(Neuron neuron)
	{
		if (neuron.Weights == null)
			Weights = null;
		else
			Weights = (float[])neuron.Weights.Clone();
	}

	public Neuron()
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
			sum += input[i] * Weights[i];
		}

		return Activation(sum);
	}

	private float Activation(float value)
	{
		float exp = Mathf.Exp(value);
		float inverseExp = Mathf.Exp(-value);

		return (exp - inverseExp) / (exp + inverseExp);
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