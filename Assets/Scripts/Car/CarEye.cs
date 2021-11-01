using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class CarEye : MonoBehaviour, IInputNeuralModule
{
	public ConvolutionalNeuralNetwork Network;
	public Vector2Int PixelBaseSize;
	public int PixelMultiplier;
	public Camera Camera;

	public RenderTexture RenderTexture { get; private set; }

	public Vector2Int PixelCount => PixelBaseSize * PixelMultiplier;
	public int InputNeuronCount => Network.GetOutputLayerNeuronCount();

	private Texture2D _internalTexture;

	private void Awake()
	{
		_internalTexture = new Texture2D(PixelCount.x, PixelCount.y, GraphicsFormat.R8G8B8A8_UNorm, TextureCreationFlags.None);
		_internalTexture.hideFlags = HideFlags.HideAndDontSave;

		RenderTexture = new RenderTexture(PixelCount.x, PixelCount.y, GraphicsFormat.R8G8B8A8_UNorm, GraphicsFormat.D32_SFloat_S8_UInt)
		{
			filterMode = FilterMode.Point
		};
		RenderTexture.hideFlags = HideFlags.HideAndDontSave;

		Camera.forceIntoRenderTexture = true;
		Camera.targetTexture = RenderTexture;
	}

	private void OnDestroy()
	{
		Destroy(_internalTexture);
		Destroy(RenderTexture);
	}

	public IEnumerable<float> GetInput()
	{
		UpdateViewData();

		float[,] inputPixelData = GetPixelData();
		float[,] outputPixelData = Network.Calculate(inputPixelData);

		foreach (float pixel in outputPixelData)
		{
			yield return pixel;
		}
	}

	public void UpdateViewData()
	{
		Camera.Render();
	}

	private float[,] GetPixelData()
	{
		RenderTexture.active = RenderTexture;
		_internalTexture.ReadPixels(new Rect(0, 0, PixelCount.x, PixelCount.y), 0, 0, false);
		_internalTexture.GetRawTextureData<Color32>();
		NativeArray<Color32> pixelData = _internalTexture.GetRawTextureData<Color32>();

		float[,] data = new float[PixelCount.x, PixelCount.y];

		for (int i = 0; i < PixelCount.x; i++)
		{
			for (int j = 0; j < PixelCount.y; j++)
			{
				int flatArrayIndex = j * PixelCount.x + i;
				data[i, j] = (float)pixelData[flatArrayIndex].r / byte.MaxValue;
			}
		}

		RenderTexture.active = null;

		return data;
	}
}