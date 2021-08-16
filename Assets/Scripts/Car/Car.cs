using System;
using UnityEngine;

public class Car : MonoBehaviour, ISpawnable<Car>
{
	public CarBrain Brain;
	public CarEye Eye;
	public CarController Movement;
	public CarFood Food;

	public int Index;

	public event Action<Car> OnDespawn;

	public void Destroy()
	{
		Destroy(gameObject);
	}

	private void OnDestroy()
	{
		OnDespawn?.Invoke(this);
	}

	public NeuralNetwork GetGenome() //TODO temp - make new class for CarGenome
	{
		return Brain.Network;
	}

	public void SetGenome(NeuralNetwork data)
	{
		Brain.Network = data;
	}
}