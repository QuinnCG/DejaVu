using System.Linq;
using Quinn.DamageSystem;
using Quinn.MissileSystem.Behaviors;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

namespace Quinn.MissileSystem
{
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(Rigidbody2D))]
	public class Missile : MonoBehaviour
	{
		[SerializeField]
		private bool HasFiniteLifespan = true;
		[SerializeField, EnableIf(nameof(HasFiniteLifespan)), Unit(Units.Second)]
		private float Lifespan = 5f;
		[SerializeField, EnableIf(nameof(HasFiniteLifespan)), Tooltip("If this is false, then upon the lifespan expiring, the missile will just be deleted.")]
		private bool TriggerDeathBehaviorOnLifespanExpire = true;

		[Space]

		[SerializeField]
		private float Damage = 1f;
		[SerializeField]
		private float Knockback = 0f;
		[SerializeField]
		private StatusEffect StatusEffects = StatusEffect.None;

		[Space]

		[SerializeField]
		private bool DestroyOnHit = true;

		[Space]

		[SerializeReference]
		private MissileBehavior[] Behaviors = new MissileBehavior[]
		{
			new MissileDirect()
		};

		/// <summary>
		/// The owner of the missile. This is used primarily by the damage system when filtering for friendlies.
		/// </summary>
		public GameObject Owner { get; private set; }
		public Team Team { get; set; } = Team.Hostile;
		public Vector2 BaseDirection => _baseDir;
		/// <summary>
		/// Has <see cref="DestroyMissile(bool)"/> been called yet or not.
		/// </summary>
		public bool IsDestroyed { get; private set; }

		private Rigidbody2D _rb;
		// Note that this value is reset at the end of every frame.
		private Vector2 _velocity;

		private Vector2 _baseDir;
		private float _lifespanEndTime;

		private VisualEffect[] _vfx;

		private void OnValidate()
		{
			foreach (var vfx in GetComponentsInChildren<VisualEffect>())
			{
				vfx.initialEventName = string.Empty;
			}

			GetComponent<Collider2D>().isTrigger = true;
		}

		private void Awake()
		{
			_rb = GetComponent<Rigidbody2D>();

			foreach (var behavior in Behaviors)
			{
				behavior.Init(this);
				behavior.OnCreate();
			}

			_vfx = GetComponentsInChildren<VisualEffect>(true);
		}

		public void Launch(Vector2 baseDir, GameObject owner, Team team)
		{
			_baseDir = baseDir.normalized;
			_lifespanEndTime = Time.time + Lifespan;

			Owner = owner;
			Team = team;

			foreach (var vfx in _vfx)
			{
				vfx.SendEvent("OnPlay");
			}
		}

		private void Update()
		{
			foreach (var behavior in Behaviors)
			{
				behavior.OnUpdate();
			}

			if (HasFiniteLifespan && Time.time > _lifespanEndTime)
			{
				DestroyMissile(!TriggerDeathBehaviorOnLifespanExpire);
			}
		}

		private void LateUpdate()
		{
			_rb.linearVelocity = _velocity;
			_velocity = default;
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject != Owner)
			{
				if (other.TryGetComponent(out IDamageable damage))
				{
					bool success = damage.TakeDamage(new DamageInfo()
					{
						Owner = Owner,
						Source = gameObject,

						Damage = Damage,
						StatusEffects = StatusEffects,

						Direction = _rb.linearVelocity.normalized,
						Knockback = Knockback,
						Team = Team
					});

					// Only attempt destroying self if damage was actually applied.
					if (success && DestroyOnHit)
					{
						DestroyMissile();
					}
				}
				else if (other.gameObject.layer == LayerMask.NameToLayer(CollisionHelper.ObstacleName))
				{
					if (DestroyOnHit)
					{
						DestroyMissile();
					}
				}
			}
		}

		public void RemoveBehavior<T>() where T : MissileBehavior
		{
			// HACK: [Missile] There is definitely a better way to do this.

			foreach (var behavior in Behaviors)
			{
				if (behavior is T)
				{
					Behaviors = Behaviors.Where(b => b != behavior).ToArray();
					break;
				}
			}
		}

		/// <summary>
		/// Call this over the regular <see cref="Object.Destroy(Object)"/> method.
		/// </summary>
		/// <param name="supressBehaviorCallbacks">This should be true if you wish to avoid various VFX. For instance, if you are deleting a missile in an unloaded scene.</param>
		public void DestroyMissile(bool supressBehaviorCallbacks = false)
		{
			if (IsDestroyed)
			{
				return;
			}

			IsDestroyed = true;

			if (!supressBehaviorCallbacks)
			{
				foreach (var behavior in Behaviors)
				{
					behavior.OnDestroy();
				}
			}

			foreach (var vfx in _vfx)
			{
				if (vfx.gameObject == gameObject)
				{
					Debug.LogWarning("Cannot detach a VFX from a missile if that VFX component doesn't have its own game object!");
					continue;
				}

				vfx.SendEvent("OnStop");

				vfx.transform.SetParent(null);
				vfx.gameObject.AddComponent<DestroyVFXOnFinish>();
			}

			Destroy(gameObject);
		}

		/// <summary>
		/// Velocity is summed up for a single frame and reset by the end of the frame.
		/// </summary>
		public void AddVelocity(Vector2 velocity)
		{
			_velocity += velocity;
		}
	}
}
