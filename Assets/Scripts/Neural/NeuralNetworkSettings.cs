using System;

[Serializable]
public class NeuralNetworkSettings
{
	public int[] NeuronsCount;
	public float MinRandomErrorCoefficient;
	public float MaxRandomErrorCoefficient;

	public NeuralNetworkSettings(params int[] neuronsCount)
	{
		NeuronsCount = neuronsCount;
	}
}