using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class GenerationsEvolution : MonoBehaviour
{
	public int InitialSpeciesCount;
	public int SpeciesPerGeneration;
	public int BestSpeciesPerGeneration;
	public int BestSpeciesPerGenerationClones;
	public NeuralNetworkSettings NeuralNetworkSettings;

	public Car CarPrefab;
	public Transform CarParent;

	public int Generation;

	private List<Car> _currentGeneration = new List<Car>();
	private List<CarLifeResult> _lifeResults = new List<CarLifeResult>();
	private List<Car> _aliveCars = new List<Car>();

	private void Start()
	{
		InitialSpawn();
	}

	private void InitialSpawn()
	{
		Generation = 0;

		Debug.Log($"Initial spawning cars: {InitialSpeciesCount}");

		ResetCurrentGeneration();

		for (int i = 0; i < InitialSpeciesCount; i++)
		{
			Car newCar = SpawnCar(i.ToString());
			newCar.SetGenome(NeuralNetwork.Random(NeuralNetworkSettings));
		}
	}

	private void SpawnNextGeneration()
	{
		List<CarLifeResult> bestLifeResults = FinishCurrentGeneration();

		Generation++;

		//Debug.Log($"Generation: {Generation}. Spawning cars: {SpeciesPerGeneration}");

		ResetCurrentGeneration();

		int possibleBestSpeciesPerGeneration = Mathf.Min(bestLifeResults.Count, BestSpeciesPerGeneration);
		int clonesToSpawn = BestSpeciesPerGenerationClones * possibleBestSpeciesPerGeneration;

		for (int i = 0; i < SpeciesPerGeneration + clonesToSpawn; i++)
		{
			int prevBestCarIndex = i % possibleBestSpeciesPerGeneration;
			NeuralNetwork prevBestGenome = bestLifeResults[prevBestCarIndex].Genome; //TEMP currently genome is just network

			NeuralNetwork newGenome = new NeuralNetwork(prevBestGenome) { Settings = NeuralNetworkSettings };

			string name = i.ToString();

			if (clonesToSpawn > 0)
			{
				clonesToSpawn--;

				name += $" - Clone ({prevBestCarIndex})";
			}
			else
			{
				float error = newGenome.IntroduceRandomError();

				name += $" - Error ({error})";
			}

			Car newCar = SpawnCar(name);
			newCar.SetGenome(newGenome);
		}
	}

	private List<CarLifeResult> FinishCurrentGeneration()
	{
		IEnumerable<CarLifeResult> bestCars = GetBestResults();

		Debug.Log($"Generation: {Generation}. Average fitness: {GetAverageFitness()}");

		return bestCars.ToList();
	}

	private IEnumerable<CarLifeResult> GetBestResults()
	{
		return _lifeResults.OrderByDescending(result => result.TotalAcquiredFood).Take(BestSpeciesPerGeneration);
	}

	private float GetAverageFitness()
	{
		return _lifeResults.Average(result => result.TotalAcquiredFood);
	}

	private void ResetCurrentGeneration()
	{
		foreach (Car aliveCar in _aliveCars)
		{
			aliveCar.OnFinishLife -= OnCarFinishLife;
			aliveCar.Destroy();
		}

		_aliveCars.Clear();
		_currentGeneration.Clear();
		_lifeResults.Clear();
	}

	private Car SpawnCar(string name)
	{
		Car newCar = Instantiate(CarPrefab, CarParent);
		newCar.gameObject.name = name;

		newCar.OnFinishLife += OnCarFinishLife;

		_currentGeneration.Add(newCar);
		_aliveCars.Add(newCar);

		return newCar;
	}

	private void OnCarFinishLife(Car car)
	{
		_lifeResults.Add(new CarLifeResult { Genome = car.Brain.Network, TotalAcquiredFood = car.Food.TotalAcquiredFood });

		car.OnFinishLife -= OnCarFinishLife;

		_aliveCars.Remove(car);

		if (_aliveCars.Count == 0)
		{
			SpawnNextGeneration();
		}
	}

	public void SerializeCurrentPopulation(StreamWriter writer)
	{
		List<NeuralNetwork> carData = _currentGeneration.Select(car => car.GetGenome()).ToList();

		JsonSerializer serializer = new JsonSerializer { Formatting = Formatting.Indented };
		serializer.Serialize(writer, carData);
	}

	public void LoadPopulation(List<NeuralNetwork> population)
	{
		ResetCurrentGeneration();

		for (int i = 0; i < population.Count; i++)
		{
			Car car = SpawnCar(i.ToString());
			car.SetGenome(population[i]);
		}
	}

	public List<NeuralNetwork> DeserializePopulation(StreamReader reader)
	{
		JsonSerializer serializer = new JsonSerializer();

		return serializer.Deserialize<List<NeuralNetwork>>(new JsonTextReader(reader));
	}
}