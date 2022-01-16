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
	private int _inputNeuronCount;
	private int _outputNeuronCount;

	private void Start()
	{
		InputModules.Add(StaticSignalModule);
		InputModules.Add(Car.Food);
		InputModules.Add(Car.FoodPleasure);
		InputModules.Add(Car.Movement);
		InputModules.Add(Car.Eye);

		OutputModules.Add(Car.Movement);

		_inputNeuronCount = InputModules.Sum(module => module.InputNeuronCount);
		_outputNeuronCount = OutputModules.Sum(module => module.OutputNeuronCount);
	}

	public async Task TryMakeDecisionAsync()
	{
		if (!IsTimeToMakeDecision())
			return;

		_prevDecisionRealtimeSinceStartup = Time.time;
		await UpdateNetwork();

		OnMadeDecision?.Invoke(Car);
	}

	private bool IsTimeToMakeDecision()
	{
		return Time.time >= _prevDecisionRealtimeSinceStartup + 1.0f / DecisionsPerSecond;
	}

	private async Task UpdateNetwork()
	{
		float[] inputData = await GetInputData();

		if (inputData.Length != _inputNeuronCount || Network.FirstLayer.Size != _inputNeuronCount)
			throw new ArgumentException($"Input neuron count is {Network.FirstLayer.Size}. Should be {_inputNeuronCount}");

		float[] result = Network.Calculate(inputData);

		if (result.Length != _outputNeuronCount || Network.LastLayer.Size != _outputNeuronCount)
			throw new ArgumentException($"Output output neuron count is {Network.LastLayer.Size}. Should be {_outputNeuronCount}");

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