using System.Collections;
using System.IO;
using UnityEngine;

public class AutoSaveManager : MonoBehaviour
{
	public float IntervalSeconds;
	public string FileName;
	public string SaveFileExtension;
	[SerializeField] private EvolutionManager _manager;

	private string SaveDirectory => Application.persistentDataPath;
	private string SavePath => Path.Combine(SaveDirectory, $"{FileName}_{_manager.ExperimentName}", SaveFileExtension);

	private void Awake()
	{
		StartCoroutine(AutoSaveRoutine());
	}

	private IEnumerator AutoSaveRoutine()
	{
		while (true)
		{
			yield return new WaitForSecondsRealtime(IntervalSeconds);

			_manager.Save(SavePath);
		}
	}
}