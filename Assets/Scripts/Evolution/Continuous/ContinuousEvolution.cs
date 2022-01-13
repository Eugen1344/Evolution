﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ContinuousEvolution : MonoBehaviour
{
	public EvolutionManager _manager;
	public ContinuousEvolutionSettings Settings;
	[SerializeField] private CarSpawner _carSpawner;
	[SerializeField] private List<FoodSpawner> _foodSpawners;

	private List<Car> _currentPopulation = new List<Car>();
	[SerializeField] private List<CarLifeResult> _lifeResults = new List<CarLifeResult>();
	private int _lastCarIndex = 0;

	private void Update()
	{
		List<Task> carUpdateTasks = new List<Task>();

		foreach (Car car in _currentPopulation)
		{
			Task updateTask = car.UpdateCarAsync();
			carUpdateTasks.Add(updateTask);
		}

		Task.WhenAll(carUpdateTasks);
	}

	public void InitialSpawn()
	{
		Debug.Log($"Initial spawning cars: {Settings.InitialSpeciesCount}");

		Reset();

		for (_lastCarIndex = 0; _lastCarIndex < Settings.InitialSpeciesCount; _lastCarIndex++)
		{
			Car newCar = SpawnCar(_lastCarIndex.ToString());
			CarGenome newGenome = new CarGenome(Settings.NeuralNetworkSettings, Settings.EyeNeuralNetworkSettings);
			newGenome.Generation = 0;
			newCar.SetGenome(newGenome);
		}

		TryRespawnAllFood();
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

			CarGenome genome = new CarGenome(randomBest);
			Car clone = SpawnCar(_lastCarIndex.ToString());

			if (i % 2 == 0)
				genome.IntroduceRandomError();

			clone.SetGenome(genome);
		}

		_lifeResults.Clear();
		
		TryRespawnAllFood();
	}

	public void TryRespawnAllFood()
	{
		if (Settings.RespawnAllFood)
		{
			foreach (FoodSpawner spawner in _foodSpawners)
			{
				spawner.SpawnMaxObjects();
			}
		}
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

	private void Reset()
	{
		_carSpawner.DespawnObjects();

		foreach (Car car in _currentPopulation)
		{
			car.Food.OnPickupFood -= OnCarPickupFood;
			car.OnDespawn -= OnCarFinishLife;
		}

		_currentPopulation.Clear();
		_lifeResults.Clear();
	}

	public List<CarGenome> GetPopulation()
	{
		List<CarGenome> genomes = _currentPopulation.Select(car => car.GetGenome()).ToList();
		return genomes;
	}

	public void SetPopulation(List<CarGenome> genomes)
	{
		for (int i = 0; i < 1; i++) //TODO: TEMP load only first car
		{
			Car car = SpawnCar(i.ToString());
			car.SetGenome(new CarGenome(genomes[i]));
		}

		TryRespawnAllFood();
	}
}