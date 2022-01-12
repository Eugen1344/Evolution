using System.Collections.Generic;
using UnityEngine;

public class EvolutionManager : MonoBehaviour
{
	public List<ContinuousEvolution> Evolutions;
	public List<FoodSpawner> FoodSpawners;
	public bool InitialSpawnAllFood;

	public void InitialSpawn()
	{
		foreach (ContinuousEvolution evolution in Evolutions)
		{
			evolution.InitialSpawn();
		}
		
		TryRespawnAllFood();
	}

	public void TryRespawnAllFood()
	{
		if (InitialSpawnAllFood)
		{
			foreach (FoodSpawner spawner in FoodSpawners)
			{
				spawner.SpawnMaxObjects();
			}
		}
	}
}