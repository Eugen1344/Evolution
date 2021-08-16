using System;

public struct CarLifeResult : IComparable<CarLifeResult>
{
	public NeuralNetwork Genome;
	public float TotalAcquiredFood;
	public int Index;
	
	public int CompareTo(CarLifeResult other)
	{
		return TotalAcquiredFood.CompareTo(other.TotalAcquiredFood);
	}
}