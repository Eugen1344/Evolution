using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Neuron
{
	public const float MaxWeight = 10.0f;

	public float[] Weights;

	public Neuron()
	{
		Weights = null;
	}

	public Neuron(int weightsCount)
	{
		Weights = new float[weightsCount];
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
}