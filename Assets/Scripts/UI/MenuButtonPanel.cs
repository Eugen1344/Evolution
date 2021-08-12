using GenericPanels;

public class MenuButtonPanel : UiPanel<MenuButtonPanel>
{
	public void ClickMenuButton()
	{
		if (MenuPanel.Instance.State == UiPanelState.Hidden || MenuPanel.Instance.State == UiPanelState.Hiding)
			MenuPanel.Instance.Show();
		else
			MenuPanel.Instance.Hide();
	}
}