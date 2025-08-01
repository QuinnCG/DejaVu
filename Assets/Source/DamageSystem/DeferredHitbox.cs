using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.DamageSystem
{
	/// <summary>
	/// Defers all damage to a health script, that may not be on this game object.
	/// </summary>
	public class DeferredHitbox : MonoBehaviour, IDamageable, ITeam
	{
		[SerializeField, Required]
		private Health Health;

		public Team Team => Health.Team;

		public bool TakeDamage(DamageInfo info)
		{
			return Health.TakeDamage(info);
		}
	}
}
