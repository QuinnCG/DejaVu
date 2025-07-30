using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Quinn
{
	/// <summary>
	/// A utility class for spawning VFX prefabs that use the <see cref="VisualEffect"/> component.<br/>
	/// Utilizies a factory pattern, with method chaining.
	/// 
	/// <example>
	/// <code>
	/// <see cref="VFX.Create(GameObject)"/><br/>
	///		.<see cref="Set(string, Vector2)"/><br/>
	///		.<see cref="DestroyOnFinish()"/><br/>
	///		.<see cref="Spawn(Vector2, Transform)"/>;<br/>
	/// </code>
	/// </example>
	/// </summary>
	public class VFX
	{
		private GameObject _prefab;
		private bool _destroyOnFinish;

		private readonly List<(string name, object value, System.Type type)> _toSetVariables = new();

		private VFX() { }

		public static VFX Create(GameObject prefab) => new()
		{
			_prefab = prefab
		};

		public static VFX Create(string resource) => new()
		{
			_prefab = Resources.Load<GameObject>(resource)
		};

		public VFX DestroyOnFinish()
		{
			_destroyOnFinish = true;
			return this;
		}

		public VFX Set(string name, float value)
		{
			_toSetVariables.Add((name, value, typeof(float)));
			return this;
		}
		public VFX Set(string name, int value)
		{
			_toSetVariables.Add((name, value, typeof(int)));
			return this;
		}
		public VFX Set(string name, Vector2 value)
		{
			_toSetVariables.Add((name, value, typeof(Vector2)));
			return this;
		}
		public VFX Set(string name, Vector3 value)
		{
			_toSetVariables.Add((name, value, typeof(Vector3)));
			return this;
		}
		public VFX Set(string name, bool value)
		{
			_toSetVariables.Add((name, value, typeof(bool)));
			return this;
		}
		public VFX Set(string name, Gradient value)
		{
			_toSetVariables.Add((name, value, typeof(Gradient)));
			return this;
		}
		public VFX Set(string name, AnimationCurve value)
		{
			_toSetVariables.Add((name, value, typeof(AnimationCurve)));
			return this;
		}
		public VFX Set(string name, Texture2D value)
		{
			_toSetVariables.Add((name, value, typeof(Texture2D)));
			return this;
		}

		public VisualEffect Spawn(Vector2 pos, Transform parent = default)
		{
			var instance = _prefab.Clone(pos, parent);
			var vfx = instance.GetComponent<VisualEffect>();

			if (_destroyOnFinish)
			{
				instance.AddComponent<DestroyVFXOnFinish>();
			}

			foreach (var (name, value, type) in _toSetVariables)
			{
				if (type == typeof(float))
				{
					vfx.SetFloat(name, (float)value);
				}
				else if (type == typeof(int))
				{
					vfx.SetInt(name, (int)value);
				}
				else if (type == typeof(Vector2))
				{
					vfx.SetVector2(name, (Vector2)value);
				}
				else if (type == typeof(Vector3))
				{
					vfx.SetVector3(name, (Vector3)value);
				}
				else if (type == typeof(bool))
				{
					vfx.SetBool(name, (bool)value);
				}
				else if (type == typeof(Gradient))
				{
					vfx.SetGradient(name, (Gradient)value);
				}
				else if (type == typeof(AnimationCurve))
				{
					vfx.SetAnimationCurve(name, (AnimationCurve)value);
				}
				else if (type == typeof(Texture2D))
				{
					vfx.SetTexture(name, (Texture2D)value);
				}
			}

			vfx.Play();
			return vfx;
		}
	}
}
