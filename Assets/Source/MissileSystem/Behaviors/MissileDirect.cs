using UnityEngine;

namespace Quinn.MissileSystem.Behaviors
{
	public class MissileDirect : MissileBehavior
	{
		[SerializeField]
		private float Speed = 8f;

		public override void OnUpdate()
		{
			Missile.AddVelocity(Speed * Missile.BaseDirection);
		}
	}
}
