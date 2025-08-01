using UnityEngine;
using UnityEngine.Events;

namespace Quinn
{
	[RequireComponent(typeof(Collider2D))]
	public class Trigger : MonoBehaviour
	{
		[SerializeField]
		private UnityEvent OnEnter, OnExit;
		[SerializeField]
		private bool TriggerOnce;

		private bool _hasTriggered;

		public void OnTriggerEnter2D(Collider2D collision)
		{
			if (_hasTriggered && TriggerOnce)
				return;

			if (collision.IsPlayer())
			{
				OnEnter?.Invoke();
			}
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			if (_hasTriggered && TriggerOnce)
				return;

			if (collision.IsPlayer())
			{
				_hasTriggered = true;
				OnExit?.Invoke();
			}
		}
	}
}
