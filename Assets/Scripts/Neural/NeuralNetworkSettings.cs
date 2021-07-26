using System;

[Serializable]
public class NeuralNetworkSettings
{
	public int[] NeuronsCount;

	public NeuralNetworkSettings(params int[] neuronsCount)
	{
		NeuronsCount = neuronsCount;
	}
}