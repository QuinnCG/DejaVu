using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn
{
	/// <summary>
	/// Primarily for use by animation events.
	/// </summary>
	[InfoBox("Primarily for use by animation events.")]
	public class SoundPlayer : MonoBehaviour
	{
		/// <summary>
		/// Play a sound at this game object's origin position.
		/// </summary>
		/// <param name="sound">The FMOD path to the sound; e.g. '<c>event:/Player/Footstep</c>'.</param>
		public void PlaySound(string sound)
		{
			Audio.Play(sound, transform.position);
		}
	}
}
