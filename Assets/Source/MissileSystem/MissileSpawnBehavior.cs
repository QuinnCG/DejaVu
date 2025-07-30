using UnityEngine;

namespace Quinn.MissileSystem
{
	[System.Serializable]
	public record MissileSpawnBehavior
	{
		public int Count = 1;

		[Space]

		public float Interval = 0f;
		public float Delay = 0f;

		[Space]

		public MissileSpawnSpread SpreadType = MissileSpawnSpread.Direct;
		public float SpreadAngle = 15f;
	}
}
