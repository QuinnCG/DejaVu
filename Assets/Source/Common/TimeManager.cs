using System.Collections.Generic;
using QFSW.QC;
using UnityEngine;

namespace Quinn
{
	public class TimeManager : MonoBehaviour
	{
		private static TimeManager _instance;
		private readonly Dictionary<object, float> _factors = new();

		// The key used when setting the time scale via commands.
		private readonly object _cmdKey = new();

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void StaticReset()
		{
			_instance = null;
		}

		public static float GetFactor(object key)
		{
			if (_instance._factors.TryGetValue(key, out float value))
			{
				return value;
			}

			return 1f;
		}

		/// <summary>
		/// Add or update a factor.
		/// </summary>
		/// <param name="key">The reference value used to identify this factor.</param>
		/// <param name="factor">The multiplier for time-scale. This is applied along with any other registered factors.</param>
		public static void ApplyFactor(object key, float factor)
		{
			var factors = _instance._factors;

			if (factors.ContainsKey(key))
			{
				factors[key] = factor;
			}
			else
			{
				factors.Add(key, factor);
			}

			_instance.UpdateScale();
		}

		public static void RemoveFactor(object key)
		{
			_instance._factors.Remove(key);
			_instance.UpdateScale();
		}

		public static bool HasKey(object key)
		{
			return _instance._factors.ContainsKey(key);
		}

		private void Start()
		{
			_instance = this;
		}

		private void UpdateScale()
		{
			float factor = 1f;

			foreach (var f in _factors.Values)
			{
				factor *= f;
			}

			Time.timeScale = factor;
		}

		[Command("time", "Get information on the global time scale.")]
		protected void Time_Cmd()
		{
			Debug.Log($"Time scale: {Time.timeScale}. Number of registered factors: {_factors.Count}.");
		}
		[Command("time", "Apply a global time factor to 'Time.timeScale'.")]
		protected void Time_Cmd(float factor)
		{
			if (factor == 1f)
			{
				RemoveFactor(_cmdKey);
			}
			else
			{
				ApplyFactor(_cmdKey, factor);
			}

			Debug.Log($"Applied time scale factor {factor}. Final time scale: {Time.timeScale}.");
		}

		[Command("pause", "Set time scale to 0.")]
		protected void Pause_Cmd()
		{
			ApplyFactor(_cmdKey, 0f);
			Debug.Log("Time is paused.");
		}

		[Command("play", "Set time scale to 1. There may be other time scale factors overriding this.")]
		protected void Play_Cmd()
		{
			RemoveFactor(_cmdKey);
			Debug.Log("The console is no longer influencing time. There may be other registered time scale factors that are keeping time paused or slowed.");
		}

		[Command("p", "Toggles between pause and play.")]
		protected void TogglePlay_Cmd()
		{
			if (HasKey(_cmdKey))
			{
				Play_Cmd();
			}
			else
			{
				Pause_Cmd();
			}
		}
	}
}
