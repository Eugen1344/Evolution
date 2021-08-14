public interface IOutputNeuralModule : INeuralModule
{
	public int OutputNeuronCount { get; }

	public void SetOutput(float[] output, int startingIndex);
}