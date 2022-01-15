using System.Collections.Generic;
using UnityEngine;

public class TimeScaleController : MonoBehaviour
{
	[SerializeField] private List<float> _timeScaleButtonValues;
	[SerializeField] private KeyCode _pauseKey;
	[SerializeField] private KeyCode _adaptiveTimeScaleKey;
	[SerializeField] private AdaptiveFps _adaptiveFps;

	private float _prevTimeScale;

	private void Start()
	{
		InitializeCommandBarFields();
	}

	private void Update()
	{
		if (Time.timeScale != 0)
		{
			_prevTimeScale = Time.timeScale;
		}
		
		if (Input.GetKeyUp(_pauseKey))
		{
			if (Time.timeScale == 0)
			{
				Time.timeScale = _prevTimeScale;
			}
			else
			{
				Time.timeScale = 0;
			}
		}
		else if (Input.GetKeyUp(KeyCode.Alpha1))
		{
			Time.timeScale = _timeScaleButtonValues[0];
		}
		else if (Input.GetKeyUp(KeyCode.Alpha2))
		{
			Time.timeScale = _timeScaleButtonValues[1];
		}
		else if (Input.GetKeyUp(KeyCode.Alpha3))
		{
			Time.timeScale = _timeScaleButtonValues[2];
		}
		else if (Input.GetKeyUp(KeyCode.Alpha4))
		{
			Time.timeScale = _timeScaleButtonValues[3];
		}
		else if (Input.GetKeyUp(KeyCode.Alpha5))
		{
			Time.timeScale = _timeScaleButtonValues[4];
		}
		else if (Input.GetKeyUp(_adaptiveTimeScaleKey))
		{
			_adaptiveFps.IsEnabled = !_adaptiveFps.IsEnabled;
		}
	}

	private void InitializeCommandBarFields()
	{
		Commands.AddFloatField("Time scale", () => Time.timeScale, time => Time.timeScale = time);
	}
}