﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ContinuousEvolution : MonoBehaviour
{
	public ContinuousEvolutionSettings Settings;
	[SerializeField] private CarSpawner _carSpawner;
	[SerializeField] private FoodSpawner _foodSpawner;
	private SaveManager _saveManager = new SaveManager();

	public event Action<int> OnSpawnGeneration;

	private List<Car> _currentPopulation = new List<Car>();
	[SerializeField] private List<CarLifeResult> _lifeResults = new List<CarLifeResult>();
	private int _lastCarIndex = 0;
	private bool _paused = false;

	public void InitialSpawn()
	{
		Debug.Log($"Initial spawning cars: {Settings.InitialSpeciesCount}");

		ResetCurrentGeneration();

		for (_lastCarIndex = 0; _lastCarIndex < Settings.InitialSpeciesCount; _lastCarIndex++)
		{
			Car newCar = SpawnCar(_lastCarIndex.ToString());
			CarGenome newGenome = new CarGenome(Settings.NeuralNetworkSettings, Settings.EyeNeuralNetworkSettings);
			newGenome.Generation = 0;
			newCar.SetGenome(newGenome);
		}

		_foodSpawner.SpawnMaxObjects();

		OnSpawnGeneration?.Invoke(0);
	}

	private Car SpawnCar(string name)
	{
		Car newCar = _carSpawner.SpawnObject(name);
		newCar.Index = _lastCarIndex;
		_lastCarIndex++;

		newCar.Food.OnPickupFood += OnCarPickupFood;
		newCar.OnDespawn += OnCarFinishLife;

		_currentPopulation.Add(newCar);

		return newCar;
	}

	private void OnCarPickupFood(Car car, float foodAmount)
	{
		if (car.Food.StoredFood < Settings.ChildStoredFoodRequirement)
			return;

		car.Food.StoredFood -= Settings.ChildStoredFoodRequirement;
		SpawnChild(car, true);
		SpawnChild(car, false);
	}

	private void OnCarFinishLife(Car car)
	{
		AddLifeResult(car);
		_currentPopulation.Remove(car);

		car.Food.OnPickupFood -= OnCarPickupFood;
		car.OnDespawn -= OnCarFinishLife;

		if (_currentPopulation.Count == 0)
		{
			EmergencyRespawn();
		}
	}

	private void AddLifeResult(Car car)
	{
		_lifeResults.Add(new CarLifeResult {Genome = car.GetGenome(), TotalAcquiredFood = car.Food.TotalAcquiredFood, Index = car.Index});
		_lifeResults.Sort((first, second) => second.TotalAcquiredFood.CompareTo(first.TotalAcquiredFood));

		if (_lifeResults.Count > Settings.StoredLifeResultCount)
		{
			int excessElements = _lifeResults.Count - Settings.StoredLifeResultCount;

			_lifeResults = _lifeResults.SkipLast(excessElements).ToList();
		}
	}

	private void EmergencyRespawn()
	{
		Debug.Log($"Emergency respawn {Settings.EmergencyRespawnCount} cars");

		for (int i = 0; i < Settings.EmergencyRespawnCount; i++)
		{
			int randomBestIndex = Random.Range(0, _lifeResults.Count);
			CarGenome randomBest = _lifeResults[randomBestIndex].Genome;

			if (_lifeResults.Count > 1)
				_lifeResults.RemoveAt(randomBestIndex);

			CarGenome genome = new CarGenome(randomBest);
			Car clone = SpawnCar(_lastCarIndex.ToString());

			if (i % 2 == 0)
				genome.IntroduceRandomError();

			clone.SetGenome(genome);
		}

		if (Settings.RespawnAllFood)
			_foodSpawner.SpawnMaxObjects();
	}

	public Car SpawnChild(Car car, bool introduceError)
	{
		CarGenome parentGenome = car.GetGenome();
		CarGenome childGenome = new CarGenome(parentGenome);

		if (introduceError)
			childGenome.IntroduceRandomError();

		Car child = SpawnCar($"{childGenome.Generation} - {_lastCarIndex.ToString()} ({car.Index})");
		_lastCarIndex++;

		child.SetGenome(childGenome);

		child.transform.position = car.transform.position;
		child.transform.rotation = car.transform.rotation;

		return child;
	}

	/*private List<CarLifeResult> FinishCurrentGeneration()
	{
		List<CarLifeResult> bestCars = GetBestResults().ToList();
		CarLifeResult bestCar = bestCars.Max();

		Debug.Log($"Generation: {Generation}. Best fitness: {bestCar.TotalAcquiredFood} ({bestCar.Index}). Average fitness: {GetAverageFitness()}");

		return bestCars.ToList();
	}*/

	/*private IEnumerable<CarLifeResult> GetBestResults()
	{
		return _lifeResults.OrderByDescending(result => result.TotalAcquiredFood).ThenBy(_ => Random.value).Take(Settings.BestSpeciesPerGeneration);
	}*/

	public Car GetCurrentBestCar()
	{
		return _currentPopulation.OrderByDescending(car => car.Food.TotalAcquiredFood).FirstOrDefault();
	}

	/*private float GetAverageFitness()
	{
		return _lifeResults.Average(result => result.TotalAcquiredFood);
	}*/

	public void ForceFinishCurrentGeneration()
	{
		_carSpawner.DespawnObjects();
	}

	private void ResetCurrentGeneration()
	{
		_carSpawner.DespawnObjects();

		_currentPopulation.Clear();
		_lifeResults.Clear();
	}

	public void SaveCurrentPopulation(string fileName)
	{
		List<CarGenome> genomes = _currentPopulation.Select(car => car.GetGenome()).ToList();
		_saveManager.Save(fileName, genomes);
	}

	public void LoadPopulation(string fileName)
	{
		//ResetCurrentGeneration();

		List<CarGenome> genomes = _saveManager.Load(fileName);
		for (int i = 0; i < 1; i++) //TODO: TEMP load only first car
		{
			Car car = SpawnCar(i.ToString());
			car.SetGenome(new CarGenome(genomes[i]));
		}

		_foodSpawner.SpawnMaxObjects();
	}
}