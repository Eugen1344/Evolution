using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraDepthMode : MonoBehaviour
{
	[ShowInInspector]
	public DepthTextureMode DepthMode => GetComponent<Camera>().depthTextureMode;
}