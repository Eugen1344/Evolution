using System;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class Neuron
{
	public const float MaxWeight = 1.0f;
	public const float MemoryMultiplier = 0.01f;

	[JsonProperty("weights")] public float[] Weights;
	//public float[] Memory;

	public Neuron(int weightsCount)
	{
		if (weightsCount == 0)
		{
			Weights = null;
			//Memory = null;
		}
		else
		{
			Weights = new float[weightsCount];
			//Memory = new float[weightsCount];
		}
	}

	public Neuron(Neuron neuron)
	{
		if (neuron.Weights == null)
		{
			Weights = null;
			//Memory = null;
		}
		else
		{
			Weights = (float[]) neuron.Weights.Clone();
			//Memory = new float[Weights.Length];
		}
	}

	public Neuron()
	{
		Weights = null;
		//Memory = null;
	}

	public float Calculate(float[] input)
	{
		float sum = 0;

		if (Weights.Length != input.Length)
			throw new ArgumentException("Wrong neuron count");

		for (int i = 0; i < input.Length; i++)
		{
			//float weight = Mathf.Clamp(Weights[i] + Memory[i], -MaxWeight, MaxWeight);
			//sum += input[i] * (weight);
			
			sum += input[i] * (Weights[i]);
			
			//Memory[i] += MemoryActivation(sum);
			//Memory[i] = Mathf.Clamp(Memory[i], -1.0f, 1.0f);
		}

		return Activation(sum);
	}

	private float Activation(float value)
	{
		float exp = Mathf.Exp(value);
		float inverseExp = Mathf.Exp(-value);

		return (exp - inverseExp) / (exp + inverseExp);
	}

	private float MemoryActivation(float neuronOutput)
	{
		return Activation(neuronOutput) * MemoryMultiplier;
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