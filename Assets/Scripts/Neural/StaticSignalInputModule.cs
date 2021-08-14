using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StaticSignalInputModule : MonoBehaviour, IInputNeuralModule
{
	public int InputNeuronCount => Signals.Count;
	public List<float> Signals;

	public IEnumerable<float> GetInput()
	{
		return Signals.AsEnumerable();
	}
}