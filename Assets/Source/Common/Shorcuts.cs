using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ShortcutManagement;
#endif

namespace Quinn
{
	public static class Shortcuts
	{
#if UNITY_EDITOR
		[Shortcut("Clear Console", KeyCode.C, ShortcutModifiers.Alt)]
#endif
		public static void ClearConsole()
		{
#if UNITY_EDITOR
			var assembly = Assembly.GetAssembly(typeof(SceneView));
			var type = assembly.GetType("UnityEditor.LogEntries");
			var method = type.GetMethod("Clear");
			method.Invoke(new object(), null);
#endif
		}
	}
}
