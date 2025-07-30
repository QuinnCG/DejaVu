using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quinn
{
	[InfoBox("Manages a convenient system for calling something on a cooldown when the input call may be occuring more frequently than desired.")]
	public class Cooldown : MonoBehaviour
	{
		private static Cooldown _instance;

		private readonly Dictionary<object, float> _cooldowns = new();

		private void Awake()
		{
			_instance = this;
		}

		private void LateUpdate()
		{
			var toRemove = new HashSet<object>();

			foreach (var entry in _cooldowns)
			{
				if (Time.time > entry.Value)
				{
					toRemove.Add(entry.Key);
				}
			}

			foreach (var item in toRemove)
			{
				_cooldowns.Remove(item);
			}
		}

		public static void Call(object key, float cooldown, Action action)
		{
			if (!_instance._cooldowns.ContainsKey(key))
			{
				action();
				_instance._cooldowns.Add(key, Time.time + cooldown);
			}
		}

		public static void Reset(object key)
		{
			_instance._cooldowns.Remove(key);
		}
	}
}
