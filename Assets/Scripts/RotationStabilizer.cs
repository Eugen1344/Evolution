using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationStabilizer : MonoBehaviour
{
	private void FixedUpdate()
	{
		Vector3 rotation = transform.rotation.eulerAngles;
		transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);
	}
}
