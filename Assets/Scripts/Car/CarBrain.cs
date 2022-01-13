using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

	public async Task TryMakeDecisionAsync()
	{
		if (!IsTimeToMakeDecision())
			return;

		await UpdateNetwork();

		_prevDecisionRealtimeSinceStartup = Time.time;

		OnMadeDecision?.Invoke(Car);
	}

	private bool IsTimeToMakeDecision()
	{
		return Time.time >= _prevDecisionRealtimeSinceStartup + 1.0f / DecisionsPerSecond;
	}

	private async Task UpdateNetwork()
	{
		float[] inputData = await GetInputData();
		float[] result = Network.Calculate(inputData);
		int startingIndex = 0;

		foreach (IOutputNeuralModule outputModule in OutputModules)
		{
			outputModule.SetOutput(result, startingIndex);

			startingIndex += outputModule.OutputNeuronCount;
		}
	}

	private async Task<float[]> GetInputData()
	{
		IEnumerable<float> inputModules = StaticSignalModule.GetInput().Concat(Car.Food.GetInput()).Concat(Car.FoodPleasure.GetInput()).Concat(Car.Movement.GetInput());
		IEnumerable<float> eyeInput = await Car.Eye.GetInputAsync();

		inputModules = inputModules.Concat(eyeInput);

		return inputModules.ToArray();
	}
}