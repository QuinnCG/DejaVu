using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Quinn
{
	public static class Audio
	{
		public static void Play(string sound, Vector2 position = default)
		{
			var s = RuntimeManager.PathToEventReference(sound);
			if (!s.IsNull)
			{
				RuntimeManager.PlayOneShot(sound, position);
			}
		}
		public static void Play(EventReference sound, Vector2 position = default)
		{
			if (!sound.IsNull)
			{
				RuntimeManager.PlayOneShot(sound, position);
			}
		}
		public static void Play(string sound, Transform parent)
		{
			var s = RuntimeManager.PathToEventReference(sound);
			if (!s.IsNull)
			{
				RuntimeManager.PlayOneShotAttached(sound, parent.gameObject);
			}
		}
		public static void Play(EventReference sound, Transform parent)
		{
			if (!sound.IsNull)
			{
				RuntimeManager.PlayOneShotAttached(sound, parent.gameObject);
			}
		}

		public static EventInstance Create(string sound)
		{
			var instance = RuntimeManager.CreateInstance(sound);
			return instance;
		}
		public static EventInstance Create(EventReference sound)
		{
			var instance = RuntimeManager.CreateInstance(sound);
			return instance;
		}
		public static EventInstance Create(string sound, Vector2 position)
		{
			var instance = RuntimeManager.CreateInstance(sound);
			instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));

			return instance;
		}
		public static EventInstance Create(EventReference sound, Vector2 position)
		{
			var instance = RuntimeManager.CreateInstance(sound);
			instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));

			return instance;
		}
		public static EventInstance Create(string sound, Transform parent)
		{
			var instance = RuntimeManager.CreateInstance(sound);
			RuntimeManager.AttachInstanceToGameObject(instance, parent.gameObject);

			return instance;
		}
		public static EventInstance Create(EventReference sound, Transform parent)
		{
			var instance = RuntimeManager.CreateInstance(sound);
			RuntimeManager.AttachInstanceToGameObject(instance, parent.gameObject);

			return instance;
		}

		/// <summary>
		/// Returns an FMOD bus/audio channel.
		/// </summary>
		/// <param name="name">The name of the bus channel. This should NOT start with 'bus:/'.</param>
		public static Bus GetBus(string name)
		{
			RuntimeManager.StudioSystem.getBus($"bus:/{name}", out Bus bus);
			return bus;
		}
		public static Bus GetMasterBus()
		{
			// Master bus channel is obtained via only looking up "bus:/".
			return GetBus(string.Empty);
		}

		public static void SetGlobalParameter(string name, float value, bool ignoreSeedSpeed = false)
		{
			RuntimeManager.StudioSystem.setParameterByName(name, value, ignoreSeedSpeed);
		}
		public static void SetGlobalParameter(string name, int value, bool ignoreSeedSpeed = false)
		{
			RuntimeManager.StudioSystem.setParameterByName(name, value, ignoreSeedSpeed);
		}
		public static void SetGlobalParameter(string name, bool value, bool ignoreSeedSpeed = false)
		{
			RuntimeManager.StudioSystem.setParameterByName(name, value ? 1f : 0f, ignoreSeedSpeed);
		}
		public static void SetGlobalParameter(string name, string value, bool ignoreSeedSpeed = false)
		{
			RuntimeManager.StudioSystem.setParameterByNameWithLabel(name, value, ignoreSeedSpeed);
		}
	}
}
