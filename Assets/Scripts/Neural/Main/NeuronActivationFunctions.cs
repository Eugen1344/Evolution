using UnityEngine;

public static class NeuronActivationFunctions
{
	private const float ActivationThreshold = 0.4f;
	
	public static float ActivationTanh(float value)
	{
		float exp = Mathf.Exp(value);
		float inverseExp = Mathf.Exp(-value);

		float result = (exp - inverseExp) / (exp + inverseExp);
		return result > 0 ? result : 0;
	}
	
	public static float ActivationBinary(float value)
	{
		return value >= ActivationThreshold ? 1 : 0;
	}

	public static float ActivationRelu(float value)
	{
		return value < 0 ? 0 : value;
	}
	
	public static float ActivationLinear(float value)
	{
		return value;
	}
}