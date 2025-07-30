using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quinn
{
	public class Bufferer : MonoBehaviour
	{
		private class BufferHandle
		{
			public Action Action;
			public Func<bool> Predicate;
			public float ExpirationTime;
		}

		[RuntimeInitializeOnLoadMethod, System.Diagnostics.Conditional("UNITY_EDITOR")]
		private static void ResetStatic() => _bufferedHandles.Clear();

		private static readonly Dictionary<object, BufferHandle> _bufferedHandles = new();

		private void Update()
		{
			var toRemove = new HashSet<object>();

			foreach (var handle in _bufferedHandles.Values)
			{
				if (Time.time > handle.ExpirationTime)
				{
					toRemove.Add(handle);
				}

				if (handle.Predicate())
				{
					handle.Action();
					toRemove.Add(handle);
				}
			}

			_bufferedHandles.RemoveRange(toRemove);
		}

		/// <summary>
		/// Delay an <see cref="Action"/> until a given <see cref="Func{TResult}"/> predicate is met.<br/>
		/// If the <c>expiration</c> duration is reached, then cancel the buffered action all together.
		/// </summary>
		/// <param name="action">The callback to be executed if the buffer's predicate is met.</param>
		/// <param name="predicate">The condition to wait for before executing the buffered action.</param>
		/// <param name="expiration">The duration, in seconds, before the buffer is cancelled.</param>
		/// <param name="key">
		/// A reference object used in preventing duplicate instances of a given buffer.<br/>
		/// You may leave this as null if you don't care or are not worried about duplicate calls.
		/// </param>
		/// <returns>True, if the predicate was already valid and thus the action, instantly called.</returns>
		public static bool Buffer(Action action, Func<bool> predicate, float expiration = float.PositiveInfinity, object key = null)
		{
			if (key != null)
			{
				Cancel(key);
			}

			if (predicate())
			{
				action();
				return true;
			}

			var handle = new BufferHandle()
			{
				Action = action,
				Predicate = predicate,
				ExpirationTime = Time.time + expiration,
			};

			key ??= handle;
			_bufferedHandles.Add(key, handle);

			return false;
		}

		/// <summary>
		/// Cancel the specified buffered instance. This requires you to have created the buffer with the <c>key</c> parameter set.
		/// </summary>
		/// <param name="key">The object reference shared with the buffer you wish to cancel.</param>
		public static void Cancel(object key)
		{
			_bufferedHandles.Remove(key);
		}
	}
}
