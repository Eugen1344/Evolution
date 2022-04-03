using System;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour, IInputNeuralModule, IOutputNeuralModule
{
	public float MaxForwardsSpeed;
	public float MaxBackwardsSpeed;

	[SerializeField] private Rigidbody _rigidbody;
	[SerializeField] private Wheel _frontLeft;
	[SerializeField] private Wheel _frontRight;
	[SerializeField] private Wheel _rearLeft;
	[SerializeField] private Wheel _rearRight;

	public void SetTorque(WheelType wheel, float normalizedTorque)
	{
		float normalizedSpeed = GetNormalizedSpeed();

		switch (wheel)
		{
			case WheelType.FrontLeft:
				_frontLeft.SetTorque(normalizedTorque, normalizedSpeed);
				break;
			case WheelType.FrontRight:
				_frontRight.SetTorque(normalizedTorque, normalizedSpeed);
				break;
			case WheelType.RearLeft:
				_rearLeft.SetTorque(normalizedTorque, normalizedSpeed);
				break;
			case WheelType.RearRight:
				_rearRight.SetTorque(normalizedTorque, normalizedSpeed);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(wheel), wheel, "Did you invent a new wheel?");
		}
	}

	public void SetBrakeForce(WheelType wheel, float normalizedBrakeForce)
	{
		switch (wheel)
		{
			case WheelType.FrontLeft:
				_frontLeft.SetBrakeForce(normalizedBrakeForce);
				break;
			case WheelType.FrontRight:
				_frontRight.SetBrakeForce(normalizedBrakeForce);
				break;
			case WheelType.RearLeft:
				_rearLeft.SetBrakeForce(normalizedBrakeForce);
				break;
			case WheelType.RearRight:
				_rearRight.SetBrakeForce(normalizedBrakeForce);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(wheel), wheel, "Did you invent a new wheel?");
		}
	}

	public void SetSteering(WheelType wheel, float normalizedSteeringAngle)
	{
		switch (wheel)
		{
			case WheelType.FrontLeft:
				_frontLeft.SetSteeringAngle(normalizedSteeringAngle);
				break;
			case WheelType.FrontRight:
				_frontRight.SetSteeringAngle(normalizedSteeringAngle);
				break;
			case WheelType.RearLeft:
				_rearLeft.SetSteeringAngle(normalizedSteeringAngle);
				break;
			case WheelType.RearRight:
				_rearRight.SetSteeringAngle(normalizedSteeringAngle);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(wheel), wheel, "Did you invent a new wheel?");
		}
	}

	public float GetOverallNormalizedTorque()
	{
		float frontLeft = Mathf.Abs(_frontLeft.LastNormalizedTorque);
		float frontRight = Mathf.Abs(_frontRight.LastNormalizedTorque);
		float rearLeft = Mathf.Abs(_rearLeft.LastNormalizedTorque);
		float rearRight = Mathf.Abs(_rearRight.LastNormalizedTorque);
		float overallNormalizedTorque = (frontLeft + frontRight + rearLeft + rearRight) / 4.0f;

		return overallNormalizedTorque;
	}

	public float GetNormalizedSpeed()
	{
		return GetForwardsNormalizedSpeed() - GetBackwardsNormalizedSpeed();
	}

	public float GetForwardsNormalizedSpeed()
	{
		float speed = GetSpeed();
		float forwardsSpeed = Mathf.Max(0, speed);

		return forwardsSpeed / MaxForwardsSpeed;
	}

	public float GetBackwardsNormalizedSpeed()
	{
		float speed = GetSpeed();
		float backwardsSpeed = -Mathf.Min(0, speed);

		return backwardsSpeed / MaxBackwardsSpeed;
	}

	public float GetSpeed()
	{
		if (!_rigidbody) //TODO hack
			return 0;

		Vector3 forward = _rigidbody.transform.forward;
		Vector3 projectedVelocity = Vector3.Project(_rigidbody.velocity, forward);
		float speed = Vector3.Dot(projectedVelocity, forward) < 0 ? -projectedVelocity.magnitude : projectedVelocity.magnitude;

		return speed;
	}

	public int InputNeuronCount => 2;

	public IEnumerable<float> GetInput()
	{
		float forwardSpeed = GetForwardsNormalizedSpeed();
		float backwardSpeed = GetBackwardsNormalizedSpeed();

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

		SetTorque(WheelType.FrontLeft, frontLeftForward - frontLeftBackward);
		SetTorque(WheelType.FrontRight, frontRightForward - frontRightBackward);
		SetTorque(WheelType.RearLeft, rearLeftForward - rearLeftBackward);
		SetTorque(WheelType.RearRight, rearRightForward - rearRightBackward);

		SetSteering(WheelType.FrontLeft, frontLeftSteeringRight - frontLeftSteeringLeft);
		SetSteering(WheelType.FrontRight, frontRightSteeringRight - frontRightSteeringLeft);
	}
}