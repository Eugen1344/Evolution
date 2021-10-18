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
		return new CarGenome(Brain.Network, Eye.Network);
	}

	public void SetGenome(CarGenome genome)
	{
		Brain.Network = genome.BrainNetwork;
		Eye.Network = genome.EyeNetwork;
	}
}