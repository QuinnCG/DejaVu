using UnityEngine;

namespace Quinn.MissileSystem.Behaviors
{
	public class MissileDeathExplosion : MissileBehavior
	{
		[SerializeField]
		private GameObject MissilePrefab;
		[SerializeField]
		private MissileSpawnBehavior SpawnBehavior;
		[SerializeField]
		private Vector3 BaseDirection = Vector3.forward;

		[Space, SerializeField, Tooltip("If the sub missiles are references the prefab that they were spawned from, it will never stop spawning until the scene reloads. This will stop that.")]
		private bool SupressDeathExplosionOnSpawnedMissiles;

		public override void OnDestroy()
		{
			MissileManager.SpawnMissile(Missile.Owner, MissilePrefab, Missile.Team, Missile.transform.position, SpawnBehavior, BaseDirection, OnMissileSpawn);
		}

		private void OnMissileSpawn(Missile missile)
		{
			if (SupressDeathExplosionOnSpawnedMissiles)
			{
				missile.RemoveBehavior<MissileDeathExplosion>();
			}
		}
	}
}
