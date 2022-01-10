using Crosstales.FB;
using GenericPanels;

public class MenuPanel : UiPanel<MenuPanel>
{
	public ContinuousEvolution Evolution;

	public void ClickSavePopulation()
	{
		string fileName = FileBrowser.Instance.SaveFile("Save population", "", "cars", "json");

		if (string.IsNullOrWhiteSpace(fileName))
			return;

		Evolution.SaveCurrentPopulation(fileName);

		Hide();
	}

	public void ClickLoadPopulation()
	{
		string fileName = FileBrowser.Instance.OpenSingleFile("Load population", "", "", "json");

		if (string.IsNullOrWhiteSpace(fileName))
			return;

		Evolution.LoadPopulation(fileName);

		Hide();
	}

	public void ClickFinishGeneration()
	{
		Evolution.ForceFinishCurrentGeneration();

		Hide();
	}

	public void ClickNewPopulation()
	{
		Evolution.InitialSpawn();

		Hide();
	}
}