using TMPro;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
	public TextMeshProUGUI Text;

	void Update()
	{
		Text.text = GetFps().ToString("00.00");
	}

	private float GetFps()
	{
		return 1.0f / Time.unscaledDeltaTime;
	}
}
