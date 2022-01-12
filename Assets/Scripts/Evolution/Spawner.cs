using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Spawner : MonoBehaviour, IInputNeuralModule
{
	public int InputNeuronCount => 1;

	public abstract IEnumerable<float> GetInput();
}

public class Spawner<T> : Spawner
	where T : MonoBehaviour, ISpawner<T>
{
	[ValidateInput("@SpawnRadiusMin >= 0")]
	public float SpawnRadiusMin;
	[ValidateInput("@SpawnRadiusMax >= 0 && SpawnRadiusMax >= SpawnRadiusMin")]
	public float SpawnRadiusMax;
	public int ObjectsCountMax;
	public bool AutoSpawn;
	public float SpawnIntervalSeconds;
	public T Prefab;

	public int AliveObjects => _spawnedObjects.Count;

	public event Action<T> OnSpawn;

	private List<T> _spawnedObjects = new List<T>();
	private float _prevSpawnTime = 0;

	private void Update()
	{
		if (!AutoSpawn || _spawnedObjects.Count >= ObjectsCountMax)
		{
			return;
		}

		if (Time.time - _prevSpawnTime >= SpawnIntervalSeconds)
		{
			SpawnObject(Prefab.name);

			_prevSpawnTime = Time.time;
		}
	}

	public void SpawnMaxObjects()
	{
		DespawnObjects();

		for (int i = 0; i < ObjectsCountMax; i++)
		{
			SpawnObject(i.ToString());
		}
	}

	public T SpawnObject(string name)
	{
		T obj = InstantiateObject(name);
		obj.transform.localPosition = RandomAllowedLocalPosition();
		obj.OnDespawn += OnDespawnObject;

		OnSpawn?.Invoke(obj);

		return obj;
	}

	private void OnDespawnObject(T obj)
	{
		_spawnedObjects.Remove(obj);
		obj.OnDespawn -= OnDespawnObject;
	}

	private Vector3 RandomAllowedLocalPosition()
	{
		float distance = Random.Range(SpawnRadiusMin, SpawnRadiusMax);
		float angle = Random.Range(0.0f, 360.0f);

		Vector3 result = new Vector3(distance * Mathf.Sin(angle), 0, distance * Mathf.Cos(angle));

		return result;
	}

	private T InstantiateObject(string name)
	{
		T newObj = Instantiate(Prefab, transform);
		newObj.gameObject.name = name;

		_spawnedObjects.Add(newObj);

		return newObj;
	}

	public void DespawnObjects()
	{
		foreach (T obj in _spawnedObjects)
		{
			if (obj)
				Destroy(obj.gameObject);
		}

		_spawnedObjects.Clear();
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;

		Gizmos.DrawWireSphere(transform.position, SpawnRadiusMin);

		Gizmos.color = Color.green;

		Gizmos.DrawWireSphere(transform.position, SpawnRadiusMax);
	}

	public override IEnumerable<float> GetInput()
	{
		yield return AliveObjects;
	}
}
