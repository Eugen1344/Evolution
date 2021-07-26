using UnityEngine;

public class CarInput : MonoBehaviour
{
	public CarController Controller;

	private void Update()
	{
		if (Input.GetKey(KeyCode.W))
		{
			SetLeftSpeed(1);
		}
		else if (Input.GetKey(KeyCode.S))
		{
			SetLeftSpeed(-1);
		}

		if (Input.GetKey(KeyCode.UpArrow))
		{
			SetRightSpeed(1);
		}
		else if (Input.GetKey(KeyCode.DownArrow))
		{
			SetRightSpeed(-1);
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
}
