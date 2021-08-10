using System;
using UnityEngine;

namespace GenericPanels
{
	[RequireComponent(typeof(CanvasGroup))]
	public abstract class UiPanel<T> : MonoBehaviourSingleton<T>, IUiPanel where T : UiPanel<T>
	{
		public event EventHandler OnShow;
		public event EventHandler OnHide;

		public UiPanelState State { get; protected set; }

		private CanvasGroup _canvasGroup;
		public CanvasGroup CanvasGroup
		{
			get
			{
				if (_canvasGroup == null)
				{
					_canvasGroup = GetComponent<CanvasGroup>();
				}

				return _canvasGroup;
			}
		}

		[ContextMenu("Show")]
		public virtual void Show()
		{
			State = UiPanelState.Shown;
			CanvasGroup.alpha = 1;
			CanvasGroup.blocksRaycasts = true;
			InvokeOnShow(this, EventArgs.Empty);
		}

		protected virtual void InvokeOnShow(object sender, EventArgs args)
		{
			OnShow?.Invoke(sender, args);
		}

		[ContextMenu("Hide")]
		public virtual void Hide()
		{
			CanvasGroup.alpha = 0;
			State = UiPanelState.Hidden;
			CanvasGroup.blocksRaycasts = false;
			InvokeOnHide(this, EventArgs.Empty);
		}

		protected void InvokeOnHide(object sender, EventArgs args)
		{
			OnHide?.Invoke(sender, args);
		}
	}
}