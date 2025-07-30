using Quinn.DamageSystem;
using UnityEngine;

namespace Quinn.MissileSystem
{
	/// <summary>
	/// Handles spawning missiles with various patterns.
	/// </summary>
	public static class MissileManager
	{
		/// <summary>
		/// Spawn one or more missiles with a pattern.
		/// </summary>
		/// <param name="source">The owner of the missile. If the player shot the missile, this is the player. This is used mainly for damage filtering purposes.</param>
		/// <param name="prefab">The prefab of the missile. The root of the prefab must have the <see cref="Missile"/> component.</param>
		/// <param name="origin">The 3D position to spawn the missile at.</param>
		/// <param name="spawnBehavior">The data describing the pattern to spawn the missile(s) in as well as the number of missiles.</param>
		/// <param name="baseDir">The basic direction used by the missile. Missile behaviors may interpret this and apply various forces as they see fit.</param>
		/// <param name="onMissileSpawnCallback">A callback called after the missile is spawned but before <see cref="Missile.Launch(Vector2, GameObject)"/> is called.</param>
		public static async void SpawnMissile(GameObject source, GameObject prefab, Team team, Vector2 origin, MissileSpawnBehavior spawnBehavior, Vector2 baseDir, System.Action<Missile> onMissileSpawnCallback = default)
		{
			if (spawnBehavior.Delay > 0f)
			{
				await Wait.Duration(spawnBehavior.Delay);
			}

			for (int i = 0; i < spawnBehavior.Count; i++)
			{
				var instance = prefab.Clone(origin);
				var missile = instance.GetComponent<Missile>();

				Vector2 finalDir = CalculateDir(i, baseDir, spawnBehavior);
				onMissileSpawnCallback?.Invoke(missile);
				missile.Launch(finalDir, source, team);

				if (spawnBehavior.Interval > 0f && i < spawnBehavior.Count - 1)
				{
					await Wait.Duration(spawnBehavior.Interval);
				}
			}
		}

		private static Vector2 CalculateDir(int index, Vector2 baseDir, MissileSpawnBehavior spawnBehavior)
		{
			float halfAngle = spawnBehavior.SpreadAngle / 2f;
			float angleDelta = spawnBehavior.SpreadAngle / Mathf.Max(1, spawnBehavior.Count - (spawnBehavior.Count.IsOdd() ? 1 : 0));

			switch (spawnBehavior.SpreadType)
			{
				case MissileSpawnSpread.Direct:
				{
					return baseDir;
				}
				case MissileSpawnSpread.SpreadRandom:
				{
					return Quaternion.Euler(0f, 0f, Random.Range(-halfAngle, halfAngle)) * baseDir;
				}
				case MissileSpawnSpread.SpreadEven:
				{
					return Quaternion.Euler(0f, 0f, (angleDelta * index) - halfAngle) * baseDir;
				}
				default:
				{
					Debug.LogWarning("No valid spread type found, returning a missile direction of (0, 0)!");
					return default;
				}
			}
		}
	}
}
