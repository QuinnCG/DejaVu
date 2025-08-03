using FMODUnity;
using Quinn.DamageSystem;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

namespace Quinn.PlayerSystem
{
	public class Grabber : MonoBehaviour
	{
		[SerializeField, Required]
		private Transform HandOrigin;

		[Space]

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

		[Space]

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

		[Space]

		[SerializeField, AssetsOnly]
		private GameObject HandPrefab;
		[SerializeField, Required]
		private Sprite OpenHand, ClosedHand;
		[SerializeField]
		private float PunchRadius = 0.5f, PunchDamage = 10f, PunchKnockback = 12f, PunchCooldown = 1f, AdditionalPunchDistance = 2f, PunchSelfPush = 6f;

		[SerializeField, FoldoutGroup("SFX")]
		private EventReference GrabStartSound, GrabReachedSound, GrabReleasedSound, GrabRetractedSound, PunchHitSound;

		public bool IsGrabbing { get; private set; }
		public Vector2 GrabPosition => GetGrabPoint();

		private Rigidbody2D _rb;
		private SpringJoint2D _grabSpring;

		private Vector2 _grabPos, _desiredGrabPos;
		private float _targetTetherDistance;
		private GameObject _grabHand;
		private SpriteRenderer _handRenderer;

		private float _nextAllowedGrabTime;
		private float _grabStartTime;
		private bool _hasGrabReachedYet;
		private bool _isFailedGrabAttempt;

		private Rigidbody2D _grabbedRB;

		private float _nextAllowedPunchTime;
		private bool _isPunching;

		private void Awake()
		{
			_rb = GetComponent<Rigidbody2D>();
			_grabSpring = GetComponent<SpringJoint2D>();
		}

		private void Update()
		{
			if (IsGrabbing)
			{
				_grabSpring.enabled = (GetOriginPoint().DistanceTo(GetGrabPoint()) >= _targetTetherDistance) && !_isFailedGrabAttempt && !_isPunching;

				// Reduce velocity if it gets too high.
				if (_rb.linearVelocity.magnitude > VelocityReductionThreshold)
				{
					float vel = _rb.linearVelocity.magnitude;
					vel = Mathf.Max(0f, vel - (Time.deltaTime * VelocityDecayRate));

					_rb.linearVelocity = vel * _rb.linearVelocity.normalized;
				}
			}
		}

		private void LateUpdate()
		{
			if (IsGrabbing)
			{
				// 0-1 value starting when hand begins to extend and ending when it's fully extended.
				float normExtendElapsed = (Time.time - _grabStartTime) / LineExtendDuration;
				normExtendElapsed = Mathf.Min(normExtendElapsed, 1f);

				UpdateHandTransform(normExtendElapsed);
				UpdateLineData(normExtendElapsed);

				// Hand is fully extended.
				if (normExtendElapsed > 0.9f && !_hasGrabReachedYet)
				{
					_hasGrabReachedYet = true;
					Audio.Play(GrabReachedSound);

					if (_isFailedGrabAttempt || _isPunching)
					{
						_isPunching = false;
						_isFailedGrabAttempt = false;

						Release();
					}
				}

				_handRenderer.sprite = (!_isPunching && normExtendElapsed < 1f) ? OpenHand : ClosedHand;
			}
		}

		public void Grab()
		{
			if (!IsGrabbing && Time.time >= _nextAllowedGrabTime)
			{
				_isPunching = false;
				_grabbedRB = null;

				_desiredGrabPos = GetCursorWorldPos();
				_grabPos = GetGrabPoint(_desiredGrabPos);

				RaycastHit2D hit;

				var rigidbodyHit = LineOfSightRigidbody();
				if (rigidbodyHit.rigidbody != null)
				{
					hit = rigidbodyHit;
					_grabbedRB = hit.rigidbody;
				}
				else
				{
					hit = LineOfSightObstacle();
				}

				_isFailedGrabAttempt = false;

				if (hit.collider == null || GetOriginPoint().DistanceTo(hit.point) > MaxGrabDistance)
				{
					_isFailedGrabAttempt = true;
				}
				else
				{
					_grabPos = hit.point;
				}

				IsGrabbing = true;

				if (GetDistanceToGrabPoint() <= MaxGrabDistance)
				{
					if (hit.rigidbody == null)
					{
						_grabSpring.connectedAnchor = _grabPos;
						_grabSpring.connectedBody = null;
					}
					else
					{
						_grabSpring.connectedBody = hit.rigidbody;
						_grabSpring.connectedAnchor = Vector2.zero;
					}

					_targetTetherDistance = Mathf.Min(GetDistanceToGrabPoint() * GrabDistanceFactor, MaxGrabDistance);
					_grabSpring.distance = _targetTetherDistance;
				}

				CrosshairManager.Instance.Hide();

				_grabStartTime = Time.time;
				_grabHand = Instantiate(HandPrefab, GetOriginPoint(), GetHandRotation());
				_handRenderer = _grabHand.GetComponent<SpriteRenderer>();

				_hasGrabReachedYet = false;

				Audio.Play(GrabStartSound);
			}
		}

		public void Release()
		{
			if (IsGrabbing)
			{
				IsGrabbing = false;

				CrosshairManager.Instance.Show();
				Audio.Play(GrabReleasedSound);

				_nextAllowedGrabTime = Time.time + CooldownAfterRelease;
				_grabSpring.enabled = false;
				Line.positionCount = 0;

				_grabHand.Destroy();
			}
		}

