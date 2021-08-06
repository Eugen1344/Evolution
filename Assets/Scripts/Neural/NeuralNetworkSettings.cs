using System;
using LitJsonSrc;

[Serializable]
public class NeuralNetworkSettings : IJsonSerializable
{
	public int[] NeuronsCount;
	public float MinRandomErrorCoefficient;
	public float MaxRandomErrorCoefficient;

	private const string NeuronsCountKey = "neurons";
	private const string MinRandomErrorCoefficientKey = "min_error";
	private const string MaxRandomErrorCoefficientKey = "max_error";

	public NeuralNetworkSettings(params int[] neuronsCount)
	{
		NeuronsCount = neuronsCount;
	}

	public JsonData Serialize()
	{
		JsonData data = new JsonData
		{
			[NeuronsCountKey] = SerializeNeuronsCount(),
			[MinRandomErrorCoefficientKey] = MinRandomErrorCoefficient,
			[MaxRandomErrorCoefficientKey] = MaxRandomErrorCoefficient
		};

		return data;
	}

	private JsonData SerializeNeuronsCount()
	{
		JsonData data = new JsonData();

		for (int i = 0; i < NeuronsCount.Length; i++)
		{
			data[i] = NeuronsCount[i];
		}

		return data;
	}

	public void Deserialize(JsonData data)
	{
		DeserializeNeuronsCount(data[NeuronsCountKey]);
		MinRandomErrorCoefficient = (float)data[MinRandomErrorCoefficientKey];
		MaxRandomErrorCoefficient = (float)data[MaxRandomErrorCoefficientKey];
	}

	private void DeserializeNeuronsCount(JsonData data)
	{
		NeuronsCount = new int[data.Count];

		for (int i = 0; i < NeuronsCount.Length; i++)
		{
			NeuronsCount[i] = data[i].AsInt();
		}
	}
}