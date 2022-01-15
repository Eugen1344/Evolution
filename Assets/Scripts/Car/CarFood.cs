using System;
using System.Collections.Generic;
using UnityEngine;

public class CarFood : MonoBehaviour, IInputNeuralModule
{
	public FoodType ConsumedFoodType;
	public float MaxFood;
	[Range(0, 1)] public float FoodStoreRatio;
	public float FoodDecayPerSecond;
	public Car Car;

	[SerializeField] private float _currentFood;

	public float CurrentFood
	{
		get => _currentFood;
		set => _currentFood = value > 0 ? value : 0;
	}

	[SerializeField] private float _storedFood;

	public float StoredFood
	{
		get => _storedFood;
		set => _storedFood = value > 0 ? value : 0;
	}

	public float TotalAcquiredFood { get; private set; } = 0;

	public event Action<Car, float> OnPickupFood;

	private void Awake()
	{
		CurrentFood = MaxFood;
	}

	private void OnTriggerEnter(Collider obj)
	{
		Food food = obj.GetComponent<Food>();

		if (food)
		{
			if (ConsumedFoodType == food.Type)
				PickUpFood(food);
		}
	}

	private void PickUpFood(Food food)
	{
		float foodAmount = food.Pickup();

		AddFood(foodAmount);

		OnPickupFood?.Invoke(Car, foodAmount);
	}

	private void Update()
	{
		if (CurrentFood <= 0 && StoredFood <= 0)
		{
			Car.Destroy();

			return;
		}

		float foodReward = GetFoodReward(Car.Movement.GetTotalNormalizedSpeed());
		float foodDelta = (foodReward - FoodDecayPerSecond) * Time.deltaTime;
		AddFood(foodDelta);

		if (CurrentFood < 0)
			CurrentFood = 0;

		if (StoredFood < 0)
			StoredFood = 0;

		TotalAcquiredFood += foodReward * Time.deltaTime;
	}

	public void AddFood(float foodDelta)
	{
		if (foodDelta > 0)
		{
			float storedFood = foodDelta * FoodStoreRatio;
			float mainFood = foodDelta - storedFood;

			StoredFood += storedFood;
			CurrentFood += mainFood;
			TotalAcquiredFood += foodDelta;
		}
		else
		{
			if (CurrentFood == 0)
				StoredFood += foodDelta;
			else
				CurrentFood += foodDelta;
		}
	}

	public float GetFoodReward(float speed)
	{
		return 0;
		//return -Mathf.Abs(speed * FoodDecayPerSecond);
	}

	public float GetNormalizedFoodAmount()
	{
		return CurrentFood / MaxFood;
	}

	public int InputNeuronCount => 1;

	public IEnumerable<float> GetInput()
	{
		yield return GetNormalizedFoodAmount();
	}
}