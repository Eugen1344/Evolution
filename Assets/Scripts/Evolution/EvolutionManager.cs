using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EvolutionManager : MonoBehaviour
{
	public string ExperimentName;
	public List<ContinuousEvolution> Evolutions;
	public List<FoodSpawner> FoodSpawners;
	[SerializeField] private SaveManager _saveManager = new SaveManager();
	public bool InitialSpawnAllFood;

	public event Action OnInitialSpawn;
	
	private void Start()
	{
		ExperimentName = RandomStringGenerator(10);

		Commands.AddStringField("Experiment Name", () => ExperimentName, val => ExperimentName = val);
	}

	public void InitialSpawn()
	{
		foreach (ContinuousEvolution evolution in Evolutions)
		{
			evolution.InitialSpawn();
		}

		TryRespawnAllFood();
		
		OnInitialSpawn?.Invoke();
	}

	public void TryRespawnAllFood()
	{
		if (InitialSpawnAllFood)
		{
			foreach (FoodSpawner spawner in FoodSpawners)
			{
				spawner.SpawnMaxObjects();
			}
		}
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
		
		TryRespawnAllFood();
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