using System.IO;
using Newtonsoft.Json;

public class SaveManager
{
	public void Save(string fileName, SaveData data)
	{
		using FileStream outFile = new FileStream(fileName, FileMode.Create, FileAccess.Write);
		using StreamWriter writer = new StreamWriter(outFile);

		JsonSerializer serializer = JsonSerializer.CreateDefault();
		serializer.Formatting = Formatting.Indented;
		serializer.Serialize(writer, data);
	}

	public SaveData Load(string fileName)
	{
		using FileStream inFile = new FileStream(fileName, FileMode.Open, FileAccess.Read);
		using StreamReader reader = new StreamReader(inFile);
		JsonSerializer serializer = new JsonSerializer();

		SaveData data = serializer.Deserialize<SaveData>(new JsonTextReader(reader));
		return data;
	}
}