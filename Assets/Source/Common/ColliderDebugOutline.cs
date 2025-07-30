using System.Linq;
using UnityEngine;

namespace Quinn
{
	/// <summary>
	/// Displays a persistent outline for a collider (editor only), even when not selecting the collider.
	/// </summary>
	[RequireComponent(typeof(Collider2D))]
	public class ColliderDebugOutline : MonoBehaviour
	{
		[SerializeField]
		private Color Color = Color.white;

		private void OnDrawGizmos()
		{
			var collider = GetComponent<Collider2D>();

			Mesh mesh = collider.CreateMesh(true, true);
			mesh.Optimize();

			Vector3[] positions = mesh.vertices;
			positions = positions.OrderBy(pos => Vector3.SignedAngle(pos.normalized, Vector3.up, Vector3.forward)).ToArray();

			Gizmos.color = Color;
			Gizmos.DrawLineStrip(new(positions), true);
		}
	}
}
