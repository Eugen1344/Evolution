using System.Collections;
using System.IO;
using UnityEngine;

public class AutoSaveManager : MonoBehaviour
{
	public float IntervalSeconds;
	public string FileName;
	[SerializeField] private ContinuousEvolution Evolution;

	private string SaveDirectory => Application.persistentDataPath;
	private string SavePath => Path.Combine(SaveDirectory, FileName);

	private void Awake()
	{
		StartCoroutine(AutoSaveRoutine());
	}

	private IEnumerator AutoSaveRoutine()
	{
		while (true)
		{
			yield return new WaitForSecondsRealtime(IntervalSeconds);

			Evolution.SaveCurrentPopulation(SavePath);
		}
	}
}