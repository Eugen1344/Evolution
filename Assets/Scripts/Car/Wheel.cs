using NWH.WheelController3D;
using UnityEngine;

public class Wheel : MonoBehaviour
{
	[SerializeField] private float _maxTorque;
	[SerializeField] private float _maxSteeringAngle;
	[SerializeField] public AnimationCurve _torqueBySpeedCurve;
	[SerializeField] private WheelController _controller;

	private Quaternion _originalRotation;

	public WheelController Controller => _controller;
	public float LastNormalizedTorque { get; private set; }

	public void SetTorque(float normalizedTorque, float normalizedSpeed)
	{
		float torque = CalculateTorque(normalizedTorque, normalizedSpeed);
		_controller.motorTorque = torque;

		LastNormalizedTorque = normalizedTorque;
	}

	private float CalculateTorque(float normalizedTorque, float normalizedSpeed)
	{
		float actualNormalizedTorque = _torqueBySpeedCurve.Evaluate(normalizedSpeed) * normalizedTorque;
		float actualTorque = Mathf.Clamp(actualNormalizedTorque, -1, 1) * _maxTorque;
		return actualTorque;
	}

	public void SetSteeringAngle(float normalizedSteeringAngle)
	{
		float steeringAngle = normalizedSteeringAngle * _maxSteeringAngle;
		_controller.steerAngle = steeringAngle;
	}

	public void SetBrakeForce(float normalizedBrakeForce)
	{
		float brakeForce = normalizedBrakeForce * _maxTorque;
		_controller.brakeTorque = brakeForce;
	}
}