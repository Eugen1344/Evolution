using System;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class ConvolutionalNeuron
{
	public const float MaxWeight = 1.0f;

	[JsonProperty("weights")] public float[,] Weights;

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
		float[,] result = new float[WeightsLengthX, WeightsLengthY];

		for (int i = 0; i < WeightsLengthX; i++)
		{
			for (int j = 0; j < WeightsLengthY; j++)
			{
				int inputX = maskPositionX + i;
				int inputY = maskPositionY + j;

				if (inputX >= input.GetLength(0) || inputY >= input.GetLength(1))
					continue;

				result[i, j] = input[inputX, inputY] * Weights[i, j];
			}
		}

		float pooling = MaxPooling(result);
		float output = Activation(pooling);
		return output;
	}

	private float AveragePooling(float[,] output)
	{
		float sum = 0;
		int sizeX = output.GetLength(0);
		int sizeY = output.GetLength(1);

		for (int i = 0; i < sizeX; i++)
		{
			for (int j = 0; j < sizeY; j++)
			{
				sum += output[i, j];
			}
		}

		float average = sum / (sizeX * sizeY);
		return average;
	}

	private float MaxPooling(float[,] output)
	{
		int sizeX = output.GetLength(0);
		int sizeY = output.GetLength(1);

		float max = output[0, 0];

		for (int i = 0; i < sizeX; i++)
		{
			for (int j = 0; j < sizeY; j++)
			{
				float value = output[i, j];

				if (Mathf.Abs(value) > max)
					max = value;
			}
		}

		return max;
	}

	private float Activation(float value)
	{
		return Mathf.Max(0, value);
	}

	public void SetInitialWeights()
	{
		if (Weights == null)
			return;

		for (int i = 0; i < WeightsLengthX; i++)
		{
			for (int j = 0; j < WeightsLengthY; j++)
			{
				Weights[i, j] = 0;
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