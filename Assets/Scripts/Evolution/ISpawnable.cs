using System;

public interface ISpawnable<T>
{
	public event Action<T> OnDespawn;
}