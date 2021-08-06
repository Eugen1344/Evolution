using System;
using LitJsonSrc;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class Car : MonoBehaviour, IJsonSerializable
{
	[JsonProperty("")]
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

	public JsonData Serialize()
	{
		return Brain.Serialize();
	}

	public void Deserialize(JsonData data)
	{
		Brain.Deserialize(data);
	}
}