using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

namespace Quinn
{
	public class Button : MonoBehaviour
	{
		[SerializeField]
		private Collider2D Outer, Inner;
		[SerializeField]
		private SpringJoint2D Spring;

		[Space, SerializeField]
		private float PressThreshold = 0.5f;
		[SerializeField]
		private float ReleaseThreshold = 0.6f;

		[SerializeField, Space]
		private UnityEvent OnPress, OnRelease;
		[SerializeField]
		private EventReference PressSound, ReleaseSound;

		[Space, SerializeField]
		private bool EnableDebugLog;

		public bool IsPressed { get; private set; }

		private void Awake()
		{
			Physics2D.IgnoreCollision(Outer, Inner);
		}

		private void FixedUpdate()
		{
			float dst = Outer.bounds.center.DistanceTo(Inner.bounds.center);

			if (dst <= PressThreshold && !IsPressed)
			{
				IsPressed = true;
				OnPress?.Invoke();

				Audio.Play(PressSound, transform.position);

				if (EnableDebugLog)
					Log.Info("Button Pressed");
			}
			else if (dst > ReleaseThreshold && IsPressed)
			{
				IsPressed = false;
				OnRelease?.Invoke();

				Audio.Play(ReleaseSound, transform.position);

				if (EnableDebugLog)
					Log.Info("Button Released");
			}
		}
	}
}
