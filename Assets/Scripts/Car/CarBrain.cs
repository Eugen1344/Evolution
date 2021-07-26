using UnityEngine;

public class CarBrain : MonoBehaviour
{
	public const int InputNeuronsCount = 2;
	public const int OutputNeuronsCount = 6;

	public float DecisionsPerSecond;
	public CarController MovementController;
	public CarFood FoodController;

	public NeuralNetwork Network;

	private float _prevDecisionRealtimeSinceStartup;

	private void Awake()
	{
		Network = NeuralNetwork.Random(new NeuralNetworkSettings(InputNeuronsCount, 1000, 1000, 1000, 1000, 1000, OutputNeuronsCount));
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

		Debug.Log($"Wheel (Front Left): {result[0]}; Wheel (Rear Left): {result[1]};  Wheel (Front Right): {result[2]};  Wheel (Rear Right): {result[3]}; Steering (Left): {result[4]}; Steering (Right): {result[5]}");

		MovementController.SetSpeed(WheelType.FrontLeft, result[0]);
		MovementController.SetSpeed(WheelType.RearLeft, result[1]);
		MovementController.SetSpeed(WheelType.FrontRight, result[2]);
		MovementController.SetSpeed(WheelType.RearRight, result[3]);
		MovementController.SetSteering(WheelType.FrontLeft, result[4]);
		MovementController.SetSteering(WheelType.FrontRight, result[5]);
	}

	private float[] GetInputData()
	{
		return new[] { FoodController.GetNormalizedFoodAmount(), MovementController.GetTotalSpeed() };
	}
}