using System;
using Newtonsoft.Json;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class NeuralNetworkSettings
{
	[JsonProperty("layers")]
	public int[] NeuronsCount;
	[JsonProperty("activation_threshold")]
	public float ActivationThreshold;
	[JsonProperty("activation_threshold_error")]
	public float ActivationThresholdError;
	[JsonProperty("error_min")]
	public float MinRandomErrorCoefficient;
	[JsonProperty("error_max")]
	public float MaxRandomErrorCoefficient;

	public NeuralNetworkSettings(params int[] neuronsCount)
	{
		NeuronsCount = neuronsCount;
	}
}