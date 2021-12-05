using UnityEngine;

public class CarBody : MonoBehaviour
{
	public MeshRenderer Renderer;

	public void SetColor(Color color)
	{
		Renderer.material.color = color;
	}
}