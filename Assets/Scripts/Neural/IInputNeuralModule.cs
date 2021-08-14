using System.Collections.Generic;

public interface IInputNeuralModule : INeuralModule
{
	public int InputNeuronCount { get; }

	public IEnumerable<float> GetInput();
}