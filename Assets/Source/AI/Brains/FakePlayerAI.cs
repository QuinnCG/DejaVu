using Quinn.PlayerSystem;
using System.Collections;
using UnityEngine;

namespace Quinn.AI.Brains
{
	public class FakePlayerAI : AgentAI
	{
		[SerializeField]
		private Grabber Grabber;
		[SerializeField]
		private StaffOrbiter Staff;

		[Space, SerializeField]
		private float PunchCooldown = 3f;

		private float _nextPunchTime;

		private IEnumerator Start()
		{
			DoesThink = false;
			yield return new WaitUntil(() => DstToPlayer < 10f);
			DoesThink = true;

			Player.IsFinalDeath = true;

			_nextPunchTime = Time.time + PunchCooldown;
		}

		protected override void OnThink()
		{
			Staff.SetManualDirection(DirToPlayerCenter);
			FacePlayer();

			Push(100f * Time.deltaTime * DirToPlayer);

			if (Time.time > _nextPunchTime)
			{
				_nextPunchTime = Time.time + PunchCooldown;
				Grabber.Punch(PlayerPos);
			}
		}
	}
}
