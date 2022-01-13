using Crosstales.FB;
using GenericPanels;
using UnityEngine;

public class MenuPanel : UiPanel<MenuPanel>
{
	public EvolutionManager Manager;

	public void ClickSavePopulation()
	{
		string fileName = FileBrowser.Instance.SaveFile("Save population", "", "cars", "json");

		if (string.IsNullOrWhiteSpace(fileName))
			return;

		Manager.Save(fileName);

		Hide();
	}

	public void ClickLoadPopulation()
	{
		string fileName = FileBrowser.Instance.OpenSingleFile("Load population", "", "", "json");

		if (string.IsNullOrWhiteSpace(fileName))
			return;

		Manager.Load(fileName);

		Hide();
	}

	public void ClickFinishGeneration()
	{
		Debug.LogError("FinishGeneration not implemented");

		Hide();
	}

	public void ClickNewPopulation()
	{
		Manager.InitialSpawn();

		Hide();
	}
}