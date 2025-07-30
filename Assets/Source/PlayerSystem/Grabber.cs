using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions.Must;

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
		[SerializeField]
		private LineRenderer Line;
		[SerializeField, AssetsOnly]
		private GameObject HandPrefab;

		public bool IsGrabbing { get; private set; }

		private Vector2 _grabPos, _desiredGrabPos;
		private float _grabDst;

		private GameObject _grabHand;
		private CursorStateHandle _cursorState;

		private void Awake()
		{
			_cursorState = InputManager.Instance.GetCursorStateHandle();
			_grabSpring = GetComponent<SpringJoint2D>();
		}

		private void Update()
		{
			if (IsGrabbing)
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

				var vertices = new Vector3[10];

				for (int i = 0; i < vertices.Length; i++)
				{
					vertices[i] = Vector3.Lerp(transform.position, _grabPos, i / (float)(vertices.Length - 1));
				}

				Line.positionCount = vertices.Length;
				Line.SetPositions(vertices);
			}
		}

		public void Grab()
		{
			if (!IsGrabbing)
			{
				IsGrabbing = true;

				_desiredGrabPos = GetCursorWorldPos();
				_grabPos = GetGrabPoint(_desiredGrabPos);

				if (transform.position.DistanceTo(_grabPos) <= GrabMaxDistance)
				{
					_grabSpring.connectedAnchor = _grabPos;

					_grabDst = Mathf.Min(transform.position.DistanceTo(_grabPos) * GrabDistanceFactor, GrabMaxDistance);
					_grabSpring.distance = _grabDst;
				}

				_cursorState.ForceShowCursor = false;

				_grabHand = HandPrefab.Clone(_grabPos);
				var handDir = transform.position.DirectionTo(_grabPos);
				_grabHand.transform.rotation = Quaternion.AngleAxis((Mathf.Atan2(handDir.y, handDir.x) * Mathf.Rad2Deg) - 90f, Vector3.forward);
			}
		}

		public void Release()
		{
			if (IsGrabbing)
			{
				IsGrabbing = false;

				_grabSpring.enabled = false;
				_cursorState.ForceShowCursor = true;

				Line.positionCount = 0;

				_grabHand.Destroy();
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
