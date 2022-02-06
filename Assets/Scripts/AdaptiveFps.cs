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

	[DisableInspectorEditing] [SerializeField]
	private List<float> _prevFitness;

	[DisableInspectorEditing] [SerializeField]
	private float? _prevAverageFitness;

	[DisableInspectorEditing] [SerializeField]
	private float _decisionsWithoutLearning = 0;

	public bool MaximizeFitness;

	private void Start()
	{
		_inputModules.Add(_staticSignal);
		_inputModules.AddRange(_spawners);

		_network.Settings.NeuronActivationFunction = NeuronActivationFunctions.ActivationLinear;
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
		if (_decisionsWithoutLearning >= DecisionsBetweenLearning)
			Learn();

		float networkOutput = CalculateNetwork();
		float newTimeScale = Mathf.Clamp(networkOutput, MinTimeScale, MaxTimeScale);
		Debug.Log($"{networkOutput} => {newTimeScale}");

		Time.timeScale = newTimeScale;

		_decisionsWithoutLearning++;

		float fitness = CurrentFitness(networkOutput);
		_prevFitness.Add(fitness);
	}

	private void Learn()
	{
		float fitness = AverageFitness();

		if (_prevAverageFitness != null && ((_prevAverageFitness > fitness && MaximizeFitness) || (_prevAverageFitness < fitness && !MaximizeFitness)))
		{
			_network = new NeuralNetwork(_prevNetwork);
			
			Debug.LogWarning("BACK");
		}
		else
		{
			_prevNetwork = new NeuralNetwork(_network);
			_prevAverageFitness = fitness;
			
			Debug.LogWarning("NEXT");
		}

		Debug.Log(_prevAverageFitness);

		_network.IntroduceRandomError();

		_prevFitness.Clear();
		_decisionsWithoutLearning = 0;

		/*foreach (Layer layer in _network.NeuronLayers)
		{
			foreach (Neuron neuron in layer.Neurons)
			{
				if (neuron.Weights == null)
					continue;

				foreach (float weight in neuron.Weights)
				{
					Debug.Log(weight);
				}

				Debug.Log("");
			}
		}

		Debug.Log("------------------------------------------------------------------");*/
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

	private float CurrentFitness(float currentOutput)
	{
		float fitness = currentOutput;
		float fpsDelta = TargetFps - _fpsCounter.CurrentFps;

		if (fpsDelta > 0)
			fitness -= fpsDelta * fpsDelta / 2.0f;

		return fitness;
	}
}