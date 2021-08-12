using UnityEngine;

public class CarFood : MonoBehaviour
{
	public float CurrentFood;
	public float MaxFood;
	public float FoodDecayPerSecond;
	public Car Car;

	public float TotalAcquiredFood { get; private set; } = 0;

	private void Awake()
	{
		CurrentFood = MaxFood;
	}

	private void OnTriggerEnter(Collider obj)
	{
		Food food = obj.GetComponent<Food>();

		if (food)
		{
			float foodAmount = food.Pickup();

			CurrentFood += foodAmount;
			TotalAcquiredFood += foodAmount;
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
		return speed * FoodDecayPerSecond;
	}

	public float GetNormalizedFoodAmount()
	{
		return CurrentFood / MaxFood;
	}
}