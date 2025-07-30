using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class VelocityLimiter : MonoBehaviour
	{
		[SerializeField]
		private float MaxVelocity = 100f;

		private Rigidbody2D _rb;

		private void Awake()
		{
			_rb = GetComponent<Rigidbody2D>();
		}

		private void LateUpdate()
		{
			float mag = _rb.linearVelocity.magnitude;

			if (mag > MaxVelocity)
			{
				float clampedMag = Mathf.Min(mag, MaxVelocity);
				_rb.linearVelocity = _rb.linearVelocity.normalized * clampedMag;
			}
		}
	}
}
