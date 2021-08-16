using System;

public class Food : VisibleObject, ISpawnable<Food>
{
	public float FoodAmount;
	public bool DestroyAfterPickup;

	public event Action<Food> OnDespawn;

	public float Pickup()
	{
		if (DestroyAfterPickup)
			Destroy(gameObject);

		return FoodAmount;
	}

	private void OnDestroy()
	{
		OnDespawn?.Invoke(this);
	}
}