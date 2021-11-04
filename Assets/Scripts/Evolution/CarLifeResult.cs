using System;

[Serializable]
public struct CarLifeResult : IComparable<CarLifeResult>
{
	public CarGenome Genome;
	public float TotalAcquiredFood;
	public int Index;
	
	public int CompareTo(CarLifeResult other)
	{
		return TotalAcquiredFood.CompareTo(other.TotalAcquiredFood);
	}
}