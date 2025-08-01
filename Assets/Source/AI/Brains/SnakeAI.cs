using UnityEngine;

namespace Quinn.AI.Brains
{
	public class SnakeAI : AgentAI
	{
		[SerializeField]
		private float ChaseSpeed = 500f;
		[SerializeField]
		private float SinFrequency = 1f;

		[Space, SerializeField]
		private Collider2D[] Colliders;

		private void Start()
		{
			ShowBossUI();

			foreach (var collider in Colliders)
			{
				foreach (var other in Colliders)
				{
					if (collider != other)
					{
						Physics2D.IgnoreCollision(collider, other);
					}
				}
			}
		}

		protected override void OnThink()
		{
			var tangentDir = DirToPlayerCenter.Perpendicular1();
			var dir = Vector2.LerpUnclamped(DirToPlayerCenter, tangentDir, Mathf.Sin(Time.time * Mathf.PI * SinFrequency));

			Draw.Line(transform.position, (Vector2)transform.position + dir, Color.yellow);

			Push(ChaseSpeed * Time.deltaTime * dir);
			FacePlayer();
		}
	}
}
