using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization.OptOut)]
public class ConvolutionalNeuralNetworkSettings
{
	[JsonProperty("layers")]
	public Vector2Int[] NeuronsCount;
	[JsonProperty("mask")]
	public Vector2Int FilterSize;
	[JsonProperty("error_min")]
	public float MinRandomErrorCoefficient;
	[JsonProperty("error_max")]
	public float MaxRandomErrorCoefficient;

	public ConvolutionalNeuralNetworkSettings(params Vector2Int[] neuronsCount)
	{
		NeuronsCount = neuronsCount;
	}
}