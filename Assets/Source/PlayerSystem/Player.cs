using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Quinn.PlayerSystem
{
	public class Player : MonoBehaviour
	{
		[SerializeField]
		private float MinAccelerationRate = 2f, MaxAccelerationRate = 16f;
		[SerializeField]
		private float LinearDrag = 5f;

		[Space]

		[SerializeField]
		private float AccelerationFactorWhileGrabbing = 1f;
		[SerializeField]
		private float DragFactorWhileGrabbing = 1f;

		[Space]

		[SerializeField]
		private float DashForce = 12f;

		[Space]

		[SerializeField, Required, ChildGameObjectsOnly]
		private Transform CameraTarget;
		[SerializeField]
		private float CamTargetPlayerToCursorBias = 0.3f;

		private Rigidbody2D _rb;
		private Grabber _grabber;

		private void Awake()
		{
			_rb = GetComponent<Rigidbody2D>();
			_grabber = GetComponent<Grabber>();
		}

		private async void Start()
		{
			await TransitionManager.Instance.FadeFromBlackAsync(1f);
		}

		private void Update()
		{
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
		}

		private void UpdateMovement(Vector2 moveDir)
		{
			Vector2 a1 = moveDir;
			Vector2 a2 = _rb.linearVelocity.normalized;

			float aDelta = Vector2.Angle(a1, a2);
			float norm = Mathf.Clamp01(aDelta / 180f);

			float force = Mathf.Lerp(MinAccelerationRate, MaxAccelerationRate, norm);
			force *= _grabber.IsGrabbing ? AccelerationFactorWhileGrabbing : 1f;

			_rb.AddForce(force * moveDir);
			_rb.linearDamping = LinearDrag * (_grabber.IsGrabbing ? DragFactorWhileGrabbing : 1f);
		}

		private void UpdateDash(Vector2 moveDir)
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				Cooldown.Call(this, 0.5f, () =>
				{
					_rb.AddForce(moveDir * DashForce, ForceMode2D.Impulse);
				});
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
	}
}
