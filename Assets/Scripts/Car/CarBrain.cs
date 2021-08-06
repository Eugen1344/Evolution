using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class CarBrain : MonoBehaviour
{
	public float DecisionsPerSecond;
	public Car Car;

	[JsonProperty("network")]
	public NeuralNetwork Network;

	private float _prevDecisionRealtimeSinceStartup;

	private void Awake()
	{
		//Network = NeuralNetwork.Random();
	}

	private void Update()
	{
		if (!TimeToMakeDecision())
			return;

		UpdateNetwork();

		_prevDecisionRealtimeSinceStartup = Time.realtimeSinceStartup;
	}

	private bool TimeToMakeDecision()
	{
		return Time.realtimeSinceStartup >= _prevDecisionRealtimeSinceStartup + 1.0f / DecisionsPerSecond;
	}

	private void UpdateNetwork()
	{
		float[] result = Network.Calculate(GetInputData());

		//Debug.Log($"Wheel (Front Left): {result[0]}; Wheel (Rear Left): {result[1]};  Wheel (Front Right): {result[2]};  Wheel (Rear Right): {result[3]}; Steering (Left): {result[4]}; Steering (Right): {result[5]}");

		Car.Movement.SetSpeed(WheelType.FrontLeft, result[0]);
		Car.Movement.SetSpeed(WheelType.RearLeft, result[1]);
		Car.Movement.SetSpeed(WheelType.FrontRight, result[2]);
		Car.Movement.SetSpeed(WheelType.RearRight, result[3]);
		Car.Movement.SetSteering(WheelType.FrontLeft, result[4]);
		Car.Movement.SetSteering(WheelType.FrontRight, result[5]);
	}

	private float[] GetInputData()
	{
		return new[] { 1, Car.Food.GetNormalizedFoodAmount(), Car.Movement.GetTotalNormalizedSpeed() };
	}
}