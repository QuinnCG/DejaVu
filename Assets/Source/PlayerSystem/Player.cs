using UnityEngine;

namespace Quinn.PlayerSystem
{
	public class Player : MonoBehaviour
	{
		[SerializeField]
		private float AccelerationRate = 10f;
		[SerializeField]
		private float DashForce = 12f;

		private Rigidbody2D _rb;
		private Grabber _grabber;

		private void Awake()
		{
			_rb = GetComponent<Rigidbody2D>();
			_grabber = GetComponent<Grabber>();
		}

		private void Update()
		{
			Vector2 moveDir = new Vector2()
			{
				x = Input.GetAxis("Horizontal"),
				y = Input.GetAxis("Vertical")
			}.normalized;

			_rb.AddForce(moveDir * AccelerationRate);

			if (Input.GetKeyDown(KeyCode.Space))
			{
				Cooldown.Call(this, 0.5f, () =>
				{
					_rb.AddForce(moveDir * DashForce, ForceMode2D.Impulse);
				});
			}

			if (Input.GetMouseButtonDown(1))
			{
				_grabber.Grab();
			}
			else if (Input.GetMouseButtonUp(1))
			{
				_grabber.Release();
			}
		}
	}
}