		public void Punch()
		{
			if (Time.time > _nextAllowedPunchTime)
			{
				_nextAllowedPunchTime = Time.time + PunchCooldown;

				Grab();
				_isPunching = true;

				ApplyPunchDamage();
			}
		}

		private void ApplyPunchDamage()
		{
			Vector2 start = GetOriginPoint();
			Vector2 end = GetGrabPoint();

			Vector2 dir = start.DirectionTo(end);
			float maxDst = start.DistanceTo(end);

			bool hitAny = false;
			var hits = Physics2D.CircleCastAll(GetOriginPoint(), PunchRadius, dir, maxDst + 1f);

			foreach (var hit in hits)
			{
				if (hit.collider.TryGetComponent(out IDamageable damage))
				{
					bool success = damage.TakeDamage(new DamageInfo()
					{
						Damage = PunchDamage,
						Source = gameObject,
						Owner = gameObject,
						Direction = dir,
						Knockback = PunchKnockback,
						Team = Team.Friendly
					});

					if (success)
						hitAny = true;
				}
			}

			if (hitAny)
			{
				Audio.Play(PunchHitSound, GetGrabPoint());
				_rb.AddForce(-dir * PunchSelfPush, ForceMode2D.Impulse);
			}
		}

		private RaycastHit2D LineOfSightRigidbody()
		{
			Vector2 end = _desiredGrabPos + GetOriginPoint().DirectionTo(_desiredGrabPos) * 1f;
			var hits = Physics2D.LinecastAll(GetOriginPoint(), end).OrderBy(x => x.point.DistanceTo(GetOriginPoint())).ToList();

			return hits.FirstOrDefault(x => x.rigidbody != _rb && x.rigidbody != null && x.rigidbody.bodyType == RigidbodyType2D.Dynamic);
		}

		private RaycastHit2D LineOfSightObstacle()
		{
			Vector2 end = _desiredGrabPos + (Vector2)(GetOriginPoint().DirectionTo(_desiredGrabPos) * 1f);
			return Physics2D.Linecast(GetOriginPoint(), end, LayerMask.GetMask("Obstacle"));
		}

		private Quaternion GetHandRotation()
		{
			var handDir = GetOriginPoint().DirectionTo(GetGrabPoint());
			return Quaternion.AngleAxis((Mathf.Atan2(handDir.y, handDir.x) * Mathf.Rad2Deg) - 90f, Vector3.forward);
		}

		private Vector2 GetCursorWorldPos()
		{
			return Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}

		private Vector2 GetGrabPoint(Vector2 desiredPos)
		{
			var diff = desiredPos - (Vector2)GetOriginPoint();
			var clampedDiff = Vector2.ClampMagnitude(diff, MaxGrabDistance);

			return (Vector2)GetOriginPoint() + clampedDiff;
		}

		private AnimationCurve GetLineWidthCurve()
		{
			float dstNorm = Mathf.Min(GetOriginPoint().DistanceTo(GetGrabPoint()) / MaxGrabDistance, 1f);
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

		private float GetDistanceToGrabPoint()
		{
			return GetOriginPoint().DistanceTo(GetGrabPoint());
		}

		private void UpdateLineData(float normExtendElapsed)
		{
			float normGrabDst = Mathf.Min(GetDistanceToGrabPoint() / MaxGrabDistance, 1f);

			var vertices = new Vector3[32];

			Vector2 start = GetOriginPoint();
			Vector2 end = _grabHand.transform.position;

			Vector3 perpDir = GetOriginPoint().DirectionTo(_grabHand.transform.position);
			perpDir.Set(-perpDir.y, perpDir.x, 0f);

			for (int i = 0; i < vertices.Length; i++)
			{
				float t = i / (float)(vertices.Length - 1);

				Vector3 basePos = Vector3.Lerp(start, end, t);
				// Start under player but go above afterwards.
				basePos.z = (t < 0.5f) ? 0.1f : -0.1f;

				// Perpendicular offset (appearance of elastic band shooting out).
				Vector3 offset = Mathf.Sin(t * Mathf.PI * LineFrequencyFactor) * LineAmpFactor.Evaluate(normExtendElapsed) * LineAmpMultiplier * t * normGrabDst * perpDir;

				vertices[i] = basePos + offset;
			}

			Line.positionCount = vertices.Length;
			Line.SetPositions(vertices);
			Line.widthCurve = GetLineWidthCurve();
		}

		private void UpdateHandTransform(float normExtendElapsed)
		{
			Vector2 dir = GetOriginPoint().DirectionTo(GetGrabPoint());
			float addend = _isPunching ? AdditionalPunchDistance : 0f;

			var handPos = Vector2.Lerp(_grabHand.transform.position, GetOriginPoint() + (dir * (GetDistanceToGrabPoint() + addend)), normExtendElapsed);
			var handRot = GetHandRotation();

			_grabHand.transform.SetPositionAndRotation(handPos, handRot);
		}

		private Vector2 GetOriginPoint()
		{
			return HandOrigin.position;
		}

		private Vector2 GetGrabPoint()
		{
			if (_grabbedRB == null || _isPunching)
			{
				return _grabPos;
			}
			else
			{
				return _grabbedRB.worldCenterOfMass;
			}
		}
	}
}
