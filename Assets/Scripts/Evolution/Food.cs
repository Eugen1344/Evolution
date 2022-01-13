using System;
using UnityEngine;

public class Food : VisibleObject, ISpawner<Food>
{
	public GameObject RootObject;
	public float FoodAmount;
	public bool DestroyAfterPickup;

	public FoodType Type;

	public event Action<Food> OnDespawn;

	public float Pickup()
	{
		if (DestroyAfterPickup)
			Destroy(RootObject);

		return FoodAmount;
	}

	private void OnDestroy()
	{
		OnDespawn?.Invoke(this);
	}
}