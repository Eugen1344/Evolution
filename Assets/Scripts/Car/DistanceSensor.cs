using UnityEngine;

public class DistanceSensor : MonoBehaviour
{
	public Vector2Int RaysCount;
	public float SampleCountPerSecond = 5;

	public Camera ViewCamera;

	private void OnDrawGizmos()
	{
		for (int i = 0; i < RaysCount.x; i++)
		{
			for (int j = 0; j < RaysCount.y; j++)
			{
				Ray distanceRay = ViewCamera.ViewportPointToRay(new Vector3((float)i / RaysCount.x, (float)j / RaysCount.y));

				if (Physics.Raycast(distanceRay))
				{
					Gizmos.color = Color.red;
				}
				else
				{
					Gizmos.color = Color.white;
				}

				Gizmos.DrawRay(distanceRay);
			}
		}
	}
}