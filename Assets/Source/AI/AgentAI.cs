using Quinn.DamageSystem;
using Quinn.PlayerSystem;
using UnityEngine;

namespace Quinn.AI
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(Health))]
	public class AgentAI : MonoBehaviour
	{
		public Animator Animator { get; private set; }
		protected Collider2D Collider { get; private set; }
		protected Rigidbody2D Rigidbody { get; private set; }
		protected Health Health { get; private set; }
		protected float HealthNorm => Health.Normalized;

		protected Player Player => Player.Instance;
		protected Vector2 PlayerPos => Player.transform.position;
		protected Vector2 DirToPlayer => transform.position.DirectionTo(PlayerPos);
		protected float DstToPlayer => transform.position.DistanceTo(PlayerPos);
		protected float PlayerHealthNorm => Player.Health.Normalized;
		protected bool IsPlayerAlive => Player.Health.IsAlive;
		protected bool IsPlayerDeath => Player.Health.IsDead;

		protected virtual void Awake()
		{
			Animator = GetComponent<Animator>();
			Collider = GetComponent<Collider2D>();
			Rigidbody = GetComponent<Rigidbody2D>();
			Health = GetComponent<Health>();
		}

		protected virtual void Update()
		{
			OnThink();
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
	}
}
