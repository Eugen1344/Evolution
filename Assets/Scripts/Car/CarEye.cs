using UnityEngine;

public class CarEye : MonoBehaviour
{
	public float ViewDistance;
	public float MaxViewAngle;
	public LayerMask VisibleLayers;

	public bool DebugLines;
	public int MaxDebugLines;

	private void Update()
	{
		Collider[] objectsAround = Physics.OverlapSphere(transform.position, ViewDistance, VisibleLayers, QueryTriggerInteraction.Collide);

		foreach (Collider obj in objectsAround)
		{
			Vector3 sightVector = obj.transform.position - transform.position;
			bool isSeen = IsWithinAngle(sightVector, transform.forward, MaxViewAngle);
			if (isSeen)
				Debug.Log($"{gameObject.name} = {obj.name}");
		}
	}

	private bool IsWithinAngle(Vector3 vector, Vector3 planeDirectional, float angle)
	{
		float sightVectorToNormalAngleCos = Mathf.Cos(angle * Mathf.Deg2Rad);
		float vectorToNormalAngleCos = VectorToPlaneAngleCos(vector, planeDirectional);

		return vectorToNormalAngleCos > sightVectorToNormalAngleCos;
	}

	private float VectorToPlaneAngleCos(Vector3 vector, Vector3 planeDirectional)
	{
		float vectorToNormalAngleCos = (vector.x * planeDirectional.x + vector.y * planeDirectional.y + vector.z * planeDirectional.z) / (vector.magnitude * planeDirectional.magnitude);
		return vectorToNormalAngleCos;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.position, ViewDistance);

		if (!DebugLines)
			return;

		Vector3 line = transform.forward * ViewDistance;

		for (int i = 0; i < MaxDebugLines; i++)
		{
			for (int j = 0; j < MaxDebugLines; j++)
			{
				Gizmos.color = IsWithinAngle(line, transform.forward, MaxViewAngle) ? Color.green : Color.red;

				Gizmos.DrawLine(transform.position, transform.position + line);

				line = Quaternion.AngleAxis(360.0f / MaxDebugLines, transform.up) * line;
			}

			line = Quaternion.AngleAxis(360.0f / MaxDebugLines, Vector3.right) * line;
		}
	}
}