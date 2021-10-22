using GenericPanels;

public class MenuButtonsPanel : UiPanel<MenuButtonsPanel>
{
	public void ClickMenuButton()
	{
		if (MenuPanel.Instance.State == UiPanelState.Hidden || MenuPanel.Instance.State == UiPanelState.Hiding)
			MenuPanel.Instance.Show();
		else
			MenuPanel.Instance.Hide();
	}

    public void ClickCommandBarButton()
    {
		CommandBar.Instance.Toggle();
    }
}