using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class CarEye : MonoBehaviour, IInputNeuralModule
{
	public float ViewDistance;
	public float MaxViewAngleYaw;
	public float MaxViewAnglePitch;
	public LayerMask VisibleLayers;
	public Camera Camera;
	public Shader EyeGlobalShader;

	public bool DebugLines;
	public int MaxDebugLines;

	public readonly ViewData CurrentViewData = new ViewData();

	public int InputNeuronCount => 4;

	private HashSet<VisibleObject> _invisibleObjects = new HashSet<VisibleObject>();

	private void Awake()
	{
		Camera.targetTexture = new RenderTexture(10, 10, GraphicsFormat.R8G8B8A8_UNorm, GraphicsFormat.D32_SFloat_S8_UInt);

		Camera.forceIntoRenderTexture = true;
	}

	public IEnumerable<float> GetInput()
	{
		UpdateViewData();

		yield return CurrentViewData.Distance;
		yield return CurrentViewData.AngleYaw;
		yield return CurrentViewData.AnglePitch;
		yield return CurrentViewData.Color;
	}

	public void UpdateViewData()
	{
		Camera.RenderWithShader(EyeGlobalShader, "");

		VisibleObject closestObj = ClosestObjectAround(out Vector3 closestPoint, out float distance);

		if (closestObj == null)
		{
			CurrentViewData.ClearView();

			return;
		}

		Vector3 forward = transform.forward;
		Vector3 sightVector = closestPoint - transform.position;

		float sightProjectionYawAngle = SightVectorToPlaneAxisSignedAngle(sightVector, transform.up, forward);
		float sightProjectionPitchAngle = SightVectorToPlaneAxisSignedAngle(sightVector, transform.right, forward);

		bool isInViewRange = IsInViewRange(sightProjectionYawAngle, MaxViewAngleYaw) && IsInViewRange(sightProjectionPitchAngle, MaxViewAnglePitch);

		if (isInViewRange)
		{
			CurrentViewData.IsAnyObjectViewed = true;
			CurrentViewData.Distance = distance / ViewDistance;
			CurrentViewData.AngleYaw = sightProjectionYawAngle / 180.0f;
			CurrentViewData.AnglePitch = sightProjectionPitchAngle / 180.0f;
			CurrentViewData.Color = closestObj.Color;
		}
		else
		{
			CurrentViewData.ClearView();
		}
	}

	private float SightVectorToPlaneAxisSignedAngle(Vector3 sightVector, Vector3 planeNormal, Vector3 axis)
	{
		Vector3 yawSightProjection = Vector3.ProjectOnPlane(sightVector, planeNormal);
		float sightProjectionForwardAngle = Vector3.SignedAngle(yawSightProjection, axis, planeNormal);

		return sightProjectionForwardAngle;
	}

	private bool IsInViewRange(float angle, float maxAngle)
	{
		return Mathf.Abs(angle) <= Mathf.Abs(maxAngle);
	}

	private VisibleObject ClosestObjectAround(out Vector3 closestObjectPoint, out float distance)
	{
		Vector3 position = transform.position;
		Collider[] objectsAround = Physics.OverlapSphere(transform.position, ViewDistance, VisibleLayers, QueryTriggerInteraction.Collide);
		closestObjectPoint = Vector3.zero;
		distance = 0;

		float minDistance = 0;
		VisibleObject closestVisibleObj = null;

		foreach (Collider obj in objectsAround)
		{
			VisibleObject visibleObj = obj.GetComponent<VisibleObject>();

			if (!visibleObj || _invisibleObjects.Contains(visibleObj))
				continue;

			Vector3 point = obj.ClosestPoint(position);
			distance = Vector3.Distance(point, position);

			if (closestVisibleObj == null || distance < minDistance)
			{
				minDistance = distance;
				closestObjectPoint = point;
				closestVisibleObj = visibleObj;
			}
		}

		return closestVisibleObj;
	}

	public void DisableSeeingObject(VisibleObject obj)
	{
		_invisibleObjects.Add(obj);
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
				float yawAngle = SightVectorToPlaneAxisSignedAngle(line, transform.up, transform.forward);
				float pitchAngle = SightVectorToPlaneAxisSignedAngle(line, transform.right, transform.forward);

				Gizmos.color = IsInViewRange(yawAngle, MaxViewAngleYaw) && IsInViewRange(pitchAngle, MaxViewAnglePitch) ? Color.green : Color.red;

				Gizmos.DrawLine(transform.position, transform.position + line);

				line = Quaternion.AngleAxis(360.0f / MaxDebugLines, transform.up) * line;
			}

			line = Quaternion.AngleAxis(360.0f / MaxDebugLines, Vector3.right) * line;
		}
	}
}