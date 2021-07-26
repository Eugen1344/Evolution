using UnityEngine;

public class CarFood : MonoBehaviour
{
	public float CurrentFood;
	public float MaxFood;

	private void Awake()
	{
		CurrentFood = MaxFood;
	}

	private void Update()
	{

	}

	public float GetNormalizedFoodAmount()
	{
		return CurrentFood / MaxFood;
	}
}