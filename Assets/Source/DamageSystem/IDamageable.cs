using UnityEngine;

namespace Quinn.DamageSystem
{
	public interface IDamageable
	{
		/// <summary>
		/// Conditionally, apply damage to this object.
		/// </summary>
		/// <param name="info">The payload containing all relavant damage information for the proper application of said damage.</param>
		/// <returns>True, if the damage was successfully applied. False, if it failed to be applied (e.g. same team).</returns>
		public bool TakeDamage(DamageInfo info);
		/// <inheritdoc cref="TakeDamage(DamageInfo)"/>
		public bool TakeDamage(float damage, GameObject ownerAndSource, Vector2 direction = default, Team team = Team.Hostile, StatusEffect status = StatusEffect.None)
		{
			return TakeDamage(new DamageInfo()
			{
				Damage = damage,
				Owner = ownerAndSource,
				Source = ownerAndSource,
				Direction = direction.normalized,
				StatusEffects = status,
				Team = team
			});
		}
	}
}
