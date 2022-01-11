﻿using System.Collections.Generic;
using UnityEngine;

public class TimeScaleController : MonoBehaviour
{
	[SerializeField] private List<float> _timeScaleButtonValues;
	[SerializeField] private KeyCode _pauseKey;
	[SerializeField] private KeyCode _adaptiveTimeScaleKey;

	private float _prevTimeScale;
	private bool _paused;

	private void Start()
	{
		InitializeCommandBarFields();
	}

	private void Update()
	{
		if (Input.GetKeyUp(_pauseKey))
		{
			if (_paused)
			{
				Time.timeScale = _prevTimeScale;
				
				_paused = false;
			}
			else
			{
				_prevTimeScale = Time.timeScale;

				_paused = true;
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
		else if (Input.GetKeyUp(_adaptiveTimeScaleKey))
		{
			Debug.Log("ADAPTIVE FPS BRRRR");
		}
	}

	private void InitializeCommandBarFields()
	{
		Commands.AddFloatField("Time scale", () => Time.timeScale, time => Time.timeScale = time);
	}
}