using System;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class Car : MonoBehaviour
{
	[JsonProperty("brain")]
	public CarBrain Brain;
	public CarController Movement;
	public CarFood Food;

	public event Action<Car> OnFinishLife;

	public void Destroy()
	{
		Destroy(gameObject);
	}

	private void OnDestroy()
	{
		OnFinishLife?.Invoke(this);
	}
}