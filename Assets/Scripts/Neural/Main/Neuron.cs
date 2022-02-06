using System;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class Neuron
{
	public const float MaxWeight = 1.0f;
	public const float MemoryMultiplier = 0.001f;
	public const float MemoryDecay = 0.0001f;

	[JsonProperty("weights")] public float[] Weights;
	//public float[] Memory;

	private Func<float, float> _activationFunction;

	public Neuron(int weightsCount, Func<float, float> activationFunction)
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

		_activationFunction = activationFunction;
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

		_activationFunction = neuron._activationFunction;
	}

	private Neuron()
	{
	}

	public void InitAfterDeserialization(Func<float,float> activationFunction)
	{
		_activationFunction = activationFunction;
	}

	public float Calculate(float[] input)
	{
		float sum = 0;

		if (Weights.Length != input.Length)
			throw new ArgumentException("Wrong neuron count");

		for (int i = 0; i < input.Length; i++)
		{
			//float weight = Mathf.Clamp(Weights[i] + Memory[i], -MaxWeight, MaxWeight);
			//sum += input[i] * weight;
			sum += input[i] * Weights[i];
			//float memory = Memory[i] + MemoryActivation(sum) - MemoryDecay;
			//Memory[i] = Mathf.Clamp(memory, 0f, 1.0f);
		}

		return _activationFunction(sum);
	}

	private float MemoryActivation(float neuronOutput)
	{
		return _activationFunction(neuronOutput) * MemoryMultiplier;
	}

	public void SetRandomWeights()
	{
		if (Weights == null)
			return;

		for (int i = 0; i < Weights.Length; i++)
		{
			Weights[i] = Random.Range(-MaxWeight, MaxWeight);
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