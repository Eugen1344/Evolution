using System;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour, IInputNeuralModule, IOutputNeuralModule
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

	public int InputNeuronCount => 1;

	public IEnumerable<float> GetInput()
	{
		yield return GetTotalNormalizedSpeed();
	}

	public int OutputNeuronCount => 6;

	public void SetOutput(float[] output, int startingIndex)
	{
		SetSpeed(WheelType.FrontLeft, output[startingIndex]);
		SetSpeed(WheelType.RearLeft, output[startingIndex + 1]);
		SetSpeed(WheelType.FrontRight, output[startingIndex + 2]);
		SetSpeed(WheelType.RearRight, output[startingIndex + 3]);
		SetSteering(WheelType.FrontLeft, output[startingIndex + 4]);
		SetSteering(WheelType.FrontRight, output[startingIndex + 5]);
	}
}