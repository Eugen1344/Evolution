﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerationsEvolution : MonoBehaviour
{
	public int InitialSpeciesCount;
	public int SpeciesPerGeneration;
	public int BestSpeciesPerGeneration;
	public NeuralNetworkSettings NeuralNetworkSettings;

	public Car CarPrefab;
	public Transform CarParent;

	public int Generation;

	private List<Car> _currentGeneration = new List<Car>();
	private List<CarLifeResult> _lifeResults = new List<CarLifeResult>();
	private int _aliveCars;

	private void Awake()
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
			Car newCar = SpawnCar();
			newCar.Brain.Network = NeuralNetwork.Random(NeuralNetworkSettings);
		}
	}

	private void SpawnNextGeneration()
	{
		List<CarLifeResult> bestLifeResults = FinishCurrentGeneration();

		Generation++;

		Debug.Log($"Generation: {Generation}. Spawning cars: {SpeciesPerGeneration}");

		ResetCurrentGeneration();

		for (int i = 0; i < SpeciesPerGeneration; i++)
		{
			int prevBestCarIndex = i % BestSpeciesPerGeneration;
			NeuralNetwork prevBestGenome = bestLifeResults[prevBestCarIndex].Genome; //TEMP currently genome is equivalent to network

			NeuralNetwork newGenome = new NeuralNetwork(prevBestGenome);
			newGenome.Settings = NeuralNetworkSettings;
			newGenome.IntroduceRandomError();

			Car newCar = SpawnCar();
			newCar.Brain.Network = newGenome;
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
		_aliveCars = 0;
		_currentGeneration.Clear();
		_lifeResults.Clear();
	}

	private Car SpawnCar()
	{
		Car newCar = Instantiate(CarPrefab, CarParent);
		newCar.OnDestroy += OnCarDestroy;

		_currentGeneration.Add(newCar);
		_aliveCars++;

		return newCar;
	}

	private void OnCarDestroy(Car car)
	{
		_lifeResults.Add(new CarLifeResult { Genome = car.Brain.Network, TotalAcquiredFood = car.Food.TotalAcquiredFood });

		car.OnDestroy -= OnCarDestroy;

		_aliveCars--;

		if (_aliveCars == 0)
		{
			SpawnNextGeneration();
		}
	}
}