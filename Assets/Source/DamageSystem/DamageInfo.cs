using UnityEngine;

namespace Quinn.DamageSystem
{
	[System.Serializable]
	public record DamageInfo
	{
		public float Damage = 1f;
		public StatusEffect StatusEffects;
		/// <summary>
		/// The associated team of the damage source. If an enemy's attack created this damage instance, then the team would be <see cref="Team.Hostile"/>.
		/// </summary>
		public Team Team;

		/// <summary>
		/// The original owner of the damage source; e.g. the enemy that shot the missile.
		/// </summary>
		public GameObject Owner;
		/// <summary>
		/// The damage source itself; e.g. the missile shot by an enemy.
		/// </summary>
		public GameObject Source;

		/// <summary>
		/// The logical direction of the damage, or (0, 0) if it doesn't have one.<br/>
		/// This is mainly used by the knockback system.
		/// </summary>
		public Vector2 Direction;
		public float Knockback;

		public DamageInfo() { }
		public DamageInfo(float damage, StatusEffect status = StatusEffect.None)
		{
			Damage = damage;
			StatusEffects = status;
		}
	}
}
