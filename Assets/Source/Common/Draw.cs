using System.Collections.Generic;
using UnityEngine;

namespace Quinn
{
	public class Draw : MonoBehaviour
	{
		class Command
		{
			public System.Action Callback;
			public float ExpirationTime;

			public Command(float expirationTime, System.Action callback)
			{
				ExpirationTime = expirationTime;
				Callback = callback;
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void ResetStatic() => _cmds = new();

		private static HashSet<Command> _cmds = new();

		private void OnDrawGizmos()
		{
			if (Application.isPlaying)
			{
				var toRemove = new List<Command>();

				foreach (var cmd in _cmds)
				{
					cmd.Callback?.Invoke();

					if (Time.time > cmd.ExpirationTime)
					{
						toRemove.Add(cmd);
					}
				}

				foreach (var cmd in toRemove)
				{
					_cmds.Remove(cmd);
				}
			}
		}

		public static void Clear()
		{
			_cmds.Clear();
		}

		public static void Rect(Vector3 center, Vector3 size, Color color, float duration = 0f, bool filled = false)
		{
			_cmds.Add(new(Time.time + duration, () =>
			{
				Gizmos.color = color;
				
				if (filled)
				{
                    Gizmos.DrawCube(center, size);
                }
				else
				{
                    Gizmos.DrawWireCube(center, size);
                }
			}));
		}

		public static void Sphere(Vector3 center, float radius, Color color, float duration = 0f, bool filled = false)
		{
			_cmds.Add(new(Time.time + duration, () =>
			{
				Gizmos.color = color;
				
				if (filled)
				{
                    Gizmos.DrawSphere(center, radius);
                }
				else
				{
                    Gizmos.DrawWireSphere(center, radius);
                }
			}));
		}

		public static void Line(Vector3 start, Vector3 end, Color color, float duration = 0f)
		{
			_cmds.Add(new(Time.time + duration, () =>
			{
				Gizmos.color = color;
				Gizmos.DrawLine(start, end);
			}));
		}

		public static void Ray(Vector3 start, Vector3 direction, Color color, float duration = 0f)
		{
			_cmds.Add(new(Time.time + duration, () =>
			{
				Gizmos.color = color;
				Gizmos.DrawRay(start, direction);
			}));
		}

		public static void Text(Vector3 pos, string text, int fontSize, Color color, float duration = 0f)
		{
#if UNITY_EDITOR
			_cmds.Add(new(Time.time + duration, () =>
			{
				UnityEditor.Handles.color = color;

				var guiStyle = new GUIStyle()
				{
					alignment = TextAnchor.MiddleCenter,
					fontSize = fontSize,
					richText = true
				};

				UnityEditor.Handles.Label(pos, text, guiStyle);
			}));
#endif
		}
	}
}
