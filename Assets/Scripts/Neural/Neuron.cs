using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Neuron
{
	public const float MaxWeight = 1.0f;

	public float[] Weights;

	public Neuron(int weightsCount)
	{
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

	public void IntroduceRandomError(float errorCoefficient)
	{
		if (Weights == null)
			return;

		int randomWeightIndex = Random.Range(0, Weights.Length);
		bool isErrorPositive = Random.value < 0.5f;

		float weight = Weights[randomWeightIndex];
		float weightDelta = MaxWeight * errorCoefficient;
		float newWeight = Mathf.Clamp(isErrorPositive ? weight + weightDelta : weight - weightDelta, -MaxWeight, MaxWeight);

		Weights[randomWeightIndex] = newWeight;
	}
}