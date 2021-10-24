using System;
using UnityEngine;

public class Car : MonoBehaviour, ISpawnable<Car>
{
	public CarBrain Brain;
	public CarEye LetEye;
	public CarEye RightEye;
	public CarController Movement;
	public CarFood Food;
	public GameObject SelectionIndicator;

	public int Index;

	public event Action<Car> OnDespawn;
	public static event Action<Car> OnClick;

	public void Destroy()
	{
		Destroy(gameObject);
	}

	private void OnDestroy()
	{
		OnDespawn?.Invoke(this);
	}

	private void OnMouseUpAsButton()
	{
		OnClick?.Invoke(this);
	}

	public CarGenome GetGenome()
	{
		return new CarGenome(Brain.Network, LetEye.Network.Settings);
	}

	public void SetGenome(CarGenome genome)
	{
		Brain.Network = new NeuralNetwork(genome.BrainNetwork);
		LetEye.Network = new ConvolutionalNeuralNetwork(genome.EyeNetwork);
		RightEye.Network = new ConvolutionalNeuralNetwork(genome.EyeNetwork);
	}

	public void Select()
	{
		SelectionIndicator.SetActive(true);
	}

	public void Unselect()
	{
		SelectionIndicator.SetActive(false);
	}
}