using UnityEngine;

public class TestAllocateMemory : MonoBehaviour
{
	public int HUGE_MEMORY_CHUNK_SIZE;

	private byte[] HUGE_MEMORY_CHUNK;

	private void Start()
	{
		HUGE_MEMORY_CHUNK = new byte[HUGE_MEMORY_CHUNK_SIZE];

		Debug.Log(HUGE_MEMORY_CHUNK[100]);
	}
}