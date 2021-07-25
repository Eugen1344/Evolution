using UnityEngine;

public class Wheel : MonoBehaviour
{
	public WheelCollider Collider;

	private Quaternion _originalRotation;

	private void Awake()
	{
		_originalRotation = transform.localRotation;
	}

	private void FixedUpdate()
	{
		Vector3 position;
		Quaternion rotation;
		Collider.GetWorldPose(out position, out rotation);

		Collider.transform.rotation = rotation * _originalRotation;
	}

	public void SetTorque(float torque)
	{
		Collider.motorTorque = torque;
	}
}