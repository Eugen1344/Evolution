using System;
using Newtonsoft.Json;

[Serializable]
[JsonObject(MemberSerialization.OptOut)]
public class NeuralNetworkSettings
{
	[JsonProperty("layers")]
	public int[] NeuronsCount;
	[JsonProperty("error_min")]
	public float MinRandomErrorCoefficient;
	[JsonProperty("error_max")]
	public float MaxRandomErrorCoefficient;

	public NeuralNetworkSettings(params int[] neuronsCount)
	{
		NeuronsCount = neuronsCount;
	}
}