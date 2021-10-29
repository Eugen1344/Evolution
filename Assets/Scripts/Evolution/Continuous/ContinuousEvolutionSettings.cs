using System;
using Newtonsoft.Json;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class ContinuousEvolutionSettings
{
	public int InitialSpeciesCount;
	public int SpeciesPerGeneration;
	public int ChildStoredFoodRequirement;
	public NeuralNetworkSettings NeuralNetworkSettings;
	public ConvolutionalNeuralNetworkSettings EyeNeuralNetworkSettings;
	public bool RespawnFoodEachGeneration;
	public float TimeScale;

	public void InitializeCommandBarFields()
	{
		Commands.AddFloatField("Time scale", () => TimeScale, time => TimeScale = time);
	}
}