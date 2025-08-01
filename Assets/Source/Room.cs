using UnityEngine;

namespace Quinn
{
	public class Room : MonoBehaviour
	{
		[SerializeField]
		private Door[] Doors;

		public bool IsLocked { get; private set; }

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
