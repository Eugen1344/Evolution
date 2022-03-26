using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EvolutionManager : MonoBehaviour
{
	public List<ContinuousEvolution> Evolutions;
	public bool SpeedRewardEnabled;
	public bool SpeedPenaltyEnabled;
	public float SpeedFoodRatio;
	[SerializeField] private SaveManager _saveManager = new SaveManager();
	public event Action OnInitialSpawn;

	private string _experimentName;

	public string ExperimentName
	{
		get => _experimentName;
		private set
		{
			_experimentName = value;
			WindowTitle.Set($"{Application.productName} - {ExperimentName}");
		}
	}

	private void Start()
	{
		ExperimentName = RandomStringGenerator(10);

		Commands.AddStringField("Experiment Name", () => ExperimentName, val => ExperimentName = val);
		Commands.AddCheckbox("Reward speed", () => SpeedRewardEnabled, val => SpeedRewardEnabled = val);
		Commands.AddCheckbox("Punish speed", () => SpeedPenaltyEnabled, val => SpeedPenaltyEnabled = val);
		Commands.AddFloatField("Speed to food ratio", () => SpeedFoodRatio, val => SpeedFoodRatio = val);
	}

	public void InitialSpawn()
	{
		foreach (ContinuousEvolution evolution in Evolutions)
		{
			evolution.InitialSpawn();
		}

		OnInitialSpawn?.Invoke();
	}

	public void Save(string savePath)
	{
		SaveData data = GetSaveData();
		_saveManager.Save(savePath, data);
	}

	public void Load(string savePath)
	{
		SaveData data = _saveManager.Load(savePath);
		SetSaveData(data);
	}

	public SaveData GetSaveData()
	{
		SaveData data = new SaveData(ExperimentName);

		foreach (ContinuousEvolution evolution in Evolutions)
		{
			data.SaveAddEvolution(evolution);
		}

		return data;
	}

	public void SetSaveData(SaveData data)
	{
		ExperimentName = data.ExperimentName;

		for (int i = 0; i < Evolutions.Count; i++)
		{
			ContinuousEvolution evolution = Evolutions[i];
			List<CarGenome> genomes = data.Evolutions[i];

			evolution.SetPopulation(genomes);
		}
	}

	public Car GetRandomCar()
	{
		int evolutionIndex = Random.Range(0, Evolutions.Count);

		return Evolutions[evolutionIndex].GetCurrentBestCar();
	}

	private string RandomStringGenerator(int length)
	{
		string result = "";
		for (int i = 0; i < length; i++)
		{
			char c = (char) ('a' + Random.Range(0, 26));
			result += c;
		}

		return result;
	}
}