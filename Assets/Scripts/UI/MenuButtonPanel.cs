using GenericPanels;

public class MenuButtonPanel : UiPanel<MenuButtonPanel>
{
	public void ClickMenuButton()
	{
		MenuPanel.Instance.Show();
	}
}