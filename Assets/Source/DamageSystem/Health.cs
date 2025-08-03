using DG.Tweening;
using FMODUnity;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quinn.DamageSystem
{

	public class Health : MonoBehaviour, IDamageable, ITeam
	{
		[SerializeField, Tooltip("The starting HP.")]
		private float Default = 20f;
		[field: SerializeField]
		public Team Team { get; private set; } = Team.Hostile;
		[SerializeField, Unit(Units.Second), Tooltip("How long to be immune from damage for after taking damage. 0 will disable this feature.")]
		private float ImmunityOnHurt = 0f;

		[Space]

		[SerializeField]
		private bool FlashOnHurt;
		[SerializeField, ShowIf(nameof(FlashOnHurt))]
		private float FlashDuration = 0.2f;
		[SerializeField, ShowIf(nameof(FlashOnHurt))]
		private Ease FlashInEase = Ease.Linear, FlashOutEase = Ease.Linear;

		[Space]

		[SerializeField]
		private bool BlinkOnHurt;
		[SerializeField, ShowIf(nameof(BlinkOnHurt))]
		private int BlinkCount = 3;
		[SerializeField, ShowIf(nameof(BlinkOnHurt))]
		private float BlinkDelay = 0.1f, BlinkDuration = 0.1f;

		[Space]

		[SerializeField, EnableIf("@FlashOnHurt || BlinkOnHurt")]
		private SpriteRenderer[] Renderers;

		[SerializeField, FoldoutGroup("SFX")]
		private EventReference HurtSound, DeathSound;
		[SerializeField, FoldoutGroup("SFX")]
		private bool PlayHurtOnDeathToo = false;

		[ShowInInspector, ReadOnly]
		public float Current { get; private set; } = float.NaN;
		[ShowInInspector, ReadOnly]
		public float Max { get; private set; } = float.NaN;
		/// <summary>
		/// HP normalized.
		/// </summary>
		[ShowInInspector, ReadOnly]
		public float Normalized => Current / Max;

		public bool IsDead { get; private set; }
		public bool IsAlive => !IsDead;
		/// <summary>
		/// Damage can be taken, but death never occurs and health never drains.
		/// </summary>
		[field: SerializeField]
		public bool InfiniteHP { get; set; }

		public event System.Action<DamageInstance> OnDamage;
		public event System.Action<float> OnHeal;
		public event System.Action OnDeath;
		/// <summary>
		/// Called when healed or damaged. Passes the final HP delta of the change.
		/// </summary>
		public event System.Action<float> OnHealthChanged;

		/// <summary>
		/// Is damage being blocked for this damageable?
		/// </summary>
		public bool IsImmune => _dmgBlockers.Count > 0;

		private readonly HashSet<object> _dmgBlockers = new();
		private readonly object _hitImmunityKey = new();
		private float _nextImmunityEndTime;

		// May be null.
		private Rigidbody2D _rb;

		private void Awake()
		{
			Current = Max = Default;

			TryGetComponent(out _rb);
		}

		private void FixedUpdate()
		{
			if (ImmunityOnHurt > 0f && Time.time > _nextImmunityEndTime)
			{
				UnblockDamage(_hitImmunityKey);
			}
		}

		private void OnValidate()
		{
			if (Renderers == null || Renderers.Length == 0)
			{
				return;
			}

			if (FlashOnHurt)
			{
				foreach (var renderer in Renderers)
				{
					if (!renderer.sharedMaterial.HasFloat("_Flash"))
					{
						Debug.LogWarning($"{renderer.gameObject.name} has a SpriteRenderer whose material is lacking '_Flash'!");
					}
				}
			}
		}

		public bool TakeDamage(DamageInfo info)
		{
			if (CanTakeDamage(info))
			{
				float actualDmgApplied = Mathf.Min(Current, info.Damage);
				Current = Mathf.Max(Current - info.Damage, 0f);

				if (InfiniteHP)
				{
					Current = Max;
				}

				OnDamage?.Invoke(new DamageInstance()
				{
					OriginalInfo = info,
					RealDamage = actualDmgApplied,
					IsLethal = Current == 0f
				});

				OnHealthChanged?.Invoke(actualDmgApplied);

				// Death.
				if (Current <= 0f)
				{
					IsDead = true;
					OnDeath?.Invoke();

					if (PlayHurtOnDeathToo)
					{
						Audio.Play(HurtSound, transform.position);
					}

					Audio.Play(DeathSound, transform.position);
				}
				// Not dead.
				else
				{
					Audio.Play(HurtSound, transform.position);
				}

				if (ImmunityOnHurt > 0f)
				{
					_nextImmunityEndTime = Time.time + ImmunityOnHurt;
					BlockDamage(_hitImmunityKey);
				}

				// HACK: Reference the individual coroutines and stop them each, instead of stopping all coroutines.
				StopAllCoroutines();

				if (FlashOnHurt)
				{
					StartCoroutine(FlashSequence());
				}

				if (BlinkOnHurt)
				{
					StartCoroutine(BlinkSequence());
				}

				if (_rb != null)
				{
					_rb.AddForce(info.Direction * info.Knockback, ForceMode2D.Impulse);
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		public void Kill()
		{
			TakeDamage(new DamageInfo()
			{
				Damage = Max + 1f,
				Source = gameObject,
				Team = Team == Team.Environment ? Team.Hostile : Team.Environment,
				Owner = gameObject,
			});
		}

		public bool CanTakeDamage(DamageInfo info)
		{
			return IsAlive
				&& info.Team != Team
				&& !IsImmune;
		}

		public void Heal(float amount)
		{
			// The amount to heal or the amount we can heal by, which ever is small.
			float actualAmountHealed = Mathf.Min(Max - Current, amount);
			Current = Mathf.Min(Current + amount, Max);

			OnHeal?.Invoke(actualAmountHealed);
			OnHealthChanged?.Invoke(actualAmountHealed);
		}

		public void HealFully()
		{
			float actualAmountHealed = Max - Current;
			Current = Max;

			OnHeal?.Invoke(actualAmountHealed);
			OnHealthChanged?.Invoke(actualAmountHealed);
		}

		public void BlockDamage(object key)
		{
			_dmgBlockers.Add(key);
		}

		public void UnblockDamage(object key)
		{
			_dmgBlockers.Remove(key);
		}

		private IEnumerator FlashSequence()
		{
			float halfDur = FlashDuration / 2f;

			foreach (var renderer in Renderers)
			{
				renderer.material.DOFloat(1f, "_Flash", halfDur)
					.SetEase(FlashInEase);
			}

			yield return new WaitForSeconds(halfDur);

			foreach (var renderer in Renderers)
			{
				renderer.material.DOFloat(0f, "_Flash", halfDur)
					.SetEase(FlashOutEase);
			}
		}

		private IEnumerator BlinkSequence()
		{
			for (int i = 0; i < BlinkCount; i++)
			{
				yield return new WaitForSeconds(BlinkDelay);

				foreach (var renderer in Renderers)
				{
					renderer.enabled = false;
				}

				yield return new WaitForSeconds(BlinkDuration);

				foreach (var renderer in Renderers)
				{
					renderer.enabled = true;
				}
			}
		}
	}
}
