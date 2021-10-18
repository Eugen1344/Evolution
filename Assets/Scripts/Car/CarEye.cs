using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class CarEye : MonoBehaviour, IInputNeuralModule
{
	public int PixelMultiplier;
	public Camera Camera;

	public RenderTexture RenderTexture { get; private set; }

	public int InputNeuronCount => 4;

	private readonly HashSet<VisibleObject> _invisibleObjects = new HashSet<VisibleObject>();

	private void Awake()
	{
		RenderTexture = new RenderTexture(16 * PixelMultiplier, 10 * PixelMultiplier, GraphicsFormat.R8G8B8A8_UNorm, GraphicsFormat.D32_SFloat_S8_UInt)
		{
			filterMode = FilterMode.Point
		};

		Camera.forceIntoRenderTexture = true;
		Camera.targetTexture = RenderTexture;
	}

	public IEnumerable<float> GetInput()
	{
		UpdateViewData();
	}

	public void UpdateViewData()
	{
		Camera.Render();
	}

	public void DisableSeeingObject(VisibleObject obj)
	{
		_invisibleObjects.Add(obj);
	}
}