public class ViewData
{
	public bool IsAnyObjectViewed;
	public float Distance;
	public float AngleYaw;
	public float AnglePitch;
	public float Color;

	public void ClearView()
	{
		IsAnyObjectViewed = false;
		Distance = 0;
		AngleYaw = 0;
		AnglePitch = 0;
		Color = 0;
	}
}