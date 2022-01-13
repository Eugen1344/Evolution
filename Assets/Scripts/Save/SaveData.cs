using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class SaveData
{
	[JsonProperty("experiment_name")] public string ExperimentName;
	[JsonProperty("evolutions")] public List<List<CarGenome>> Evolutions;

	public SaveData(string experimentName)
	{
		ExperimentName = experimentName;
		Evolutions = new List<List<CarGenome>>();
	}

	public void SaveAddEvolution(ContinuousEvolution evolution)
	{
		List<CarGenome> evolutionPopulation = evolution.GetPopulation();
		Evolutions.Add(evolutionPopulation);
	}
}