using System;
using UnityEngine;

public class CarController : MonoBehaviour
{
	public float MaxSpeed;

	public WheelCollider FrontLeft;
	public WheelCollider FrontRight;
	public WheelCollider RearLeft;
	public WheelCollider RearRight;

	private void FixedUpdate()
	{

	}

	public void SetSpeed(WheelType wheel, float normalizedSpeed)
	{
		float speed = MaxSpeed * normalizedSpeed;

		switch (wheel)
		{
			case WheelType.FrontLeft:
				FrontLeft.motorTorque = speed;
				break;
			case WheelType.FrontRight:
				FrontRight.motorTorque = speed;
				break;
			case WheelType.RearLeft:
				RearLeft.motorTorque = speed;
				break;
			case WheelType.RearRight:
				RearRight.motorTorque = speed;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(wheel), wheel, "Did you invent a new wheel?");
		}
	}
}