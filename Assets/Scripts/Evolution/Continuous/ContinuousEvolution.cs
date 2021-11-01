using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class ContinuousEvolution : MonoBehaviour
{
	public ContinuousEvolutionSettings Settings;
	public CarSpawner CarSpawner;
	public FoodSpawner FoodSpawner;

	public event Action<int> OnSpawnGeneration;

	private List<Car> _current = new List<Car>();
	//private List<CarLifeResult> _lifeResults = new List<CarLifeResult>();
	private int _lastCarIndex = 0;

	private void Start()
	{
		Settings.InitializeCommandBarFields();
	}

	private void Update()
	{
		Time.timeScale = Settings.TimeScale; //TODO bad solution
	}

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

		FoodSpawner.SpawnMaxObjects();

		OnSpawnGeneration?.Invoke(0);
	}

	private Car SpawnCar(string name)
	{
		Car newCar = CarSpawner.SpawnObject(name);
		newCar.Index = _lastCarIndex;
		_lastCarIndex++;

		newCar.Food.OnPickupFood += OnCarPickupFood;
		newCar.OnDespawn += OnCarFinishLife;

		_current.Add(newCar);

		return newCar;
	}

	private void OnCarPickupFood(Car car, float foodAmount)
	{
		if (car.Food.StoredFood < Settings.ChildStoredFoodRequirement)
			return;

		car.Food.StoredFood -= Settings.ChildStoredFoodRequirement;
		SpawnChild(car);
	}

	private void OnCarFinishLife(Car car)
	{
		//_lifeResults.Add(new CarLifeResult { Genome = car.GetGenome(), TotalAcquiredFood = car.Food.TotalAcquiredFood, Index = car.Index });
		_current.Remove(car);

		car.Food.OnPickupFood -= OnCarPickupFood;
		car.OnDespawn -= OnCarFinishLife;

		if (_current.Count == 0)
		{
			EmergencyRespawn(car);
		}
	}

	private void EmergencyRespawn(Car car)
	{
		CarGenome genome = car.GetGenome();
		Car clone = SpawnCar(_lastCarIndex.ToString());
		clone.SetGenome(genome);

		for (int i = 0; i < Settings.EmergencyRespawnCount; i++)
		{
			genome = new CarGenome(genome);
			genome.IntroduceRandomError();

			clone = SpawnCar(_lastCarIndex.ToString());
			clone.SetGenome(genome);
		}

		if (Settings.RespawnAllFood)
			FoodSpawner.SpawnMaxObjects();
	}

	public Car SpawnChild(Car car)
	{
		CarGenome parentGenome = car.GetGenome();
		CarGenome childGenome = new CarGenome(parentGenome);
		childGenome.IntroduceRandomError();

		Car child = SpawnCar($"{childGenome.Generation} - {_lastCarIndex.ToString()} ({car.Index})");
		_lastCarIndex++;

		child.SetGenome(childGenome);

		child.transform.position = car.transform.position;
		child.transform.rotation = car.transform.rotation;

		return child;
	}

	/*private void SpawnNextGeneration()
	{
		List<CarLifeResult> bestLifeResults = FinishCurrentGeneration();

		ResetCurrentGeneration();

		int possibleBestSpeciesPerGeneration = Mathf.Min(bestLifeResults.Count, Settings.BestSpeciesPerGeneration);
		int clonesToSpawn = Settings.BestSpeciesPerGenerationClones * possibleBestSpeciesPerGeneration;

		for (int i = 0; i < Settings.SpeciesPerGeneration + clonesToSpawn; i++)
		{
			int prevBestCarIndex = i % possibleBestSpeciesPerGeneration;
			CarLifeResult lifeResult = bestLifeResults[prevBestCarIndex];
			CarGenome prevBestGenome = lifeResult.Genome;

			CarGenome newGenome = new CarGenome(prevBestGenome);

			string name = i.ToString();

			if (clonesToSpawn > 0)
			{
				clonesToSpawn--;

				name += $" - Clone ({lifeResult.Index})";
			}
			else
			{
				float error = newGenome.IntroduceRandomError();

				name += $" - Error ({lifeResult.Index}) [{error}]";
			}

			Car newCar = SpawnCar(name, i);
			newCar.SetGenome(newGenome);
		}

		if (Settings.RespawnAllFood)
			FoodSpawner.SpawnMaxObjects();

		OnSpawnGeneration?.Invoke(Generation);
	}*/

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
		return _current.OrderByDescending(car => car.Food.TotalAcquiredFood).FirstOrDefault();
	}

	/*private float GetAverageFitness()
	{
		return _lifeResults.Average(result => result.TotalAcquiredFood);
	}*/

	public void ForceFinishCurrentGeneration()
	{
		CarSpawner.DespawnObjects();
	}

	private void ResetCurrentGeneration()
	{
		CarSpawner.DespawnObjects();

		_current.Clear();
		//_lifeResults.Clear();
	}

	public void SerializeCurrentPopulation(StreamWriter writer)
	{
		List<CarGenome> genomes = _current.Select(car => car.GetGenome()).ToList();

		JsonSerializer serializer = new JsonSerializer { Formatting = Formatting.Indented };
		serializer.Serialize(writer, genomes);
	}

	public void LoadPopulation(List<CarGenome> genomes)
	{
		ResetCurrentGeneration();

		for (int i = 0; i < genomes.Count; i++)
		{
			genomes[i].LeftEyeNetwork = ConvolutionalNeuralNetwork.Initial(Settings.EyeNeuralNetworkSettings);
			genomes[i].RightEyeNetwork = ConvolutionalNeuralNetwork.Initial(Settings.EyeNeuralNetworkSettings);

			Car car = SpawnCar(i.ToString());
			car.SetGenome(genomes[i]);
		}

		if (Settings.RespawnAllFood)
			FoodSpawner.SpawnMaxObjects();
	}

	public List<CarGenome> DeserializePopulation(StreamReader reader)
	{
		JsonSerializer serializer = new JsonSerializer();

		return serializer.Deserialize<List<CarGenome>>(new JsonTextReader(reader));
	}
}