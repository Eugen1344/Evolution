using System;
using Newtonsoft.Json;
using UnityEngine;

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

	public Color GetColor()
	{
		float firstWeightsSum = 0;
		float secondWeightsSum = 0;
		float thirdWeightsSum = 0;

		int firstWeightsCount = 0;
		int secondWeightsCount = 0;
		int thirdWeightsCount = 0;

		int i = 0;
		foreach (Layer layer in BrainNetwork.NeuronLayers)
		{
			foreach (Neuron neuron in layer.Neurons)
			{
				if(neuron.Weights == null)
					continue;

				foreach (float weight in neuron.Weights)
				{
					if (i % 3 == 0)
					{
						firstWeightsSum += weight;
						firstWeightsCount++;
					}
					else if (i % 3 == 1)
					{
						secondWeightsSum += weight;
						secondWeightsCount++;
					}
					else
					{
						thirdWeightsSum += weight;
						thirdWeightsCount++;
					}

					i++;
				}
			}
		}

		float averageFirstWeight = firstWeightsSum / firstWeightsCount;
		float averageSecondWeight = secondWeightsSum / secondWeightsCount;
		float averageThirdWeight = thirdWeightsSum / thirdWeightsCount;

		return new Color(WeightToColorComponentLinear(averageFirstWeight, Neuron.MaxWeight), WeightToColorComponentLinear(averageSecondWeight, Neuron.MaxWeight), WeightToColorComponentLinear(averageThirdWeight, Neuron.MaxWeight));
	}

	private float WeightToColorComponentLinear(float weight, float maxWeight)
	{
		return weight / (2 * maxWeight) + 0.5f;
	}

	public float IntroduceRandomError() //TODO maybe should not introduce both error simultaneously
	{
		//EyeNetwork.IntroduceRandomError();
		return BrainNetwork.IntroduceRandomError();
	}
}