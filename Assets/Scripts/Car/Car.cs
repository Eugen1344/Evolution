using System;
using System.Threading.Tasks;
using UnityEngine;

public class Car : MonoBehaviour, ISpawner<Car>
{
	public CarBrain Brain;
	public CarEye Eye;
	public CarController Movement;
	public CarFood Food;
	public CarFoodPleasure FoodPleasure;
	public CarBody Body;
	public GameObject SelectionIndicator;

	public int Generation = 0;
	public int Index;

	public event Action<Car> OnDespawn;
	public static event Action<Car> OnClick;

	public async Task UpdateCarAsync()
	{
		await Brain.TryMakeDecisionAsync();
	}

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
		return new CarGenome(Brain.Network, Eye.Network) { Generation = Generation, Color = Body.Color };
	}

	public void SetGenome(CarGenome genome)
	{
		Brain.Network = new NeuralNetwork(genome.BrainNetwork);
		Eye.Network = new ConvolutionalNeuralNetwork(genome.EyeNetwork);
		Generation = genome.Generation + 1;

		Body.SetColor(genome.Color);
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