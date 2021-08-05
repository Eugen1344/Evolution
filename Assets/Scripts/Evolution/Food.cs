using UnityEngine;

public class Food : MonoBehaviour
{
	public float FoodAmount;
	public bool DestroyAfterPickup;

	public float Pickup()
	{
		if (DestroyAfterPickup)
			Destroy(gameObject);

		return FoodAmount;
	}
}