using System;

public interface ISpawner
{
	public int ObjectCount { get; }
}

public interface ISpawner<T>
{
	public event Action<T> OnDespawn;
}