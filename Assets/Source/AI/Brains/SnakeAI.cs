using DG.Tweening;
using Quinn.DamageSystem;
using System.Collections;
using UnityEngine;

namespace Quinn.AI.Brains
{
	public class SnakeAI : AgentAI
	{
		[SerializeField]
		private float ChaseSpeed = 500f;
		[SerializeField]
		private float SinFrequency = 1f;

		[Space, SerializeField]
		private float Damage = 1f;
		[SerializeField]
		private float Knockback = 6f;

		[Space, SerializeField]
		private Collider2D[] Colliders;

		private bool _doneBigAttack;

		private IEnumerator Start()
		{
			DoesThink = false;
			yield return new WaitUntil(() => DstToPlayer < 4f || HealthNorm < 1f);
			DoesThink = true;

			ShowBossUI();

			foreach (var collider in Colliders)
			{
				foreach (var other in Colliders)
				{
					if (collider != other)
					{
						Physics2D.IgnoreCollision(collider, other);
					}
				}
			}
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (collision.collider.IsPlayer())
			{
				ApplyDamage(collision.collider.GetComponent<IDamageable>(), Damage, Knockback * transform.position.DirectionTo(collision.collider.bounds.center));
			}
		}

		protected override void OnThink()
		{
			var tangentDir = DirToPlayerCenter.Perpendicular1();
			var dir = Vector2.LerpUnclamped(DirToPlayerCenter, tangentDir, Mathf.Sin(Time.time * Mathf.PI * SinFrequency));

			Draw.Line(transform.position, (Vector2)transform.position + dir, Color.yellow);

			Push(ChaseSpeed * Time.deltaTime * dir);
			FacePlayer();
		}

		protected override void OnDamaged(DamageInstance info)
		{
			if (HealthNorm < 0.4f && !_doneBigAttack)
			{
				_doneBigAttack = true;
				Rigidbody.linearVelocity = Vector2.zero;
				StartCoroutine(BigAttack());
			}
		}

		private IEnumerator BigAttack()
		{
			yield return transform.DOMove(PlayerPos, 0.5f).SetEase(Ease.OutCubic);

			var pHealth = Player.Health;
			ApplyDamage(pHealth, Mathf.Max(0f, pHealth.Current - 1f), DirToPlayerCenter * 32f);

			Player.SetSpritesRed();
		}
	}
}
