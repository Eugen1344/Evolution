using System;
using Newtonsoft.Json;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class CarGenome
{
	[JsonProperty("brain_network")]
	public NeuralNetwork BrainNetwork;
	//[JsonProperty("eye_network")]
	public ConvolutionalNeuralNetwork LeftEyeNetwork;
	public ConvolutionalNeuralNetwork RightEyeNetwork;
	[JsonProperty("generation")]
	public int Generation;

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
	}

	public CarGenome(CarGenome genome)
	{
		BrainNetwork = new NeuralNetwork(genome.BrainNetwork);
		LeftEyeNetwork = new ConvolutionalNeuralNetwork(genome.LeftEyeNetwork);
		RightEyeNetwork = new ConvolutionalNeuralNetwork(genome.RightEyeNetwork);
		Generation = genome.Generation;
	}

	public float IntroduceRandomError() //TODO maybe should not introduce both error simultaneously
	{
		//EyeNetwork.IntroduceRandomError();
		return BrainNetwork.IntroduceRandomError();
	}
}