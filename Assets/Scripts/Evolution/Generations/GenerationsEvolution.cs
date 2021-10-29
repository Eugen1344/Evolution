using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerationsEvolution : MonoBehaviour
{
	public GenerationsEvolutionSettings Settings;
	public CarSpawner CarSpawner;
	public FoodSpawner FoodSpawner;

	public int Generation;

	public event Action<int> OnSpawnGeneration;

	private List<Car> _currentGeneration = new List<Car>();
	private List<CarLifeResult> _lifeResults = new List<CarLifeResult>();

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
		Generation = 0;

		Debug.Log($"Initial spawning cars: {Settings.InitialSpeciesCount}");

		ResetCurrentGeneration();

		for (int i = 0; i < Settings.InitialSpeciesCount; i++)
		{
			Car newCar = SpawnCar(i.ToString(), i);
			CarGenome newGenome = new CarGenome(Settings.NeuralNetworkSettings, Settings.EyeNeuralNetworkSettings);
			newCar.SetGenome(newGenome);
		}

		FoodSpawner.SpawnMaxObjects();

		OnSpawnGeneration?.Invoke(0);
	}

	private void SpawnNextGeneration()
	{
		List<CarLifeResult> bestLifeResults = FinishCurrentGeneration();

		Generation++;

		//Debug.Log($"Generation: {Generation}. Spawning cars: {SpeciesPerGeneration}");

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

		if (Settings.RespawnFoodEachGeneration)
			FoodSpawner.SpawnMaxObjects();

		OnSpawnGeneration?.Invoke(Generation);
	}

	private List<CarLifeResult> FinishCurrentGeneration()
	{
		List<CarLifeResult> bestCars = GetBestResults().ToList();
		CarLifeResult bestCar = bestCars.Max();

		Debug.Log($"Generation: {Generation}. Best fitness: {bestCar.TotalAcquiredFood} ({bestCar.Index}). Average fitness: {GetAverageFitness()}");

		return bestCars.ToList();
	}

	private IEnumerable<CarLifeResult> GetBestResults()
	{
		return _lifeResults.OrderByDescending(result => result.TotalAcquiredFood).ThenBy(_ => Random.value).Take(Settings.BestSpeciesPerGeneration);
	}

	public Car GetCurrentBestCar()
	{
		return _currentGeneration.OrderByDescending(car => car.Food.TotalAcquiredFood).FirstOrDefault();
	}

	private float GetAverageFitness()
	{
		return _lifeResults.Average(result => result.TotalAcquiredFood);
	}

	public void ForceFinishCurrentGeneration()
	{
		CarSpawner.DespawnObjects();
	}

	private void ResetCurrentGeneration()
	{
		CarSpawner.DespawnObjects();

		_currentGeneration.Clear();
		_lifeResults.Clear();
	}

	private Car SpawnCar(string name, int index)
	{
		Car newCar = CarSpawner.SpawnObject(name);
		newCar.Index = index;

		newCar.OnDespawn += OnCarFinishLife;

		_currentGeneration.Add(newCar);

		return newCar;
	}

	private void OnCarFinishLife(Car car)
	{
		_lifeResults.Add(new CarLifeResult { Genome = car.GetGenome(), TotalAcquiredFood = car.Food.TotalAcquiredFood, Index = car.Index });

		car.OnDespawn -= OnCarFinishLife;

		if (CarSpawner.AliveObjects == 0)
		{
			SpawnNextGeneration();
		}
	}

	public void SerializeCurrentPopulation(StreamWriter writer)
	{
		List<CarGenome> genomes = _currentGeneration.Select(car => car.GetGenome()).ToList();

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

			Car car = SpawnCar(i.ToString(), i);
			car.SetGenome(genomes[i]);
		}
	}

	public List<CarGenome> DeserializePopulation(StreamReader reader)
	{
		JsonSerializer serializer = new JsonSerializer();

		return serializer.Deserialize<List<CarGenome>>(new JsonTextReader(reader));
	}
}