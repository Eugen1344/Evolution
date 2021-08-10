using UnityEngine;

namespace Develop
{
	public class LineRendererFollowObject : MonoBehaviour
	{
		public float UpdateInterval = 1f;
		public int MaxPoints = 15;
		private float _lastUpdateTime = -1;

		private LineRenderer _lineRenderer = null;
		public LineRenderer LineRenderer
		{
			get
			{
				if (_lineRenderer == null)
				{
					_lineRenderer = GetComponent<LineRenderer>();
				}
				return _lineRenderer;
			}
		}

		private void FixedUpdate()
		{
			if (LineRenderer == null || _lastUpdateTime + UpdateInterval > Time.fixedTime)
			{
				return;
			}
			int lineRendererPositionCount = LineRenderer.positionCount;
			Vector3[] oldPoints = new Vector3[lineRendererPositionCount];
			LineRenderer.GetPositions(oldPoints);
			Vector3[] newPoints;
			if (oldPoints.Length < MaxPoints)
			{
				newPoints = new Vector3[lineRendererPositionCount + 1];
			}
			else
			{
				newPoints = new Vector3[lineRendererPositionCount];
			}
			newPoints[0] = transform.position;
			for (int index = 0; index < oldPoints.Length; index++)
			{
				int lastElementIndex = oldPoints.Length - 1;
				if (lineRendererPositionCount >= MaxPoints && index == lastElementIndex)
				{
					continue;
				}
				newPoints[index + 1] = oldPoints[index];
			}
			LineRenderer.positionCount = newPoints.Length;
			LineRenderer.SetPositions(newPoints);
			_lastUpdateTime = Time.fixedTime;
		}
	}
}
