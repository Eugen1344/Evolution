﻿using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class ConvolutionalNeuralNetworkSettings
{
	[JsonProperty("layers")]
	public Vector2Int[] NeuronsCount;
	[JsonProperty("filter_size")]
	public Vector2Int FilterSize;
	[JsonProperty("stride")]
	public int Overlap;
	[JsonProperty("error_min")]
	public float MinRandomErrorCoefficient;
	[JsonProperty("error_max")]
	public float MaxRandomErrorCoefficient;
	
	public ConvolutionalNeuralNetworkSettings(params Vector2Int[] neuronsCount)
	{
		NeuronsCount = neuronsCount;
	}
}