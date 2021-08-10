using System;

namespace GenericPanels
{
	public interface IUiPanel
	{
		public UiPanelState PanelState { get; }
		public event EventHandler OnShow;
		public event EventHandler OnHide;
		public void Show(bool useTween = true);
		public void Hide(bool useTween = true);
	}
}