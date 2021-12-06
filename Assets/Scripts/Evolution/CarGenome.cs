using System;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class CarGenome
{
	public float ColorChangeDelta = 0.05f;

	[JsonProperty("brain_network")]
	public NeuralNetwork BrainNetwork;
	//[JsonProperty("eye_network")]
	public ConvolutionalNeuralNetwork LeftEyeNetwork;
	public ConvolutionalNeuralNetwork RightEyeNetwork;
	[JsonProperty("generation")]
	public int Generation;
	[JsonProperty("color")]
	public Color Color;

	public CarGenome()
	{

	}

	public CarGenome(NeuralNetwork brain, ConvolutionalNeuralNetwork leftEye, ConvolutionalNeuralNetwork rightEye)
	{
		BrainNetwork = brain;
		LeftEyeNetwork = leftEye;
		RightEyeNetwork = rightEye;
	}

	public CarGenome(NeuralNetworkSettings brainSettings, ConvolutionalNeuralNetworkSettings eyeSettings)
	{
		BrainNetwork = NeuralNetwork.Random(brainSettings);
		LeftEyeNetwork = ConvolutionalNeuralNetwork.Initial(eyeSettings);
		RightEyeNetwork = ConvolutionalNeuralNetwork.Initial(eyeSettings);
		Generation = 0;
		Color = RandomColor();
	}

	public CarGenome(CarGenome genome)
	{
		BrainNetwork = new NeuralNetwork(genome.BrainNetwork);
		LeftEyeNetwork = new ConvolutionalNeuralNetwork(genome.LeftEyeNetwork);
		RightEyeNetwork = new ConvolutionalNeuralNetwork(genome.RightEyeNetwork);
		Generation = genome.Generation;
		Color = genome.Color;
	}

	public float IntroduceRandomError()
	{
		Color = NextColor(Color);
		return BrainNetwork.IntroduceRandomError();
	}

	private Color NextColor(Color prevColor)
	{
		int colorChannel = Random.Range(0, 3);
		Color nextColor = prevColor;
		nextColor.a = 1;
		float colorDelta = Random.value <= 0.5 ? ColorChangeDelta : -ColorChangeDelta;

		switch (colorChannel)
		{
			case 0:
				nextColor.r = Mathf.Clamp(nextColor.r + colorDelta, 0, 1);
				break;
			case 1:
				nextColor.g = Mathf.Clamp(nextColor.g + colorDelta, 0, 1);;
				break;
			case 2:
				nextColor.b = Mathf.Clamp(nextColor.b + colorDelta, 0, 1);;
				break;
		}

		return nextColor;
	}

	private Color RandomColor()
	{
		return new Color(Random.value, Random.value, Random.value, 1);
	}
}