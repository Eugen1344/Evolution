using System;
using UnityEngine;

public class CarController : MonoBehaviour
{
	public float MaxSpeed;

	public Wheel FrontLeft;
	public Wheel FrontRight;
	public Wheel RearLeft;
	public Wheel RearRight;

	public void SetSpeed(WheelType wheel, float normalizedSpeed)
	{
		float speed = MaxSpeed * normalizedSpeed;

		switch (wheel)
		{
			case WheelType.FrontLeft:
				FrontLeft.SetTorque(speed);
				break;
			case WheelType.FrontRight:
				FrontRight.SetTorque(speed);
				break;
			case WheelType.RearLeft:
				RearLeft.SetTorque(speed);
				break;
			case WheelType.RearRight:
				RearRight.SetTorque(speed);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(wheel), wheel, "Did you invent a new wheel?");
		}
	}
}