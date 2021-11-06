using NWH.WheelController3D;
using UnityEngine;

public class Wheel : MonoBehaviour
{
	public WheelController Controller;

	private Quaternion _originalRotation;

	private void Awake()
	{
		_originalRotation = transform.localRotation;
	}

	private void FixedUpdate()
	{
		//UpdateWheelModel();
	}

	private void UpdateWheelModel()
	{
		Vector3 position;
		Quaternion rotation;
		Controller.GetWorldPose(out position, out rotation);

		transform.rotation = rotation * _originalRotation;
	}

	public void SetTorque(float torque)
	{
		Controller.motorTorque = torque;
	}

	public void SetBrake(float brakeTorque)
	{
		Controller.brakeTorque = brakeTorque;
	}

	public void SetSteeringAngle(float steeringAngle)
	{
		Controller.steerAngle = steeringAngle;
	}
}