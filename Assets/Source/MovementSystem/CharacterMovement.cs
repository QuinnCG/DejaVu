using System.Collections.Generic;
using UnityEngine;

namespace Quinn.MovementSystem
{
	/// <summary>
	/// Supports things like slowed movement from a status effect.
	/// </summary>
	public class CharacterMovement : Locomotion
	{
		public float MoveSpeed { get; protected set; }
		/// <summary>
		/// The move speed, accounting for any factors from status effects or the like.
		/// </summary>
		public float FinalMoveSpeed => SpeedFactor * MoveSpeed;
		public float SpeedFactor
		{
			get
			{
				float speed = 1f;

				foreach (var factor in _moveSpeedFactors)
				{
					speed *= factor.Factor;
				}

				return speed;
			}
		}

		private readonly HashSet<SpeedFactor> _moveSpeedFactors = new();

		public SpeedFactor CreateMoveSpeedFactor()
		{
			var factor = new SpeedFactor();
			_moveSpeedFactors.Add(factor);

			return factor;
		}

		public void DestroyMoveSpeedFactor(SpeedFactor factor)
		{
			_moveSpeedFactors.Remove(factor);
		}

		/// <summary>
		/// Applies a velocity that decays over time by <paramref name="decayRate"/>.
		/// </summary>
		/// <param name="velocity">The initial speed and direction.</param>
		/// <param name="decayRate">How much speed should be subtracted every second.</param>
		/// <param name="overrideVelocity">If true, the velocity will be reset before applying this value.</param>
		public void ApplyKnockback(Vector2 velocity, float decayRate, bool overrideVelocity = false)
		{
			if (overrideVelocity)
			{
				CeaseMomentum();
			}

			AddDecayingVelocity(velocity, decayRate);
		}

		public void CeaseMomentum()
		{
			ResetVelocity(true);
		}
	}
}
