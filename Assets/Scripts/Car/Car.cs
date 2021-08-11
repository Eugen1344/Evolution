using System;
using UnityEngine;

public class Car : MonoBehaviour
{
	public CarBrain Brain;
	public CarController Movement;
	public CarFood Food;

	public event Action<Car> OnFinishLife;

	public void Destroy()
	{
		Destroy(gameObject);
	}

	private void OnDestroy()
	{
		OnFinishLife?.Invoke(this);
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