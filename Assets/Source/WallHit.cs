using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class WallHit : MonoBehaviour
	{
		[SerializeField]
		private EventReference ImpactSound;
		[SerializeField]
		private float SpeedThreshold = 5f;
		[SerializeField]
		private float MaxVolumeThreshold = 40f;

		/// <summary>
		/// Called when an impact occurs. Provides relative the speed of the impact.
		/// </summary>
		public event System.Action<float> OnImpact;

		private EventInstance _instance;

		private void Awake()
		{
			_instance = RuntimeManager.CreateInstance(ImpactSound);
		}

		private void OnDestroy()
		{
			_instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			_instance.release();
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			float speed = collision.relativeVelocity.magnitude;
			OnImpact?.Invoke(speed);

			if (speed >= SpeedThreshold)
			{
				float volume = (speed - SpeedThreshold) / (MaxVolumeThreshold - SpeedThreshold);

				_instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
				_instance.setParameterByName("volume", volume);
				_instance.start();
			}
		}
	}
}
