using Quinn.DamageSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.MissileSystem
{
	/// <summary>
	/// Spawns missiles in a repeating fashion in the world.<br/>
	/// This is not meant for spawning missiles by code. For that use the <see cref="MissileManager"/> static class.
	/// </summary>
	public class MissileSpawner : MonoBehaviour
	{
		[SerializeField, Required]
		private Transform Origin;

		[Space]

		[SerializeField]
		private GameObject Prefab;
		[SerializeField]
		private MissileSpawnBehavior SpawnBehavior;

		[Space]

		[SerializeField]
		private Team Team = Team.Environment;

		[Space]

		[SerializeField]
		private Vector3 Direction = Vector3.forward;
		[SerializeField]
		private float Cooldown = 2f;

		private float _nextSpawnTime;

		private void Update()
		{
			if (Time.time >= _nextSpawnTime)
			{
				_nextSpawnTime = Time.time + Cooldown + (SpawnBehavior.Interval * SpawnBehavior.Count) + SpawnBehavior.Delay;
				MissileManager.SpawnMissile(gameObject, Prefab, Team, Origin.position, SpawnBehavior, Direction.normalized);
			}
		}
	}
}
