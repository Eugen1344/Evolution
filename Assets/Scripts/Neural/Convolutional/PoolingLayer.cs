using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class PoolingLayer : AbstractConvolutionalLayer
{
	public PoolingLayer(ConvolutionalNeuralNetworkSettings settings, Vector2Int inputPixelCount) : base(settings, inputPixelCount)
	{
	}

	public override float[,,] Calculate(float[,,] input)
	{
		float max = 0;

		for (int k = 0; k < ConvolutionalNeuralNetwork.ColorChannelCount; k++)
		{
			int inputPositionX = 0;

			for (int i = 0; i < OutputPixelCount.x; i++)
			{
				int inputPositionY = 0;

				for (int j = 0; j < OutputPixelCount.y; j++)
				{
					float result = MaxPooling(input, inputPositionX, inputPositionY, k);

					_output[i, j, k] = result;

					inputPositionY += Settings.PoolingSize.y;

					if (result > max)
						max = result;
				}

				inputPositionX += Settings.PoolingSize.x;
			}
		}

		Normalize(max);

		return _output;
	}

	private void Normalize(float maxValue)
	{
		if (maxValue <= 0)
			return;

		for (int k = 0; k < ConvolutionalNeuralNetwork.ColorChannelCount; k++)
		{
			for (int i = 0; i < OutputPixelCount.x; i++)
			{
				for (int j = 0; j < OutputPixelCount.y; j++)
				{
					_output[i, j, k] /= maxValue;
				}
			}
		}
	}

	private float MaxPooling(float[,,] input, int maskPositionX, int maskPositionY, int maskPositionZ)
	{
		float max = input[maskPositionX, maskPositionY, maskPositionZ];

		for (int i = 0; i < Settings.PoolingSize.x; i++)
		{
			int inputX = maskPositionX + i;

			for (int j = 0; j < Settings.PoolingSize.y; j++)
			{
				int inputY = maskPositionY + j;

				if (inputX >= _inputPixelCount.x || inputY >= _inputPixelCount.y || inputX < 0 || inputY < 0)
					continue;

				float value = input[inputX, inputY, maskPositionZ];

				if (value > max)
				{
					max = value;
				}
			}
		}

		return max;
	}

	public override Vector2Int OutputSize(Vector2Int inputSize)
	{
		return Vector2Int.CeilToInt((Vector2) inputSize / Settings.PoolingSize);
	}
}