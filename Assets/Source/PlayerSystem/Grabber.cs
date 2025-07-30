using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Quinn.PlayerSystem
{
	public class Grabber : MonoBehaviour
	{
		private SpringJoint2D _grabSpring;

		[SerializeField]
		private float MaxGrabDistance = 5f;
		[SerializeField]
		private float GrabDistanceFactor = 1.5f;
		[SerializeField]
		private float MaxTetherDistance = 5f;
		[SerializeField]
		private float VelocityReductionThreshold = 20f;
		[SerializeField]
		private float VelocityDecayRate = 10f;
		[SerializeField]
		private LineRenderer Line;
		[SerializeField]
		private AnimationCurve LineThicknessChangeOverDistance;
		[SerializeField, AssetsOnly]
		private GameObject HandPrefab;

		public bool IsGrabbing { get; private set; }

		private Vector2 _grabPos, _desiredGrabPos;
		private float _initGrabDst;

		private GameObject _grabHand;
		private CursorStateHandle _cursorState;

		private Vector2 _grabHandVel;

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

				_grabSpring.enabled = transform.position.DistanceTo(_grabPos) >= _initGrabDst;

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
					vertices[i] = Vector3.Lerp(transform.position, _grabHand.transform.position, i / (float)(vertices.Length - 1));
				}

				Line.positionCount = vertices.Length;
				Line.SetPositions(vertices);
				Line.widthCurve = GetLineWidthCurve();

				Vector2 handPos = Vector2.SmoothDamp(_grabHand.transform.position, _grabPos, ref _grabHandVel, 0.01f);
				Quaternion handRot = GetHandRotation();
				_grabHand.transform.SetPositionAndRotation(handPos, handRot);
			}
		}

		public void Grab()
		{
			if (!IsGrabbing)
			{
				IsGrabbing = true;

				_desiredGrabPos = GetCursorWorldPos();
				_grabPos = GetGrabPoint(_desiredGrabPos);

				if (transform.position.DistanceTo(_grabPos) <= MaxTetherDistance)
				{
					_grabSpring.connectedAnchor = _grabPos;

					_initGrabDst = Mathf.Min(transform.position.DistanceTo(_grabPos) * GrabDistanceFactor, MaxTetherDistance);
					_grabSpring.distance = _initGrabDst;
				}

				_cursorState.ForceShowCursor = false;

				_grabHandVel = Vector2.zero;
				_grabHand = Instantiate(HandPrefab, transform.position, GetHandRotation());
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

		private Quaternion GetHandRotation()
		{
			var handDir = transform.position.DirectionTo(_grabPos);
			return Quaternion.AngleAxis((Mathf.Atan2(handDir.y, handDir.x) * Mathf.Rad2Deg) - 90f, Vector3.forward);
		}

		private Vector2 GetCursorWorldPos()
		{
			return Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}

		private Vector2 GetGrabPoint(Vector2 desiredPos)
		{
			var diff = desiredPos - (Vector2)transform.position;
			var clampedDiff = Vector2.ClampMagnitude(diff, MaxGrabDistance);

			return (Vector2)transform.position + clampedDiff;
		}

		private AnimationCurve GetLineWidthCurve()
		{
			float dstNorm = Mathf.Min(transform.position.DistanceTo(_grabPos) / MaxTetherDistance, 1f);
			float t = LineThicknessChangeOverDistance.Evaluate(dstNorm);

			var start = new Keyframe(0f, 1f)
			{
				outTangent = Mathf.Lerp(3f, -5f, t)
			};

			var mid = new Keyframe(0.5f, Mathf.Lerp(1.5f, 0.4f, t))
			{
				inTangent = 0f,
				outTangent = 0f
			};

			var end = new Keyframe(1f, 1f)
			{
				inTangent = Mathf.Lerp(-3f, 5f, t)
			};

			return new AnimationCurve(start, mid, end);
		}
	}
}
