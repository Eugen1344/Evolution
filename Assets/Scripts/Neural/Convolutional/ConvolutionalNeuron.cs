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

	public int WeightsLengthX;
	public int WeightsLengthY;
	public int WeightsLengthZ;

	private float[,,] _result;

	public ConvolutionalNeuron()
	{
		Weights = null;
	}

	public ConvolutionalNeuron(int weightsLengthX, int weightsLengthY, int weightsLengthZ)
	{
		if (weightsLengthX == 0 || weightsLengthY == 0)
		{
			Weights = null;
		}
		else
		{
			WeightsLengthX = weightsLengthX;
			WeightsLengthY = weightsLengthY;
			WeightsLengthZ = weightsLengthZ;

			Weights = new float[weightsLengthX, weightsLengthY, weightsLengthZ];
			_result = new float[weightsLengthX, weightsLengthY, weightsLengthZ];
		}
	}

	public ConvolutionalNeuron(float[,,] weights)
	{
		WeightsLengthX = weights.GetLength(0);
		WeightsLengthY = weights.GetLength(1);
		WeightsLengthZ = weights.GetLength(2);

		Weights = weights;
		_result = new float[WeightsLengthX, WeightsLengthY, WeightsLengthZ];
	}

	public ConvolutionalNeuron(ConvolutionalNeuron neuron)
	{
		if (neuron.Weights == null)
		{
			Weights = null;
		}
		else
		{
			WeightsLengthX = neuron.Weights.GetLength(0);
			WeightsLengthY = neuron.Weights.GetLength(1);
			WeightsLengthZ = neuron.Weights.GetLength(2);

			Weights = (float[,,]) neuron.Weights.Clone();

			_result = new float[WeightsLengthX, WeightsLengthY, WeightsLengthZ];
		}
	}

	public float Calculate(float[,,] input, int inputLengthX, int inputLengthY, int maskPositionX, int maskPositionY)
	{
		float sum = 0;

		for (int k = 0; k < WeightsLengthZ; k++)
		{
			int inputZ = k;

			for (int i = 0; i < WeightsLengthX; i++)
			{
				int inputX = maskPositionX + i;

				for (int j = 0; j < WeightsLengthY; j++)
				{
					int inputY = maskPositionY + j;

					if (inputX >= inputLengthX || inputY >= inputLengthY)
						continue;

					float value = input[inputX, inputY, inputZ] * Weights[i, j, k];

					sum += value;
				}
			}
		}

		return Activation(sum);
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
					Weights[i, j, k] = 1;
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