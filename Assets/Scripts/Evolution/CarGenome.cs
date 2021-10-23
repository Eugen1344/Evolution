using System;
using Newtonsoft.Json;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class CarGenome
{
	[JsonProperty("brain_network")]
	public NeuralNetwork BrainNetwork;
	//[JsonProperty("eye_network")]
	public ConvolutionalNeuralNetwork EyeNetwork;

	public CarGenome()
	{

	}

	public CarGenome(NeuralNetwork brain, ConvolutionalNeuralNetworkSettings eyeSettings)
	{
		BrainNetwork = brain;
		EyeNetwork = ConvolutionalNeuralNetwork.Initial(eyeSettings);
	}

	public CarGenome(CarGenome genome)
	{
		BrainNetwork = new NeuralNetwork(genome.BrainNetwork);
		EyeNetwork = new ConvolutionalNeuralNetwork(genome.EyeNetwork);
	}

	public float IntroduceRandomError() //TODO maybe should not introduce both error simultaneously
	{
		//EyeNetwork.IntroduceRandomError();
		return BrainNetwork.IntroduceRandomError();
	}
}