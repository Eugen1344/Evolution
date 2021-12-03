using UnityEngine;

public class SingletonTest : MonoBehaviour
{
	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
		Debug.Log(MenuButtonsPanel.Instance);
	}
}