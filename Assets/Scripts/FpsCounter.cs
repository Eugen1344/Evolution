using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
	public int MaxStoredFpsReadings;
	public TextMeshProUGUI FpsText;
	public TextMeshProUGUI AverageFpsText;
	public TextMeshProUGUI MinFpsText;
	
	public float AverageFps { get; private set; }
	public float MinFps { get; private set; }

	private Queue<float> _prevFpsReadings = new Queue<float>();

	void Update()
	{
		float fps = GetFps();

		UpdateAverageFps(fps);

		AverageFps = GetAverageFps();
		MinFps = GetMinFps();

		FpsText.text = $"{fps:00.00}";
		AverageFpsText.text = $"{AverageFps:00.00}";
		MinFpsText.text = $"{MinFps:00.00}";
	}

	private void UpdateAverageFps(float newFps)
	{
		_prevFpsReadings.Enqueue(newFps);

		while (_prevFpsReadings.Count > MaxStoredFpsReadings)
		{
			_prevFpsReadings.Dequeue();
		}
	}

	private float GetAverageFps()
	{
		return _prevFpsReadings.Sum() / _prevFpsReadings.Count;
	}

	private float GetMinFps()
	{
		return _prevFpsReadings.Min();
	}

	private float GetFps()
	{
		return 1.0f / Time.unscaledDeltaTime;
	}
}