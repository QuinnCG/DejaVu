using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

namespace Quinn
{
	[InfoBox("Destroy this game object when the VFX component no longer has any alive particles.")]
	[RequireComponent(typeof(VisualEffect))]
	public class DestroyVFXOnFinish : MonoBehaviour
	{
		[field: ShowInInspector, ReadOnly]
		public int AliveParticles { get; private set; }

		private VisualEffect _vfx;

		private void Awake()
		{
			_vfx = GetComponent<VisualEffect>();
		}

		private void FixedUpdate()
		{
			if (Time.frameCount % 2 == 0)
			{
				AliveParticles = _vfx.aliveParticleCount;

				if (_vfx.aliveParticleCount <= 0)
				{
					gameObject.Destroy();
				}
			}
		}
	}
}
