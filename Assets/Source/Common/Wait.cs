using System;
using System.Threading;
using UnityEngine;

namespace Quinn
{
	public static class Wait
	{
		public static async Awaitable Duration(float seconds, CancellationToken token = default)
		{
			try
			{
				await Awaitable.WaitForSecondsAsync(seconds, token);
			}
			catch (OperationCanceledException) { }
		}

		public static async Awaitable NextFrame(CancellationToken token = default)
		{
			try
			{
				await Awaitable.NextFrameAsync(token);
			}
			catch (OperationCanceledException) { }
		}
		/// <summary>
		/// Wait for more than 1 frame.
		/// </summary>
		public static async Awaitable NextFrame(int frames, CancellationToken token = default)
		{
			for (int i = 0; i < frames; i++)
			{
				if (token.IsCancellationRequested)
				{
					break;
				}

				await Awaitable.NextFrameAsync(token);
			}
		}

		public static async Awaitable While(Func<bool> predicate, CancellationToken token = default)
		{
			while (!token.IsCancellationRequested && predicate())
			{
				await NextFrame(token);
			}
		}

		public static async Awaitable Until(Func<bool> predicate, float timeout, Action timeoutCallback, CancellationToken token = default)
		{
			float endTime = Time.time + timeout;

			while (token.IsCancellationRequested && !predicate() && Time.time < endTime)
			{
				await NextFrame(token);
			}

			if (Time.time >= endTime)
			{
				timeoutCallback?.Invoke();
			}
		}
		public static async Awaitable Until(Func<bool> predicate, CancellationToken token = default)
		{
			while (!token.IsCancellationRequested && !predicate())
			{
				await NextFrame(token);
			}
		}
	}
}
