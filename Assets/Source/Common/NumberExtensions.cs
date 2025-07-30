using UnityEngine;

namespace Quinn
{
	public static class NumberExtensions
	{
		public static float ToRadians(this float degrees)
		{
			return Mathf.Deg2Rad * degrees;
		}
		public static float ToDegrees(this float radians)
		{
			return Mathf.Rad2Deg * radians;
		}
		
		public static Vector2 ToDirection(this float degrees)
		{
			return new(Mathf.Cos(degrees), Mathf.Sin(degrees));
		}

		/// <summary>
		/// <c>Mathf.Abs(x - y)</c>
		/// </summary>
		public static float AbsDiff(this float x, float y)
		{
			return Mathf.Abs(x - y);
		}

		/// <remarks>
		/// This does not return a copy. This modifies the value directly.
		/// </remarks>
		/// <param name="min">Limit the value so that it is no lower than this.</param>
		public static void MakeAtLeast(this ref float value, float min)
		{
			value = Mathf.Max(value, min);
		}

		public static void MakeLessThan(this ref float value, float max)
		{
			value = Mathf.Min(value, max);
		}

		public static bool IsEven(this int value)
		{
			return value % 2 == 0;
		}

		public static bool IsOdd(this int value)
		{
			return value % 2 > 0;
		}

		public static bool NearlyEqual(this float a, float b, float tolerance = float.Epsilon)
		{
			return Mathf.Abs(a - b) <= tolerance;
		}
	}
}
