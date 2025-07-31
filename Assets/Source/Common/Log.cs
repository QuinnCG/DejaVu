using UnityEngine;

namespace Quinn
{
	/// <summary>
	/// Logging methods for editor and development-build usage only. These methods are stripped in release builds.
	/// </summary>
	public static class Log
	{
		// HACK: These should use [Conditional("DEVELOPMENT")] or similar, but there isn't any built in "DEVELOPMENT" define in Unity.

		[HideInCallstack]
		public static void Info(object message)
		{
			if (Debug.isDebugBuild || Application.isEditor)
			{
				Debug.Log($"{message}".ToColor(Color.gray));
			}
		}

		[HideInCallstack]
		public static void Notice(object message)
		{
			if (Debug.isDebugBuild || Application.isEditor)
			{
				Debug.Log($"{message}".ToColor(Color.white));
			}
		}

		[HideInCallstack]
		public static void Important(object message)
		{
			if (Debug.isDebugBuild || Application.isEditor)
			{
				Debug.Log($"{message}".ToColor(Color.white).ToBold());
			}
		}

		[HideInCallstack]
		public static void Warning(object message)
		{
			if (Debug.isDebugBuild || Application.isEditor)
			{
				Debug.LogWarning($"{message}".ToColor(Color.yellow));
			}
		}

		[HideInCallstack]
		public static void Error(object message)
		{
			if (Debug.isDebugBuild || Application.isEditor)
			{
				Debug.LogError($"{message}".ToColor(Color.red));
			}
		}

		[HideInCallstack]
		public static void Fatal(object message)
		{
			if (Debug.isDebugBuild || Application.isEditor)
			{
				Debug.LogError($"{message}".ToColor(Color.darkRed));
				Debug.Break();
				Application.Quit(1);
			}
		}
	}
}
