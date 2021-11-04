using System;
using Newtonsoft.Json;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class ContinuousEvolutionSettings
{
	public int InitialSpeciesCount;
	public int EmergencyRespawnCount;
	public int ChildStoredFoodRequirement;
	public int StoredLifeResultCount;
	public NeuralNetworkSettings NeuralNetworkSettings;
	public ConvolutionalNeuralNetworkSettings EyeNeuralNetworkSettings;
	public bool RespawnAllFood;
	public float TimeScale;

	public void InitializeCommandBarFields()
	{
		Commands.AddFloatField("Time scale", () => TimeScale, time => TimeScale = time);
	}
}