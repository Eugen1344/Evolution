using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class GenerationsEvolutionSettings
{
	public int InitialSpeciesCount;
	public int SpeciesPerGeneration;
	public int BestSpeciesPerGeneration;
	public int BestSpeciesPerGenerationClones;
	public NeuralNetworkSettings NeuralNetworkSettings;
	public ConvolutionalNeuralNetworkSettings EyeNeuralNetworkSettings;
	public bool RespawnFoodEachGeneration;
	public float TimeScale;

	public void InitializeCommandBarFields()
	{
		Commands.AddFloatField("Time scale", () => TimeScale, time => TimeScale = time);
	}
}