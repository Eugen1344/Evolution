using System;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class ConvolutionalNeuron
{
	public const float MaxWeight = 1.0f;

	[JsonProperty("weights")] public float[,,] Weights;

	public int WeightsLengthX => Weights.GetLength(0);
	public int WeightsLengthY => Weights.GetLength(1);
	public int WeightsLengthZ => Weights.GetLength(2);

	public ConvolutionalNeuron()
	{
		Weights = null;
	}

	public ConvolutionalNeuron(int weightsLengthX, int weightsLengthY, int weightsLengthZ)
	{
		if (weightsLengthX == 0 || weightsLengthY == 0)
			Weights = null;
		else
			Weights = new float[weightsLengthX, weightsLengthY, weightsLengthZ];
	}

	public ConvolutionalNeuron(float[,,] weights)
	{
		Weights = weights;
	}

	public ConvolutionalNeuron(ConvolutionalNeuron neuron)
	{
		if (neuron.Weights == null)
			Weights = null;
		else
			Weights = (float[,,]) neuron.Weights.Clone();
	}

	public float Calculate(float[,,] input, int maskPositionX, int maskPositionY)
	{
		float[,,] result = new float[WeightsLengthX, WeightsLengthY, WeightsLengthZ];

		for (int i = 0; i < WeightsLengthX; i++)
		{
			for (int j = 0; j < WeightsLengthY; j++)
			{
				for (int k = 0; k < WeightsLengthZ; k++)
				{
					int inputX = maskPositionX + i;
					int inputY = maskPositionY + j;
					int inputZ = 0;

					if (inputX >= input.GetLength(0) || inputY >= input.GetLength(1))
						continue;

					result[i, j, k] = input[inputX, inputY, inputZ] * Weights[i, j, k];
				}
			}
		}

		float pooling = MaxPooling(result);

		float output = Activation(pooling);
		return output;
	}

	private float AveragePooling(float[,,] output)
	{
		float sum = 0;
		int sizeX = output.GetLength(0);
		int sizeY = output.GetLength(1);
		int sizeZ = output.GetLength(1);

		for (int i = 0; i < sizeX; i++)
		{
			for (int j = 0; j < sizeY; j++)
			{
				for (int k = 0; k < sizeZ; k++)
				{
					sum += output[i, j, k];
				}
			}
		}

		float average = sum / (sizeX * sizeY);
		return average;
	}

	private float MaxPooling(float[,,] output)
	{
		int sizeX = output.GetLength(0);
		int sizeY = output.GetLength(1);
		int sizeZ = output.GetLength(1);

		float max = output[0, 0, 0];

		for (int i = 0; i < sizeX; i++)
		{
			for (int j = 0; j < sizeY; j++)
			{
				for (int k = 0; k < sizeZ; k++)
				{
					float value = output[i, j, k];

					if (Mathf.Abs(value) > max)
						max = value;
				}
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
				for (int k = 0; k < WeightsLengthZ; k++)
				{
					Weights[i, j, k] = 0;
				}
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
				for (int k = 0; k < WeightsLengthZ; k++)
				{
					Weights[i, j, k] = Random.Range(-1.0f, 1.0f);
				}
			}
		}
	}

	public void IntroduceError(float errorCoefficient)
	{
		if (Weights == null)
			return;

		int randomWeightIndexX = Random.Range(0, WeightsLengthX);
		int randomWeightIndexY = Random.Range(0, WeightsLengthY);
		int randomWeightIndexZ = Random.Range(0, WeightsLengthZ);

		float weight = Weights[randomWeightIndexX, randomWeightIndexY, randomWeightIndexZ];
		float weightDelta = MaxWeight * errorCoefficient;
		float newWeight = Mathf.Clamp(weight + weightDelta, -MaxWeight, MaxWeight);

		Weights[randomWeightIndexX, randomWeightIndexY, randomWeightIndexZ] = newWeight;
	}
}