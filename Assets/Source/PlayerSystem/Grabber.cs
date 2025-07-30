using FMODUnity;
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
		private float VelocityReductionThreshold = 20f;
		[SerializeField]
		private float VelocityDecayRate = 10f;
		[SerializeField]
		private float CooldownAfterRelease = 0.3f;

		[SerializeField]
		private LineRenderer Line;
		[SerializeField]
		private AnimationCurve LineThicknessChangeOverDistance;
		[SerializeField]
		private float LineFrequencyFactor = 1f;
		[SerializeField]
		private AnimationCurve LineAmpFactor;
		[SerializeField]
		private float LineAmpMultiplier = 1f;
		[SerializeField]
		private float LineExtendDuration = 0.1f;
		[SerializeField, AssetsOnly]
		private GameObject HandPrefab;

		[SerializeField, FoldoutGroup("SFX")]
		private EventReference GrabStartSound, GrabReachedSound, GrabReleasedSound, GrabRetractedSound;

		public bool IsGrabbing { get; private set; }

		private Vector2 _grabPos, _desiredGrabPos;
		private float _initGrabDst;

		private GameObject _grabHand;

		private float _nextAllowedGrabTime;
		private float _grabStartTime;
		private bool _hasGrabReachedYet;

		private void Awake()
		{
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



				float normExtendElapsed = (Time.time - _grabStartTime) / LineExtendDuration;
				normExtendElapsed = Mathf.Min(normExtendElapsed, 1f);

				if (normExtendElapsed >= 1f && !_hasGrabReachedYet)
				{
					_hasGrabReachedYet = true;
					Audio.Play(GrabReachedSound);
				}

				float normGrabDst = Mathf.Min(transform.position.DistanceTo(_grabPos) / MaxGrabDistance, 1f);

				var vertices = new Vector3[32];

				for (int i = 0; i < vertices.Length; i++)
				{
					float t = i / (float)(vertices.Length - 1);
					Vector3 basePos = Vector3.Lerp(transform.position, _grabHand.transform.position, t);

					Vector3 perpDir = transform.position.DirectionTo(_grabHand.transform.position);
					perpDir.Set(-perpDir.y, perpDir.x, 0f);

					// Perpendicular offset (appearance of elastic band shooting out).
					Vector3 offset = Mathf.Sin(t * Mathf.PI * LineFrequencyFactor) * LineAmpFactor.Evaluate(normExtendElapsed) * LineAmpMultiplier * t * normGrabDst * perpDir;

					vertices[i] = basePos + offset;
				}

				Line.positionCount = vertices.Length;
				Line.SetPositions(vertices);
				Line.widthCurve = GetLineWidthCurve();

				var handPos = Vector2.LerpUnclamped(_grabHand.transform.position, _grabPos, normExtendElapsed);
				var handRot = GetHandRotation();
				_grabHand.transform.SetPositionAndRotation(handPos, handRot);
			}
		}

		public void Grab()
		{
			if (!IsGrabbing && Time.time >= _nextAllowedGrabTime)
			{
				IsGrabbing = true;

				_desiredGrabPos = GetCursorWorldPos();
				_grabPos = GetGrabPoint(_desiredGrabPos);

				if (transform.position.DistanceTo(_grabPos) <= MaxGrabDistance)
				{
					_grabSpring.connectedAnchor = _grabPos;

					_initGrabDst = Mathf.Min(transform.position.DistanceTo(_grabPos) * GrabDistanceFactor, MaxGrabDistance);
					_grabSpring.distance = _initGrabDst;
				}

				CrosshairManager.Instance.Hide();

				_grabStartTime = Time.time;
				_grabHand = Instantiate(HandPrefab, transform.position, GetHandRotation());

				_hasGrabReachedYet = false;

				Audio.Play(GrabStartSound);
			}
		}

		public void Release()
		{
			if (IsGrabbing)
			{
				IsGrabbing = false;

				_grabSpring.enabled = false;
				CrosshairManager.Instance.Show();

				Line.positionCount = 0;

				_grabHand.Destroy();

				_nextAllowedGrabTime = Time.time + CooldownAfterRelease;

				Audio.Play(GrabReleasedSound);
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
			float dstNorm = Mathf.Min(transform.position.DistanceTo(_grabPos) / MaxGrabDistance, 1f);
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
