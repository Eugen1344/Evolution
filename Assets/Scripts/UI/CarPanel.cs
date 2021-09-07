using System;
using GenericPanels;
using UnityEngine;
using UnityEngine.UI;

public class CarPanel : UiPanel<CarPanel>
{
	public Car CurrentCar;
	public RawImage EyePreview;

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
	}

	private void ShowCarInfo(Car car)
	{
		CurrentCar = car;
		EyePreview.texture = car.Eye.RenderTexture;
	}
}