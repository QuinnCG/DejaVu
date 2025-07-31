using UnityEngine;

namespace Quinn
{
	public static class RaycastHit2DExtensions
	{
		/// <returns>True, if the collider member is not null.</returns>
		public static bool IsValid(this RaycastHit2D hit)
		{
			return hit.collider != null;
		}
	}
}
