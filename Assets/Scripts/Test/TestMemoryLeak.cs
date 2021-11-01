using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMemoryLeak : MonoBehaviour
{
	public GameObject CarPrefab;

	private List<GameObject> _cars = new List<GameObject>();

	private void Start()
	{
		StartCoroutine(CloneAndDestroyCars());
	}

	private IEnumerator CloneAndDestroyCars()
	{
		while (true)
		{
			for (int i = 0; i < 100; i++)
			{
				GameObject car = Instantiate(CarPrefab);
				_cars.Add(car);
			}

			yield return new WaitForSeconds(1);

			foreach (GameObject car in _cars)
			{
				Destroy(car);
			}

			_cars.Clear();

			yield return new WaitForSeconds(1);
		}
	}
}