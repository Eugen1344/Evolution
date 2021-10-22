using GenericPanels;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class CarPanel : UiPanel<CarPanel>
{
	public bool AutoSelectBestCar;	
	public Car CurrentCar;
	public int PreviewLayer;
	public RawImage EyePreview;
	public Button PreviewLayerUp;
	public Button PreviewLayerDown;

	private Texture2D _internalEyePreviewTexture;

	protected override void Awake()
	{
		Car.OnClick += ClickOnCar;

		base.Awake();
	}

	private void OnDestroy()
	{
		Car.OnClick -= ClickOnCar;
	}

	private void ClickOnCar(Car car)
	{
		Show();

		ShowCarInfo(car);

		ClearCurrentCar();

		car.Brain.OnMadeDecision += UpdatePreviewImage;
	}

	public void ClickPreviewLayerUp()
	{
		PreviewLayer--;
		UpdateCurrentEyePreviewTexture();
	}

	public void ClickPreviewLayerDown()
	{
		PreviewLayer++;
		UpdateCurrentEyePreviewTexture();
	}

	private void UpdatePreviewLayerButtons()
	{
		PreviewLayerDown.interactable = false;
		PreviewLayerUp.interactable = false;

		if (CurrentCar == null)
			return;

		if (PreviewLayer < CurrentCar.Eye.Network.NeuronLayers.Count - 1)
			PreviewLayerDown.interactable = true;

		if (PreviewLayer > 0)
			PreviewLayerUp.interactable = true;
	}

	private void ClearCurrentCar()
	{
		if (CurrentCar == null)
			return;

		CurrentCar.Brain.OnMadeDecision -= UpdatePreviewImage;
		CurrentCar.OnDespawn -= DespawnCar;
	}

	private void ShowCarInfo(Car car)
	{
		CurrentCar = car;
		car.OnDespawn += DespawnCar;

		UpdateCurrentEyePreviewTexture();
	}

	private void DespawnCar(Car car)
	{
		Hide();

		ClearCurrentCar();
	}

	private void UpdatePreviewImage(Car car)
	{
		ConvolutionalLayer currentLayer = car.Eye.Network.NeuronLayers[PreviewLayer];
		WriteImage(_internalEyePreviewTexture, currentLayer.PrevOutput);
	}

	private void WriteImage(Texture2D texture, float[,] data)
	{
		for (int i = 0; i < data.GetLength(0); i++)
		{
			for (int j = 0; j < data.GetLength(1); j++)
			{
				Color color = new Color(data[i, j], 0, 0, 1);

				texture.SetPixel(i, j, color);
			}
		}

		texture.Apply(false);
	}

	private void UpdateCurrentEyePreviewTexture()
	{
		if (CurrentCar == null)
			return;

		if (PreviewLayer < 0 || PreviewLayer >= CurrentCar.Eye.Network.NeuronLayers.Count)
			PreviewLayer = 0;

		_internalEyePreviewTexture = CreateEyePreviewTexture(CurrentCar, PreviewLayer);
		EyePreview.texture = _internalEyePreviewTexture;

		UpdatePreviewImage(CurrentCar);
		UpdatePreviewLayerButtons();
	}

	private Texture2D CreateEyePreviewTexture(Car car, int depth)
	{
		ConvolutionalLayer currentLayer = car.Eye.Network.NeuronLayers[depth];

		return CreateEyePreviewTexture(currentLayer.NeuronsSizeX, currentLayer.NeuronsSizeY);
	}

	private Texture2D CreateEyePreviewTexture(int width, int height)
	{
		Texture2D texture = new Texture2D(width, height, GraphicsFormat.R8G8B8A8_UNorm, TextureCreationFlags.None);
		texture.filterMode = FilterMode.Point;

		return texture;
	}
}