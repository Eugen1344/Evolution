using System.Collections.Generic;
using System.Threading.Tasks;
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

	public Vector2Int PixelCount;
	public int InputNeuronCount => Network.GetOutputLayerNeuronCount();

	private Texture2D _internalTexture;
	private float[,,] _pixelData;
	private Color32[] _outputColorData;

	private void Awake()
	{
		PixelCount = PixelBaseSize * PixelMultiplier;
		_pixelData = new float[PixelCount.x, PixelCount.y, ConvolutionalNeuralNetwork.ColorChannelCount];
		_outputColorData = new Color32[PixelCount.x * PixelCount.y];

		_internalTexture = new Texture2D(PixelCount.x, PixelCount.y, GraphicsFormat.R8G8B8A8_UNorm, TextureCreationFlags.None);
		_internalTexture.hideFlags = HideFlags.HideAndDontSave;

		RenderTexture = new RenderTexture(PixelCount.x, PixelCount.y, GraphicsFormat.R8G8B8A8_UNorm, GraphicsFormat.D32_SFloat_S8_UInt)
		{
			filterMode = FilterMode.Point
		};
		RenderTexture.hideFlags = HideFlags.HideAndDontSave;
		RenderTexture.autoGenerateMips = false;

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
		Task<IEnumerable<float>> updateTask = UpdatePixelData();
		updateTask.Wait();

		return updateTask.Result;
	}

	public async Task<IEnumerable<float>> GetInputAsync()
	{
		UpdateViewData();

		return await UpdatePixelData();
	}

	public void UpdateViewData()
	{
		Camera.Render();
	}

	private async Task<IEnumerable<float>> UpdatePixelData()
	{
		float[,,] inputPixelData = GetPixelData();

		Task<float[,,]> calculateTask = Task.Run(() => Network.Calculate(inputPixelData));
		float[,,] outputPixelData = await calculateTask;

		return GetPixelData(outputPixelData);
	}

	private IEnumerable<float> GetPixelData(float[,,] data)
	{
		foreach (float pixel in data)
		{
			yield return pixel;
		}
	}

	private float[,,] GetPixelData()
	{
		RenderTexture.active = RenderTexture;
		_internalTexture.ReadPixels(new Rect(0, 0, PixelCount.x, PixelCount.y), 0, 0, false);
		_internalTexture.GetRawTextureData<Color32>();
		NativeArray<Color32> nativePixelData = _internalTexture.GetRawTextureData<Color32>();
		nativePixelData.CopyTo(_outputColorData);

		for (int i = 0; i < PixelCount.x; i++)
		{
			for (int j = 0; j < PixelCount.y; j++)
			{
				int flatArrayIndex = j * PixelCount.x + i;
				Color32 color = _outputColorData[flatArrayIndex];

				_pixelData[i, j, 0] = (float) color.r / byte.MaxValue;
				_pixelData[i, j, 1] = (float) color.g / byte.MaxValue;
				_pixelData[i, j, 2] = (float) color.b / byte.MaxValue;
			}
		}

		RenderTexture.active = null;

		return _pixelData;
	}
}