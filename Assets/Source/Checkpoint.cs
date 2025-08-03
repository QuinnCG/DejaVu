using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Quinn
{
	public class Checkpoint : MonoBehaviour
	{
		public static string ActiveCheckpoint { get; private set; }
		private static readonly Dictionary<string, Checkpoint> _spawnedCheckpoints = new();

		[field: SerializeField, Required]
		public Transform SpawnPoint { get; private set; }
		[SerializeField]
		private bool IsStartingCheckpoint;
		[SerializeField, ReadOnly]
		private string GUID;

		[Button("Reset")]
		protected void ResetGUID()
		{
			GUID = string.Empty;
			OnValidate();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void StaticReset()
		{
			ActiveCheckpoint = null;
			_spawnedCheckpoints.Clear();
		}

		private void OnValidate()
		{
			if (!EditorUtility.InPrefabMode() && string.IsNullOrWhiteSpace(GUID))
			{
				GUID = System.Guid.NewGuid().ToString();
			}
		}

		private void Awake()
		{
			_spawnedCheckpoints.TryAdd(GUID, this);
		}

		private void OnDestroy()
		{
			_spawnedCheckpoints.Remove(GUID);
		}

		public static Checkpoint GetCheckpoint(string guid)
		{
			if (_spawnedCheckpoints.TryGetValue(guid, out Checkpoint checkpoint))
			{
				return checkpoint;
			}
			else
			{
				Log.Error($"'{_spawnedCheckpoints[guid].gameObject.name}' has a another checkpoint with the same GUID!");
			}

			return null;
		}

		public static Checkpoint GetStartingCheckpoint()
		{
			foreach (var checkpoint in _spawnedCheckpoints.Values)
			{
				if (checkpoint.IsStartingCheckpoint)
				{
					return checkpoint;
				}
			}

			return null;
		}

		public static void ClearActive() => ActiveCheckpoint = null;

		public void SetActiveCheckpoint()
		{
			ActiveCheckpoint = GUID;
		}
	}
}
