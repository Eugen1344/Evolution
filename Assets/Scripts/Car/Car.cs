using System;
using UnityEngine;

public class Car : MonoBehaviour
{
	public CarBrain Brain;
	public CarController Movement;
	public CarFood Food;

	public event Action<Car> OnDestroy;

	public void Destroy()
	{
		OnDestroy?.Invoke(this);

		Destroy(gameObject);
	}
}