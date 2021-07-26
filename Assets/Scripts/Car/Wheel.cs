using UnityEngine;

public class Wheel : MonoBehaviour
{
	public WheelCollider Collider;

	private float _currentTorque;
	private Quaternion _originalRotation;

	private void Awake()
	{
		_originalRotation = transform.localRotation;
	}

	private void FixedUpdate()
	{
		UpdateWheelModel();
	}

	private void UpdateWheelModel()
	{
		Vector3 position;
		Quaternion rotation;
		Collider.GetWorldPose(out position, out rotation);

		transform.rotation = rotation * _originalRotation;
	}

	public void SetTorque(float torque)
	{
		Collider.motorTorque = torque;
	}
}