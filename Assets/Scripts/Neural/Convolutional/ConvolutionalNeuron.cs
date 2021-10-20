using System;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class ConvolutionalNeuron
{
	public const float MaxWeight = 1.0f;

	[JsonProperty("weights")]
	public float[,] Weights;

	public int WeightsLengthX => Weights.GetLength(0);
	public int WeightsLengthY => Weights.GetLength(1);

	public ConvolutionalNeuron()
	{
		Weights = null;
	}

	public ConvolutionalNeuron(int weightsLengthX, int weightsLengthY)
	{
		if (weightsLengthX == 0 || weightsLengthY == 0)
			Weights = null;
		else
			Weights = new float[weightsLengthX, weightsLengthY];
	}

	public ConvolutionalNeuron(float[,] weights)
	{
		Weights = weights;
	}

	public ConvolutionalNeuron(ConvolutionalNeuron neuron)
	{
		if (neuron.Weights == null)
			Weights = null;
		else
			Weights = (float[,])neuron.Weights.Clone();
	}

	public float Calculate(float[,] input, int maskPositionX, int maskPositionY)
	{
		float sum = 0;

		for (int i = 0; i < WeightsLengthX; i++)
		{
			for (int j = 0; j < WeightsLengthY; j++)
			{
				int inputX = i + maskPositionX;
				int inputY = j + maskPositionY;

				if (inputX >= input.GetLength(0) || inputY >= input.GetLength(1))
					continue;

				sum += input[inputX, inputY] * Weights[i, j];
			}
		}

		return Activation(sum);
	}

	private float Activation(float value)
	{
		if (value <= 0)
			return 0;

		float exp = Mathf.Exp(value);
		float inverseExp = Mathf.Exp(-value);

		return (exp - inverseExp) / (exp + inverseExp);
	}

	public void SetInitialWeights()
	{
		if (Weights == null)
			return;

		float averageWeight = 1.0f / (WeightsLengthX * WeightsLengthY);

		for (int i = 0; i < WeightsLengthX; i++)
		{
			for (int j = 0; j < WeightsLengthY; j++)
			{
				Weights[i, j] = averageWeight;
			}
		}
	}

	public void SetRandomWeights()
	{
		if (Weights == null)
			return;

		for (int i = 0; i < WeightsLengthX; i++)
		{
			for (int j = 0; j < WeightsLengthY; j++)
			{
				Weights[i, j] = Random.Range(-1.0f, 1.0f);
			}
		}
	}

	public void IntroduceError(float errorCoefficient)
	{
		if (Weights == null)
			return;

		int randomWeightIndexX = Random.Range(0, WeightsLengthX);
		int randomWeightIndexY = Random.Range(0, WeightsLengthY);

		float weight = Weights[randomWeightIndexX, randomWeightIndexY];
		float weightDelta = MaxWeight * errorCoefficient;
		float newWeight = Mathf.Clamp(weight + weightDelta, -MaxWeight, MaxWeight);

		Weights[randomWeightIndexX, randomWeightIndexY] = newWeight;
	}
}