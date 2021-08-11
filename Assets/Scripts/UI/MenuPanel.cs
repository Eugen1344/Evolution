using System.Collections.Generic;
using System.IO;
using Crosstales.FB;
using GenericPanels;

public class MenuPanel : UiPanel<MenuPanel>
{
	public GenerationsEvolution Evolution;

	public void ClickSavePopulation()
	{
		string fileName = FileBrowser.Instance.SaveFile("Save population", "", "cars", "json");

		if (string.IsNullOrWhiteSpace(fileName))
			return;

		using FileStream outFile = new FileStream(fileName, FileMode.Create, FileAccess.Write);
		using StreamWriter writer = new StreamWriter(outFile);

		Evolution.SerializeCurrentPopulation(writer);

		Hide();
	}

	public void ClickLoadPopulation()
	{
		string fileName = FileBrowser.Instance.OpenSingleFile("Load population", "", "", "json");

		if (string.IsNullOrWhiteSpace(fileName))
			return;

		using FileStream inFile = new FileStream(fileName, FileMode.Open, FileAccess.Read);

		List<NeuralNetwork> population = Evolution.DeserializePopulation(new StreamReader(inFile));
		Evolution.LoadPopulation(population);

		Hide();
	}
}