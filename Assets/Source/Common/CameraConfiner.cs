using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace Quinn
{
	[InfoBox("Attached collider activates confiner. Direct child should be the actual camera bounds.")]
	[RequireComponent(typeof(Collider2D))]
	public class CameraConfiner : MonoBehaviour
	{
		[SerializeField, Required]
		private CinemachineCamera VirtualCamera;

		private void OnValidate()
		{
			var collider = GetComponent<Collider2D>();
			collider.isTrigger = true;

			if (VirtualCamera != null)
			{
				VirtualCamera.enabled = false;
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.IsPlayer())
			{
				VirtualCamera.enabled = true;
				VirtualCamera.Target.TrackingTarget = collision.transform;
			}
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			if (collision.IsPlayer())
			{
				VirtualCamera.enabled = false;
			}
		}
	}
}
