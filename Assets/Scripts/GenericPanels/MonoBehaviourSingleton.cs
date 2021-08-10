using UnityEngine;

namespace GenericPanels
{
	public class MonoBehaviourSingleton<T> : MonoBehaviour
	{
		public static T Instance;

		protected virtual void Awake()
		{
			Instance = GetComponent<T>();
		}
	}
}