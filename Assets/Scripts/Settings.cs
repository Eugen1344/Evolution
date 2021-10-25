using System;
using Newtonsoft.Json;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class Settings
{
	[JsonProperty("evolution")]
	public GenerationsEvolutionSettings EvolutionSettings;
}