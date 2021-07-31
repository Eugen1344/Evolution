using System;
using UnityEngine;

public class CarController : MonoBehaviour
{
	public float MaxTorque;
	public float MaxSteeringAngle;
	public float MaxCarSpeed;

	public Rigidbody Rigidbody;
	public Wheel FrontLeft;
	public Wheel FrontRight;
	public Wheel RearLeft;
	public Wheel RearRight;

	public void SetSpeed(WheelType wheel, float normalizedSpeed)
	{
		float speed = MaxTorque * normalizedSpeed;

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

	public void SetBrake(WheelType wheel, float normalizedBrakeForce)
	{
		float brakeForce = MaxTorque * normalizedBrakeForce;

		switch (wheel)
		{
			case WheelType.FrontLeft:
				FrontLeft.SetBrake(brakeForce);
				break;
			case WheelType.FrontRight:
				FrontRight.SetBrake(brakeForce);
				break;
			case WheelType.RearLeft:
				RearLeft.SetBrake(brakeForce);
				break;
			case WheelType.RearRight:
				RearRight.SetBrake(brakeForce);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(wheel), wheel, "Did you invent a new wheel?");
		}
	}

	public void SetSteering(WheelType wheel, float normalizedSteeringFactor)
	{
		float steeringAngle = MaxSteeringAngle * normalizedSteeringFactor;

		switch (wheel)
		{
			case WheelType.FrontLeft:
				FrontLeft.SetSteeringAngle(steeringAngle);
				break;
			case WheelType.FrontRight:
				FrontRight.SetSteeringAngle(steeringAngle);
				break;
			case WheelType.RearLeft:
				RearLeft.SetSteeringAngle(steeringAngle);
				break;
			case WheelType.RearRight:
				RearRight.SetSteeringAngle(steeringAngle);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(wheel), wheel, "Did you invent a new wheel?");
		}
	}

	public float GetTotalNormalizedSpeed()
	{
		return Mathf.Clamp(GetTotalSpeed() / MaxCarSpeed, -1.0f, 1.0f);
	}

	public float GetTotalSpeed()
	{
		Vector3 projectedVelocity = Vector3.Project(Rigidbody.velocity, Rigidbody.transform.forward);
		float speed = Vector3.Dot(projectedVelocity, Rigidbody.transform.forward) < 0 ? -projectedVelocity.magnitude : projectedVelocity.magnitude;

		return speed;
	}
}