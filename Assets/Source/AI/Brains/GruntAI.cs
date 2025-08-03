using FMODUnity;
using Quinn.DamageSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Quinn.AI.Brains
{
	public class GruntAI : AgentAI
	{
		[SerializeField]
		private float PatrolRadius = 8f;
		[SerializeField]
		private float PatrolAcceleration = 0.5f;
		[SerializeField]
		private float PatrolStoppingDst = 1f;
		[SerializeField]
		private Vector2 PatrolIdle = new(1f, 2f);

		[Space]

		[SerializeField]
		private float ChaseThreshold = 5f;
		[SerializeField]
		private float ChaseAcceleration = 1f;
		[SerializeField]
		private EventReference AlertSound;

		[Space]

		[SerializeField]
		private float LeapThreshold = 3f;
		[SerializeField]
		private float LeapForce = 8f;
		[SerializeField]
		private Vector2 LeapIdle = new(3f, 4f);
		[SerializeField]
		private EventReference LeapSound;
		[SerializeField]
		private float LeapHitboxDuration = 1f;
		[SerializeField]
		private float LeapDamage = 1f, LeapKnockback = 8f;

		private float _leapEndTime;

		private IEnumerator Start()
		{
			Vector2 patrolTarget = GetPatrolTarget();
			float nextPatrolTimeoutTime = Time.time + 8f;

			while (DstToPlayer > ChaseThreshold || !HasLineOfSightToPlayer())
			{
				if (transform.position.DistanceTo(patrolTarget) < PatrolStoppingDst || Time.time > nextPatrolTimeoutTime)
				{
					nextPatrolTimeoutTime = Time.time + 8f;
					patrolTarget = GetPatrolTarget();
					yield return new WaitForSeconds(PatrolIdle.GetRandom());
				}
				else
				{
					var dirToPatrolTarget = transform.position.DirectionTo(patrolTarget);

					FaceDirection(dirToPatrolTarget.x);
					Push(PatrolAcceleration * Time.deltaTime * dirToPatrolTarget);

					yield return null;
				}
			}

			ShowBossUI();

			yield return new WaitUntil(() => DstToPlayer < 4f || HealthNorm < 1f);

			Audio.Play(AlertSound, transform.position);
			FacePlayer();

			yield return new WaitForSeconds(1f);
			StartCoroutine(Chase());
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (Time.time < _leapEndTime && collision.collider.IsPlayer())
			{
				ApplyDamage(collision.collider.GetComponent<IDamageable>(), LeapDamage, LeapKnockback * transform.position.DirectionTo(collision.collider.bounds.center));
			}
		}

		private IEnumerator Chase()
		{
			while (true)
			{
				FacePlayer();

				if (DstToPlayer < LeapThreshold)
				{
					// TODO: Cast circle nearby us for duration in separate coroutine.

					PushImpulse(DirToPlayerCenter * LeapForce);
					Audio.Play(LeapSound, transform.position);

					_leapEndTime = Time.time + LeapHitboxDuration;
					yield return new WaitForSeconds(LeapIdle.GetRandom());
				}
				else
				{
					Push(ChaseAcceleration * Time.deltaTime * DirToPlayer);
					yield return null;
				}
			}
		}

		private Vector2 GetPatrolTarget()
		{
			for (int i = 0; i < 20; i++)
			{
				var pos = (Vector2)transform.position + (Random.insideUnitCircle * PatrolRadius);

				var hits = new List<RaycastHit2D>();
				Collider.Cast(transform.position.DirectionTo(pos), hits, transform.position.DistanceTo(pos));

				bool shouldContinue = false;

				foreach (var hit in hits)
				{
					if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
					{
						Draw.Sphere(pos, 0.2f, Color.red, 5f, true);
						shouldContinue = true;
						break;
					}
				}

				if (shouldContinue)
				{
					continue;
				}

				Draw.Sphere(pos, 0.2f, Color.green, 5f, true);
				return pos;
			}

			return transform.position;
		}
	}
}
