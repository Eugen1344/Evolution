using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarBrain : MonoBehaviour
{
	public float DecisionsPerSecond;
	public Car Car;
	public StaticSignalInputModule StaticSignalModule;

	public NeuralNetwork Network;
	public List<IInputNeuralModule> InputModules = new List<IInputNeuralModule>();
	public List<IOutputNeuralModule> OutputModules = new List<IOutputNeuralModule>();

	public event Action<Car> OnMadeDecision;

	private float _prevDecisionRealtimeSinceStartup;

	private void Start()
	{
		InputModules.Add(StaticSignalModule);
		InputModules.Add(Car.Food);
		InputModules.Add(Car.FoodPleasure);
		InputModules.Add(Car.Movement);
		InputModules.Add(Car.Eye);

		OutputModules.Add(Car.Movement);
	}

	private void Update()
	{
		if (!IsTimeToMakeDecision())
			return;

		UpdateNetwork();

		_prevDecisionRealtimeSinceStartup = Time.time;

		OnMadeDecision?.Invoke(Car);
	}

	private bool IsTimeToMakeDecision()
	{
		return Time.time >= _prevDecisionRealtimeSinceStartup + 1.0f / DecisionsPerSecond;
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

		int inputNeuronsCount = InputModules.Sum(module => module.InputNeuronCount);
		int outputNeuronsCount = OutputModules.Sum(module => module.OutputNeuronCount);
	}

	private float[] GetInputData()
	{
		return InputModules.SelectMany(module => module.GetInput()).ToArray();
	}
}