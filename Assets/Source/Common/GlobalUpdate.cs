using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn
{
	[InfoBox("A system for receiving Unity frame updates outside of normal MonoBehaviors.")]
	public class GlobalUpdate : MonoBehaviour
	{
		public static event System.Action OnUpdate, OnFixedUpdate, OnLateUpdate;

		[RuntimeInitializeOnLoadMethod]
		private static void ResetStatic() => OnUpdate = OnFixedUpdate = OnLateUpdate = null;

		private void Awake()
		{
			OnUpdate = null;
			OnFixedUpdate = null;
			OnLateUpdate = null;
		}

		private void Update()
		{
			OnUpdate?.Invoke();
		}

		private void FixedUpdate()
		{
			OnFixedUpdate?.Invoke();
		}

		private void LateUpdate()
		{
			OnLateUpdate?.Invoke();
		}
	}
}
