using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace Quinn
{
	public class CameraConfiner : MonoBehaviour
	{
		public void SetActiveConfiner()
		{
			StopAllCoroutines();
			StartCoroutine(SetActiveSequence());
		}

		// This is required for when the player respawns inside of a confiner trigger.
		private IEnumerator SetActiveSequence()
		{
			yield return new WaitUntil(() => CinemachineBrain.GetActiveBrain(0).ActiveVirtualCamera != null);

			var vcam = ((CinemachineCamera)CinemachineBrain.GetActiveBrain(0).ActiveVirtualCamera);
			var confiner = vcam.GetComponent<CinemachineConfiner2D>();

			confiner.BoundingShape2D = GetComponent<Collider2D>();
		}
	}
}
