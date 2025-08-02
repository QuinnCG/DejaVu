using FMODUnity;
using QFSW.QC;
using Quinn.DamageSystem;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Quinn.PlayerSystem
{
	public class Player : MonoBehaviour
	{
		public static Player Instance { get; private set; }

		[SerializeField]
		private float DeathDuration = 3f;

		[Space]

		[SerializeField]
		private float MinAccelerationRate = 2f, MaxAccelerationRate = 16f;
		[SerializeField]
		private float LinearDrag = 5f;
		[SerializeField]
		private float AnimSpeedFactorMaxVel = 30f;

		[Space]

		[SerializeField]
		private float AccelerationFactorWhileGrabbing = 1f;
		[SerializeField]
		private float DragFactorWhileGrabbing = 1f;

		[Space]

		[SerializeField]
		private float DashForce = 12f;
		[SerializeField]
		private float DashCooldown = 0.5f;
		[SerializeField]
		private EventReference DashSound;

		[Space]

		[SerializeField, Required, ChildGameObjectsOnly]
		private Transform CameraTarget;
		[SerializeField]
		private float CamTargetPlayerToCursorBias = 0.3f;

		[Space]

		[SerializeField, Required]
		private Transform RootSprite;

		[SerializeField, FoldoutGroup("SFX")]
		private EventReference Footstep;

		public Collider2D Collider { get; private set; }
		public Health Health { get; private set; }

		private Animator _animator;
		private Rigidbody2D _rb;
		private Grabber _grabber;

		private float _nextAllowedDashTime;
		// Used for final sequence.
		private bool _isInjured;

		private void Awake()
		{
			Instance = this;

			Collider = GetComponent<Collider2D>();
			Health = GetComponent<Health>();

			Health.OnDeath += OnDeath;

			_animator = GetComponent<Animator>();
			_rb = GetComponent<Rigidbody2D>();
			_grabber = GetComponent<Grabber>();
		}

		private void Start()
		{
			CrosshairManager.Instance.Show();

			if (!string.IsNullOrWhiteSpace(Checkpoint.ActiveCheckpoint))
			{
				var checkpoint = Checkpoint.GetCheckpoint(Checkpoint.ActiveCheckpoint);
				transform.position = checkpoint.SpawnPoint.position;
			}

			TransitionManager.Instance.FadeFromBlack(2f);
		}

		private void Update()
		{
			if (Health.IsDead)
				return;

			Vector2 moveDir = new Vector2()
			{
				x = Input.GetAxis("Horizontal"),
				y = Input.GetAxis("Vertical")
			}.normalized;

			UpdateDash(moveDir);
			UpdateGrab();
			UpdatePunch();
			UpdateMovement(moveDir);
		}

		private void LateUpdate()
		{
			UpdateCameraTarget();
			UpdateFaceDirection();
			UpdateAnimation();
		}

		private void FixedUpdate()
		{
			float mag = _rb.linearVelocity.magnitude;

			if (mag < 0.5f)
			{
				mag = Mathf.Max(0f, mag - (Time.deltaTime * 5f));
				_rb.linearVelocity = _rb.linearVelocity.normalized * mag;
			}
		}

		private void OnDeath()
		{
			_rb.linearVelocity = Vector2.zero;

			CrosshairManager.Instance.Hide();

			StopAllCoroutines();
			StartCoroutine(DeathSequence());
		}

		private IEnumerator DeathSequence()
		{
			_animator.SetTrigger("Death");

			TransitionManager.Instance.FadeToBlack(DeathDuration);
			yield return new WaitForSeconds(DeathDuration);

			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		private void UpdateMovement(Vector2 moveDir)
		{
			Vector2 a1 = moveDir;
			Vector2 a2 = _rb.linearVelocity.normalized;

			float aDelta = Vector2.Angle(a1, a2);
			float norm = Mathf.Clamp01(aDelta / 180f);

			float force = Mathf.Lerp(MinAccelerationRate, MaxAccelerationRate, norm);
			force *= _grabber.IsGrabbing ? AccelerationFactorWhileGrabbing : 1f;

			_rb.AddForce(force * Time.deltaTime * moveDir);
			_rb.linearDamping = LinearDrag * (_grabber.IsGrabbing ? DragFactorWhileGrabbing : 1f);
		}

		private void UpdateDash(Vector2 moveDir)
		{
			if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextAllowedDashTime)
			{
				_nextAllowedDashTime = Time.time + DashCooldown;

				_rb.AddForce(moveDir * DashForce, ForceMode2D.Impulse);
				Audio.Play(DashSound, transform.position);
			}
		}

		private void UpdateGrab()
		{
			if (Input.GetMouseButtonDown(1))
			{
				_grabber.Grab();
			}
			else if (Input.GetMouseButtonUp(1))
			{
				_grabber.Release();
			}
		}

		private void UpdatePunch()
		{
			if (Input.GetMouseButtonDown(0) && !_grabber.IsGrabbing)
			{
				_grabber.Punch();
			}
		}

		private void UpdateCameraTarget()
		{
			Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			CameraTarget.position = Vector2.Lerp(transform.position, mousePos, CamTargetPlayerToCursorBias);

			Draw.Sphere(CameraTarget.position, 0.2f, Color.azure, 0f, true);
		}

		private void UpdateFaceDirection()
		{
			Vector2 dir = transform.position.DirectionTo(CrosshairManager.Instance.Position);

			if (_grabber.IsGrabbing)
			{
				dir = transform.position.DirectionTo(_grabber.GrabPosition);
			}

			dir = _rb.linearVelocity.normalized;

			if (dir.x != 0f)
			{
				float xDir = Mathf.Sign(dir.x);

				var scale = transform.localScale;
				scale.x = Mathf.Abs(scale.x) * xDir;
				transform.localScale = scale;
			}
		}

		private void UpdateAnimation()
		{
			_animator.SetFloat("NormSpeed", _rb.linearVelocity.magnitude / AnimSpeedFactorMaxVel);
			_animator.SetBool("IsMoving", _rb.linearVelocity.sqrMagnitude > 0f);
			_animator.SetBool("IsInjured", _isInjured);
		}

		protected void Footstep_Anim()
		{
			Audio.Play(Footstep, transform.position);
		}

		[Command("kill")]
		protected void Kill_Cmd()
		{
			Health.Kill();
		}
	}
}
