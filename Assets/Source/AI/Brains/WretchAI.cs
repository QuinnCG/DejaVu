using Quinn.DamageSystem;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Quinn.AI.Brains
{
	public class WretchAI : AgentAI
	{
		[SerializeField]
		private float EngageDst = 5f;
		[SerializeField]
		private float ChaseSpeed = 500f;
		[SerializeField]
		private float Damage = 2f, Knockback = 12f;

		[Space, SerializeField]
		private float SpeakDst = 7f;
		[SerializeField, Required]
		private Speaker Speaker;
		[SerializeField, Multiline]
		private string[] OpeningDialogue;

		private bool _takenAnyDamage;

		private IEnumerator Start()
		{
			Animator.SetBool("IsInjured", true);
			Health.OnDamage += _ => _takenAnyDamage = true;

			DoesThink = false;
			yield return new WaitUntil(() => DstToPlayer < SpeakDst);
			float speakEndTime = Time.time + Speaker.Speak(OpeningDialogue);

			yield return new WaitUntil(() => DstToPlayer < EngageDst || _takenAnyDamage || Time.time > speakEndTime);
			ShowBossUI();
			DoesThink = true;
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (collision.collider.IsPlayer())
			{
				ApplyDamage(collision.collider.GetComponent<IDamageable>(), Damage, Knockback * transform.position.DirectionTo(collision.collider.bounds.center));
			}
		}

		private void LateUpdate()
		{
			Animator.SetBool("IsMoving", Rigidbody.linearVelocity.sqrMagnitude > 0f);
		}

		protected override void OnThink()
		{
			FacePlayer();
			Push(ChaseSpeed * Time.deltaTime * DirToPlayer);
		}
	}
}
