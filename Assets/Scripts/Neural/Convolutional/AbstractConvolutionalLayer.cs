using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class AbstractConvolutionalLayer
{
	public Vector2Int OutputPixelCount;

	public float[,,] PrevOutput => _output;

	public ConvolutionalNeuralNetworkSettings Settings;

	protected float[,,] _output;
	protected Vector2Int _inputPixelCount;
	protected Vector2Int _convolutionPixelCount;

	protected AbstractConvolutionalLayer()
	{
	}

	public AbstractConvolutionalLayer(ConvolutionalNeuralNetworkSettings settings, Vector2Int inputPixelCount)
	{
		Settings = settings;
		_inputPixelCount = inputPixelCount;
		OutputPixelCount = OutputSize(inputPixelCount);

		_output = new float[OutputPixelCount.x, OutputPixelCount.y, ConvolutionalNeuralNetwork.ColorChannelCount];
	}

	public virtual float[,,] Calculate(float[,,] input)
	{
		_output = input;
		return input;
	}

	public virtual Vector2Int OutputSize(Vector2Int inputSize)
	{
		return _inputPixelCount;
	}
}