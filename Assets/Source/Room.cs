using Quinn.DamageSystem;
using Quinn.PlayerSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Quinn
{
	public class Room : MonoBehaviour
	{
		[SerializeField]
		private List<Health> OpenUponDeath;
		[SerializeField]
		private Door[] Doors;

		public bool IsLocked { get; private set; }

		private void Awake()
		{
			foreach (var hp in OpenUponDeath)
			{
				hp.OnDeath += () =>
				{
					if (OpenUponDeath.Count > 0)
					{
						OpenUponDeath.Remove(hp);

						if (OpenUponDeath.Count == 0)
						{
							Open();
							Player.Instance.Health.HealFully();
						}
					}
				};
			}
		}

		public void Open()
		{
			if (IsLocked)
				return;

			IsLocked = true;

			foreach (var door in Doors)
			{
				door.Open();
			}
		}

		public void Close()
		{
			if (!IsLocked)
				return;

			IsLocked = false;

			foreach (var door in Doors)
			{
				door.Close();
			}
		}
	}
}
