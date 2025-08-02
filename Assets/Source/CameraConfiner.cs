using Unity.Cinemachine;
using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(Collider2D))]
	public class CameraConfiner : MonoBehaviour
	{
		private Collider2D _collider;

		private void Awake()
		{
			_collider = GetComponent<Collider2D>();
		}

		public void SetActiveConfiner()
		{
			var vcam = ((CinemachineCamera)CinemachineBrain.GetActiveBrain(0).ActiveVirtualCamera);
			var confiner = vcam.GetComponent<CinemachineConfiner2D>();

			confiner.BoundingShape2D = _collider;
		}
	}
}
