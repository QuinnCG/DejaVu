using UnityEngine.VFX;

namespace Quinn
{
	public static class VisualEffectExtensions
	{
		/// <summary>
		/// Sends the "OnPlay" event.
		/// </summary>
		public static void GenericPlay(this VisualEffect vfx)
		{
			vfx.SendEvent("OnPlay");
		}

		/// <summary>
		/// Sends the "OnStop" event.
		/// </summary>
		public static void GenericStop(this VisualEffect vfx)
		{
			vfx.SendEvent("OnStop");
		}

		/// <summary>
		/// Checks if the VFX has any alive particles.
		/// </summary>
		public static bool IsPlaying(this VisualEffect vfx)
		{
			return vfx.aliveParticleCount > 0;
		}
	}
}
