using UnityEngine;

namespace Quinn.PlayerSystem
{
	public class Grabber : MonoBehaviour
	{
		private SpringJoint2D _grabSpring;

		[SerializeField]
		private float GrabDistanceFactor = 1.5f;
		[SerializeField]
		private float GrabMaxDistance = 5f;
		[SerializeField]
		private float VelocityReductionThreshold = 20f;
		[SerializeField]
		private float VelocityDecayRate = 10f;

		private Vector2 _grabPos, _desiredGrabPos;
		private float _grabDst;

		private CursorStateHandle _cursorState;

		private void Awake()
		{
			_cursorState = InputManager.Instance.GetCursorStateHandle();
			_grabSpring = GetComponent<SpringJoint2D>();
		}

		private void Update()
		{
			if (Input.GetMouseButtonDown(1))
			{
				_desiredGrabPos = GetCursorWorldPos();
				_grabPos = GetGrabPoint(_desiredGrabPos);

				if (transform.position.DistanceTo(_grabPos) <= GrabMaxDistance)
				{
					_grabSpring.connectedAnchor = _grabPos;

					_grabDst = Mathf.Min(transform.position.DistanceTo(_grabPos) * GrabDistanceFactor, GrabMaxDistance);
					_grabSpring.distance = _grabDst;
				}

				_cursorState.ForceShowCursor = false;
			}
			else if (Input.GetMouseButton(1))
			{
				Debug.DrawLine(transform.position, _grabPos, Color.yellow);
				Draw.Rect(_desiredGrabPos, Vector2.one * 0.25f, Color.red);
				Draw.Rect(_grabPos, Vector2.one * 0.25f, Color.green);

				_grabSpring.enabled = transform.position.DistanceTo(_grabPos) >= _grabDst;

				var rb = GetComponent<Rigidbody2D>();

				if (rb.linearVelocity.magnitude > VelocityReductionThreshold)
				{
					float vel = rb.linearVelocity.magnitude;
					vel = Mathf.Max(0f, vel - (Time.deltaTime * VelocityDecayRate));

					rb.linearVelocity = vel * rb.linearVelocity.normalized;

					Draw.Sphere(transform.position, 1f, Color.red);
				}
			}
			else if (Input.GetMouseButtonUp(1))
			{
				_grabSpring.enabled = false;
				_cursorState.ForceShowCursor = true;
			}
		}

		private Vector2 GetCursorWorldPos()
		{
			return Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}

		private Vector2 GetGrabPoint(Vector2 desiredPos)
		{
			var diff = desiredPos - (Vector2)transform.position;
			var clampedDiff = Vector2.ClampMagnitude(diff, GrabMaxDistance);

			return (Vector2)transform.position + clampedDiff;
		}
	}
}
