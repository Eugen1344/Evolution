using UnityEngine;

public class CarBody : MonoBehaviour
{
	public Color Color { get; private set; }
	[SerializeField] private MeshRenderer _renderer;

	public void SetColor(Color color)
	{
		Color = color;
		_renderer.material.color = color;
	}
}