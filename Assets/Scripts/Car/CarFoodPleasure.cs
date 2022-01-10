using System.Collections.Generic;
using UnityEngine;

public class CarFoodPleasure : MonoBehaviour, IInputNeuralModule
{
	[SerializeField] private float _maxPleasure;
	[SerializeField] private float _pleasureDecayPerSecond;
	[SerializeField] private CarFood _food;

	public int InputNeuronCount => 1;

	[SerializeField] private float _currentPleasure;

	private void Awake()
	{
		_food.OnPickupFood += OnPickupFood;
	}

	private void Update()
	{
		_currentPleasure -= _pleasureDecayPerSecond * Time.deltaTime;

		if (_currentPleasure < 0)
			_currentPleasure = 0;
	}

	private void OnPickupFood(Car car, float amount)
	{
		_currentPleasure = _maxPleasure;
	}

	public IEnumerable<float> GetInput()
	{
		yield return _currentPleasure;
	}
}