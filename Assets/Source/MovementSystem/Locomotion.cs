using System.Collections.Generic;
using UnityEngine;

namespace Quinn.MovementSystem
{
	[RequireComponent(typeof(Rigidbody2D))]
	public abstract class Locomotion : MonoBehaviour
	{
		// Used for things like knockback.
		class DecayingVelocity
		{
			public Vector2 Velocity;
			public float DecayRate;
		}

		public bool HasAnyVelocity => Velocity.sqrMagnitude > 0f;
		public Vector2 Velocity
		{
			get
			{
				if (_overrideVelocity.HasValue)
				{
					return _overrideVelocity.Value;
				}
				else
				{
					return _cumulativeVelocity;
				}
			}
		}

		private Rigidbody2D _rb;

		private readonly HashSet<DecayingVelocity> _decayingVelocities = new();
		private Vector2 _cumulativeVelocity;
		private Vector2? _overrideVelocity;

		protected virtual void Awake()
		{
			_rb = GetComponent<Rigidbody2D>();
		}

		protected virtual void LateUpdate()
		{
			_cumulativeVelocity += ProcessDecayingVelocities();

			Vector2 finalVel;

			if (_overrideVelocity.HasValue)
			{
				finalVel = _overrideVelocity.Value;
			}
			else
			{
				finalVel = _cumulativeVelocity;
			}

			_rb.linearVelocity = finalVel;

			_overrideVelocity = null;
			ResetVelocity();
		}

		/// <summary>
		/// Additive <c>direction * speed</c> that is reset by the end of the frame.
		/// </summary>
		protected void AddVelocity(Vector2 velocity)
		{
			_cumulativeVelocity += velocity;
		}

		protected void SetVelocity(Vector2 velocity)
		{
			_overrideVelocity = velocity;
		}

		/// <summary>
		/// This can be used for things like knockback.
		/// </summary>
		/// <param name="velocity">The initial speed and direction.</param>
		/// <param name="decayRate">How much speed should be subtracted every second.</param>
		protected void AddDecayingVelocity(Vector2 velocity, float decayRate)
		{
			_decayingVelocities.Add(new DecayingVelocity()
			{
				Velocity = velocity,
				DecayRate = decayRate
			});
		}

		/// <summary>
		/// Resets cumulative velocity to zero.
		/// </summary>
		protected void ResetVelocity(bool removeDecayingVelocities = false)
		{
			_cumulativeVelocity = Vector2.zero;

			if (removeDecayingVelocities)
			{
				_decayingVelocities.Clear();
			}
		}

		// Returns the sum of all decaying velocities, then decays them.
		private Vector2 ProcessDecayingVelocities()
		{
			var sum = Vector2.zero;
			var toRemove = new List<DecayingVelocity>();

			foreach (var handle in _decayingVelocities)
			{
				sum += handle.Velocity;

				float mag = handle.Velocity.magnitude;
				mag -= Time.deltaTime * handle.DecayRate;
				mag.MakeAtLeast(0f);

				handle.Velocity = handle.Velocity.normalized * mag;

				if (handle.Velocity.sqrMagnitude < 0.01f)
				{
					toRemove.Add(handle);
				}
			}

			_decayingVelocities.RemoveRange(toRemove);
			return sum;
		}
	}
}
