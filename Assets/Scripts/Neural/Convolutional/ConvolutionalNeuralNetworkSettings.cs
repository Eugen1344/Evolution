using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class ConvolutionalNeuralNetworkSettings
{
	[JsonProperty("input_size")] public Vector2Int InputPixelCount;
	[JsonProperty("layer_count")] public int LayerCount;
	[JsonProperty("filter_size")] public Vector2Int FilterSize;
	[JsonProperty("pooling_size")] public Vector2Int PoolingSize;
	[JsonProperty("stride")] public int Stride;
	[JsonProperty("padding")] public int Padding;
	[JsonProperty("error_min")] public float MinRandomErrorCoefficient;
	[JsonProperty("error_max")] public float MaxRandomErrorCoefficient;
}