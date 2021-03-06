using GenericPanels;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class CarPanel : UiPanel<CarPanel>
{
	public bool AutoSelectNextCar;
	public bool ViewLeftEye;
	public Car CurrentCar;
	public int PreviewLayer;
	public RawImage EyePreview;
	public Button PreviewLayerUp;
	public Button PreviewLayerDown;
	[SerializeField] private EvolutionManager _manager;

	public CarEye CurrentEye => CurrentCar == null ? null : ViewLeftEye ? CurrentCar.Eye : CurrentCar.Eye;

	private Texture2D _internalEyePreviewTexture;

	protected override void Awake()
	{
		Car.OnClick += SelectCar;
		_manager.OnInitialSpawn += OnSpawnGeneration;

		base.Awake();
	}

	protected override void OnDestroy()
	{
		if (CurrentCar != null)
		{
			CurrentCar.OnDespawn -= DespawnCar;
			CurrentCar.Brain.OnMadeDecision -= UpdatePreviewImage;
		}

		Car.OnClick -= SelectCar;
		_manager.OnInitialSpawn -= OnSpawnGeneration;

		if (_internalEyePreviewTexture != null)
			Destroy(_internalEyePreviewTexture);
	}

	private void OnSpawnGeneration()
	{
		if (!CurrentCar && AutoSelectNextCar)
		{
			SelectNextCar();
		}
	}

	private void SelectCar(Car car)
	{
		if (!car)
			return;

		Show();

		ClearCurrentCar();

		CurrentCar = car;

		CurrentCar.Select();
		ShowCarInfo(CurrentCar);

		CurrentCar.Brain.OnMadeDecision += UpdatePreviewImage;
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

		if (PreviewLayer < CurrentEye.Network.ConvolutionalLayers.Count - 1)
			PreviewLayerDown.interactable = true;

		if (PreviewLayer > 0)
			PreviewLayerUp.interactable = true;
	}

	private void ClearCurrentCar()
	{
		if (CurrentCar == null)
			return;

		CurrentCar.Unselect();
		CurrentCar.Brain.OnMadeDecision -= UpdatePreviewImage;
		CurrentCar.OnDespawn -= DespawnCar;
	}

	private void ShowCarInfo(Car car)
	{
		car.OnDespawn += DespawnCar;

		UpdateCurrentEyePreviewTexture();
	}

	private void DespawnCar(Car car)
	{
		if (AutoSelectNextCar)
		{
			SelectNextCar();
		}
		else
		{
			ClearCurrentCar();

			Hide();
		}
	}

	private void SelectNextCar()
	{
		Car nextCar = _manager.GetRandomCar();
		SelectCar(nextCar);
	}

	private void UpdatePreviewImage(Car car)
	{
		if (!car)
			return;

		ConvolutionalLayer currentLayer = CurrentEye.Network.ConvolutionalLayers[PreviewLayer];
		WriteImage(_internalEyePreviewTexture, currentLayer.PrevOutput);
	}

	private void WriteImage(Texture2D texture, float[,,] data)
	{
		if (data == null)
			return;

		for (int i = 0; i < data.GetLength(0); i++)
		{
			for (int j = 0; j < data.GetLength(1); j++)
			{
				Color color = new Color(data[i, j, 0], data[i, j, 1], data[i, j, 2], 1);

				texture.SetPixel(i, j, color);
			}
		}

		texture.Apply(false);
	}

	private void UpdateCurrentEyePreviewTexture()
	{
		if (CurrentCar == null)
			return;

		if (PreviewLayer < 0 || PreviewLayer >= CurrentEye.Network.ConvolutionalLayers.Count)
			PreviewLayer = 0;

		if (_internalEyePreviewTexture != null)
			Destroy(_internalEyePreviewTexture);

		_internalEyePreviewTexture = CreateEyePreviewTexture(CurrentCar, PreviewLayer);
		EyePreview.texture = _internalEyePreviewTexture;

		UpdatePreviewImage(CurrentCar);
		UpdatePreviewLayerButtons();
	}

	private Texture2D CreateEyePreviewTexture(Car car, int depth)
	{
		ConvolutionalLayer currentLayer = CurrentEye.Network.ConvolutionalLayers[depth];

		return CreateEyePreviewTexture(currentLayer.OutputPixelCount.x, currentLayer.OutputPixelCount.y);
	}

	private Texture2D CreateEyePreviewTexture(int width, int height)
	{
		Texture2D texture = new Texture2D(width, height, GraphicsFormat.R8G8B8A8_UNorm, TextureCreationFlags.None);
		texture.filterMode = FilterMode.Point;

		return texture;
	}
}