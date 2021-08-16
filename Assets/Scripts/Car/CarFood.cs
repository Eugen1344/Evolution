using System.Collections.Generic;
using UnityEngine;

public class CarFood : MonoBehaviour, IInputNeuralModule
{
	public float CurrentFood;
	public float MaxFood;
	public float FoodDecayPerSecond;
	public Car Car;

	public float TotalAcquiredFood { get; private set; } = 0;

	private List<Food> _acquiredFood = new List<Food>();

	private void Awake()
	{
		CurrentFood = MaxFood;
	}

	private void OnTriggerEnter(Collider obj)
	{
		Food food = obj.GetComponent<Food>();

		if (food && !_acquiredFood.Contains(food))
		{
			float foodAmount = food.Pickup();

			CurrentFood += foodAmount;
			TotalAcquiredFood += foodAmount;

			_acquiredFood.Add(food);
			Car.Eye.DisableSeeingObject(food);
		}
	}

	private void Update()
	{
		if (CurrentFood == 0)
		{
			Car.Destroy();

			return;
		}

		float foodReward = GetFoodReward(Car.Movement.GetTotalNormalizedSpeed());
		float foodDelta = (foodReward - FoodDecayPerSecond) * Time.deltaTime;
		CurrentFood += foodDelta;

		if (CurrentFood < 0)
			CurrentFood = 0;

		TotalAcquiredFood += foodReward * Time.deltaTime;
	}

	public float GetFoodReward(float speed)
	{
		return 0;
		//return speed * FoodDecayPerSecond;
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