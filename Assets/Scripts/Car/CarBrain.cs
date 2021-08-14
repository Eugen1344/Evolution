using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarBrain : MonoBehaviour
{
	public float DecisionsPerSecond;
	public Car Car;
	public StaticSignalInputModule StaticSignalModule;

	public NeuralNetwork Network;
	public List<IInputNeuralModule> InputModules;
	public List<IOutputNeuralModule> OutputModules;

	private float _prevDecisionRealtimeSinceStartup;

	private void Awake()
	{
		InputModules.Add(StaticSignalModule);
		InputModules.Add(Car.Food);
		InputModules.Add(Car.Movement);

		OutputModules.Add(Car.Movement);
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
		int startingIndex = 0;

		foreach (IOutputNeuralModule outputModule in OutputModules)
		{
			outputModule.SetOutput(result, startingIndex);

			startingIndex += outputModule.OutputNeuronCount;
		}
	}

	private float[] GetInputData()
	{
		return InputModules.SelectMany(module => module.GetInput()).ToArray();
	}
}