using System;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour, IInputNeuralModule, IOutputNeuralModule
{
	public float MaxTorque;
	public float MaxForwardsSpeed;
	public float MaxBackwardsSpeed;
	public float MaxSteeringAngle;

	[SerializeField] public AnimationCurve _torqueBySpeedCurve;
	[SerializeField] private Rigidbody _rigidbody;
	[SerializeField] private Wheel _frontLeft;
	[SerializeField] private Wheel _frontRight;
	[SerializeField] private Wheel _rearLeft;
	[SerializeField] private Wheel _rearRight;

	private void Update()
	{
		float speed = GetSpeed();
		float forwardSpeed = GetForwardsNormalizedSpeed();
		float backwardSpeed = GetBackwardsNormalizedSpeed();
		Debug.Log($"{speed} = {forwardSpeed}; {backwardSpeed}");
	}

	public void SetTorque(WheelType wheel, float normalizedTorque)
	{
		float normalizedSpeed = GetNormalizedSpeed();
		float maxTorque = GetTorque(normalizedSpeed);
		float torque = normalizedTorque * maxTorque;

		switch (wheel)
		{
			case WheelType.FrontLeft:
				_frontLeft.SetTorque(torque);
				break;
			case WheelType.FrontRight:
				_frontRight.SetTorque(torque);
				break;
			case WheelType.RearLeft:
				_rearLeft.SetTorque(torque);
				break;
			case WheelType.RearRight:
				_rearRight.SetTorque(torque);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(wheel), wheel, "Did you invent a new wheel?");
		}
	}

	public float GetTorque(float normalizedSpeed)
	{
		return _torqueBySpeedCurve.Evaluate(normalizedSpeed) * MaxTorque;
	}

	public void SetBrake(WheelType wheel, float normalizedBrakeForce)
	{
		float brakeForce = MaxTorque * normalizedBrakeForce;

		switch (wheel)
		{
			case WheelType.FrontLeft:
				_frontLeft.SetBrake(brakeForce);
				break;
			case WheelType.FrontRight:
				_frontRight.SetBrake(brakeForce);
				break;
			case WheelType.RearLeft:
				_rearLeft.SetBrake(brakeForce);
				break;
			case WheelType.RearRight:
				_rearRight.SetBrake(brakeForce);
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
				_frontLeft.SetSteeringAngle(steeringAngle);
				break;
			case WheelType.FrontRight:
				_frontRight.SetSteeringAngle(steeringAngle);
				break;
			case WheelType.RearLeft:
				_rearLeft.SetSteeringAngle(steeringAngle);
				break;
			case WheelType.RearRight:
				_rearRight.SetSteeringAngle(steeringAngle);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(wheel), wheel, "Did you invent a new wheel?");
		}
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