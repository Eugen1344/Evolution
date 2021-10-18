using System;
using UnityEngine;

public class CarInput : MonoBehaviour
{
	public CarController Controller;
	public CarEye Eye;

	private void Awake()
	{
		Eye.Network = ConvolutionalNeuralNetwork.Initial(Eye.Network.Settings);
	}

	private void Update()
	{
		Eye.UpdateViewData();

		if (Input.GetKey(KeyCode.W))
		{
			SetLeftSpeed(1);
			SetLeftBrakes(0);
		}
		else if (Input.GetKey(KeyCode.S))
		{
			SetLeftSpeed(-1);
			SetLeftBrakes(0);
		}
		else
		{
			SetLeftSpeed(0);
			SetLeftBrakes(0);
		}

		if (Input.GetKey(KeyCode.A))
		{
			SetLeftSteering(-1);
		}
		else if (Input.GetKey(KeyCode.D))
		{
			SetLeftSteering(1);
		}
		else
		{
			SetLeftSteering(0);
		}

		if (Input.GetKey(KeyCode.LeftArrow))
		{
			SetRightSteering(-1);
		}
		else if (Input.GetKey(KeyCode.RightArrow))
		{
			SetRightSteering(1);
		}
		else
		{
			SetRightSteering(0);
		}

		if (Input.GetKey(KeyCode.UpArrow))
		{
			SetRightSpeed(1);
			SetRightBrakes(0);
		}
		else if (Input.GetKey(KeyCode.DownArrow))
		{
			SetRightSpeed(-1);
			SetRightBrakes(0);
		}
		else
		{
			SetRightSpeed(0);
			SetRightBrakes(0);
		}

		if (Input.GetKey(KeyCode.LeftControl))
		{
			SetLeftBrakes(1);
			SetLeftSpeed(0);
		}
		else if (Input.GetKey(KeyCode.RightControl))
		{
			SetRightBrakes(1);
			SetRightSpeed(0);
		}
	}

	private void SetLeftSpeed(float speed)
	{
		Controller.SetSpeed(WheelType.FrontLeft, speed);
		Controller.SetSpeed(WheelType.RearLeft, speed);
	}

	private void SetRightSpeed(float speed)
	{
		Controller.SetSpeed(WheelType.FrontRight, speed);
		Controller.SetSpeed(WheelType.RearRight, speed);
	}

	private void SetLeftBrakes(float speed)
	{
		Controller.SetBrake(WheelType.FrontLeft, speed);
		Controller.SetBrake(WheelType.RearLeft, speed);
	}

	private void SetRightBrakes(float speed)
	{
		Controller.SetBrake(WheelType.FrontRight, speed);
		Controller.SetBrake(WheelType.RearRight, speed);
	}

	private void SetLeftSteering(float steering)
	{
		Controller.SetSteering(WheelType.FrontLeft, steering);
	}

	private void SetRightSteering(float steering)
	{
		Controller.SetSteering(WheelType.FrontRight, steering);
	}
}
