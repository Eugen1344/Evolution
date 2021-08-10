using System;

namespace GenericPanels
{
	public interface IUiPanel
	{
		public UiPanelState State { get; }
		public event EventHandler OnShow;
		public event EventHandler OnHide;
		public void Show();
		public void Hide();
	}
}