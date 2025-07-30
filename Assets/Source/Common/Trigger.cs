using UnityEngine;
using UnityEngine.Events;

namespace Quinn
{
	[RequireComponent(typeof(Collider2D))]
	public class Trigger : MonoBehaviour
	{
		[SerializeField]
		private UnityEvent OnTrigger;

		public void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.IsPlayer())
			{
				OnTrigger?.Invoke();
			}
		}
	}
}
