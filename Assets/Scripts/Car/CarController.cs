using System;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour, IInputNeuralModule, IOutputNeuralModule
{
	public AnimationCurve TorqueBySpeedCurve;

	public float MaxTorque;
	public float MaxSpeed;
	public float MaxSteeringAngle;

	public Rigidbody Rigidbody;
	public Wheel FrontLeft;
	public Wheel FrontRight;
	public Wheel RearLeft;
	public Wheel RearRight;

	public void SetSpeed(WheelType wheel, float normalizedSpeed)
	{
		float maxTorque = GetMaxTorque(GetTotalNormalizedSpeed());
		float torque = normalizedSpeed * maxTorque;

		switch (wheel)
		{
			case WheelType.FrontLeft:
				FrontLeft.SetTorque(torque);
				break;
			case WheelType.FrontRight:
				FrontRight.SetTorque(torque);
				break;
			case WheelType.RearLeft:
				RearLeft.SetTorque(torque);
				break;
			case WheelType.RearRight:
				RearRight.SetTorque(torque);
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

	public float GetMaxTorque(float normalizedSpeed)
	{
		return TorqueBySpeedCurve.Evaluate(normalizedSpeed) * MaxTorque;
	}

	public float GetTotalNormalizedSpeed()
	{
		return GetTotalSpeed() / MaxSpeed;
	}

	public float GetTotalSpeed()
	{
		if (!Rigidbody) //TODO hack
			return 0;

		Vector3 forward = Rigidbody.transform.forward;
		Vector3 projectedVelocity = Vector3.Project(Rigidbody.velocity, forward);
		float speed = Vector3.Dot(projectedVelocity, forward) < 0 ? -projectedVelocity.magnitude : projectedVelocity.magnitude;

		return speed;
	}

	public int InputNeuronCount => 2;

	public IEnumerable<float> GetInput()
	{
		float speed = GetTotalNormalizedSpeed();
		float forwardSpeed = Mathf.Max(0, speed);
		float backwardSpeed = Mathf.Max(0, -speed);

		yield return forwardSpeed;
		yield return backwardSpeed;
	}

	public int OutputNeuronCount => 12;

	public void SetOutput(float[] output, int startingIndex)
	{
		float frontLeftForward = output[startingIndex + 0];
		float frontLeftBackward = output[startingIndex + 1];
		float frontRightForward = output[startingIndex + 2];
		float frontRightBackward = output[startingIndex + 3];
		float rearLeftForward = output[startingIndex + 4];
		float rearLeftBackward = output[startingIndex + 5];
		float rearRightForward = output[startingIndex + 6];
		float rearRightBackward = output[startingIndex + 7];

		float frontLeftSteeringRight = output[startingIndex + 8];
		float frontLeftSteeringLeft = output[startingIndex + 9];
		float frontRightSteeringRight = output[startingIndex + 10];
		float frontRightSteeringLeft = output[startingIndex + 11];

		SetSpeed(WheelType.FrontLeft, frontLeftForward - frontLeftBackward);
		SetSpeed(WheelType.FrontRight, frontRightForward - frontRightBackward);
		SetSpeed(WheelType.RearLeft, rearLeftForward - rearLeftBackward);
		SetSpeed(WheelType.RearRight, rearRightForward - rearRightBackward);

		SetSteering(WheelType.FrontLeft, frontLeftSteeringRight - frontLeftSteeringLeft);
		SetSteering(WheelType.FrontRight, frontRightSteeringRight - frontRightSteeringLeft);
	}
}