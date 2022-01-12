using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AdaptiveFps : MonoBehaviour
{
	[SerializeField] private List<Spawner> _spawners;
	[SerializeField] private StaticSignalInputModule _staticSignal;
	[SerializeField] private FpsCounter _fpsCounter;
	[SerializeField] private NeuralNetwork _network;

	public int MinTimeScale;
	public int MaxTimeScale;
	public int TargetFps;
	public float DecisionsPerSecondRealtime;
	public int DecisionsBetweenLearning;
	public bool IsEnabled;

	private List<IInputNeuralModule> _inputModules = new List<IInputNeuralModule>();
	private List<IOutputNeuralModule> _outputModules = new List<IOutputNeuralModule>();

	private NeuralNetwork _prevNetwork;
	private float _prevDecisionRealtimeSinceStartup;
	[SerializeField] private List<float> _prevFitness;
	[SerializeField] private float? _prevAverageFitness;
	[SerializeField] private float _decisionsWithoutLearning = 0;

	public bool MaximizeFitness;

	private void Start()
	{
		_inputModules.AddRange(_spawners);
		_inputModules.Add(_staticSignal);

		_network = NeuralNetwork.Random(_network.Settings);
	}

	private void Update()
	{
		if (!IsEnabled || !IsTimeToMakeDecision() || _network == null)
			return;

		UpdateNetwork();

		_prevDecisionRealtimeSinceStartup = Time.unscaledTime;
	}

	private bool IsTimeToMakeDecision()
	{
		return Time.unscaledTime >= _prevDecisionRealtimeSinceStartup + 1.0f / DecisionsPerSecondRealtime;
	}

	private void UpdateNetwork()
	{
		float fitness = CurrentFitness();
		_prevFitness.Add(fitness);

		if (_decisionsWithoutLearning >= DecisionsBetweenLearning)
			Learn();

		float networkOutput = CalculateNetwork();
		float newTimeScale = (networkOutput + 1.0f) / 2.0f * MaxTimeScale;
		float clampedNewTimeScale = Mathf.Clamp(newTimeScale, MinTimeScale, MaxTimeScale);
		//Debug.Log($"{newTimeScale} => {clampedNewTimeScale}");

		Time.timeScale = clampedNewTimeScale;

		_decisionsWithoutLearning++;
	}

	private void Learn()
	{
		float fitness = AverageFitness();

		if (_prevAverageFitness != null && ((_prevAverageFitness > fitness && MaximizeFitness) || (_prevAverageFitness < fitness && !MaximizeFitness)))
		{
			_network = new NeuralNetwork(_prevNetwork);
		}
		else
		{
			_prevNetwork = new NeuralNetwork(_network);
			_prevAverageFitness = fitness;
		}
		
		Debug.Log(_prevAverageFitness);

		_network.IntroduceRandomError();

		_prevFitness.Clear();
		_decisionsWithoutLearning = 0;
	}

	private float CalculateNetwork()
	{
		float[] inputData = GetInputData();
		float[] result = _network.Calculate(inputData);

		return result[0];
	}

	private float[] GetInputData()
	{
		return _inputModules.SelectMany(module => module.GetInput()).ToArray();
	}

	private float AverageFitness()
	{
		return _prevFitness.Average();
	}

	private float CurrentFitness()
	{
		float fitness = Time.timeScale;
		float fpsDelta = TargetFps - _fpsCounter.AverageFps;

		if (fpsDelta > 0)
			fitness -= fpsDelta * fpsDelta / 2.0f;

		return fitness;
	}
}