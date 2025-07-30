using UnityEngine;

namespace Quinn
{
	public static class BoundsExtensions
	{
		public static Vector2 Left(this Bounds bounds)
		{
			return new Vector2(bounds.center.x - bounds.extents.x, bounds.center.y);
		}
		public static Vector2 Right(this Bounds bounds)
		{
			return new Vector2(bounds.center.x + bounds.extents.x, bounds.center.y);
		}
		public static Vector2 Top(this Bounds bounds)
		{
			return new Vector2(bounds.center.x, bounds.center.y + bounds.extents.y);
		}
		public static Vector2 Bottom(this Bounds bounds)
		{
			return new Vector2(bounds.center.x, bounds.center.y - bounds.extents.y);
		}
	}
}
