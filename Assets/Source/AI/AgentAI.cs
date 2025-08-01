using Quinn.DamageSystem;
using Quinn.PlayerSystem;
using Quinn.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.AI
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(Health))]
	public abstract class AgentAI : MonoBehaviour
	{
		[SerializeField, Required, BoxGroup("AgentAI")]
		private Transform Root;

		[Space]

		[SerializeField, BoxGroup("AgentAI")]
		private bool IsBoss;
		[SerializeField, BoxGroup("AgentAI"), EnableIf(nameof(IsBoss))]
		private string BossTitle = "Boss Title";

		public Animator Animator { get; private set; }
		protected Collider2D Collider { get; private set; }
		protected Rigidbody2D Rigidbody { get; private set; }
		protected Health Health { get; private set; }
		protected float HealthNorm => Health.Normalized;

		protected Player Player => Player.Instance;
		protected Vector2 PlayerPos => Player.transform.position;
		protected Vector2 DirToPlayer => transform.position.DirectionTo(PlayerPos);
		protected Vector2 DirToPlayerCenter => transform.position.DirectionTo(Player.Collider.bounds.center);
		protected float DstToPlayer => transform.position.DistanceTo(PlayerPos);
		protected float PlayerHealthNorm => Player.Health.Normalized;
		protected bool IsPlayerAlive => Player.Health.IsAlive;
		protected bool IsPlayerDeath => Player.Health.IsDead;

		public bool DoesThink { get; set; } = true;

		protected virtual void Awake()
		{
			Animator = GetComponent<Animator>();
			Collider = GetComponent<Collider2D>();
			Rigidbody = GetComponent<Rigidbody2D>();
			Health = GetComponent<Health>();

			Health.OnDeath += OnDeath;
		}

		protected virtual void Update()
		{
			if (DoesThink)
			{
				OnThink();
			}
		}

		protected virtual void OnThink() { }

		protected bool HasLineOfSightToPlayer()
		{
			var hit = Physics2D.Linecast(Collider.bounds.center, Player.Collider.bounds.center, LayerMask.GetMask("Obstacle"));
			return !hit.IsValid();
		}

		protected void Push(Vector2 velocity)
		{
			Rigidbody.AddForce(velocity);
		}

		protected void PushImpulse(Vector2 velocity)
		{
			Rigidbody.AddForce(velocity, ForceMode2D.Impulse);
		}

		protected void SetDrag(float drag)
		{
			Rigidbody.linearDamping = drag;
		}

		protected virtual void OnDeath()
		{
			gameObject.Destroy();
		}

		protected void FaceDirection(float xDir)
		{
			if (xDir != 0f)
			{
				var scale = Root.localScale;
				scale.x = Mathf.Abs(scale.x) * Mathf.Sign(xDir);
				Root.localScale = scale;
			}
		}
		protected void FacePlayer()
		{
			FaceDirection(DirToPlayer.x);
		}
		protected void FaceAwayFromPlayer()
		{
			FaceDirection(-DirToPlayer.x);
		}

		protected void ShowBossUI()
		{
			if (IsBoss)
			{
				BossUI.Instance.SetBoss(BossTitle, Health);
			}
		}
	}
}
