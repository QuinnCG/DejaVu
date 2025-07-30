using UnityEngine;

namespace Quinn
{
	public static class MathTools
	{
		public static Vector2 GetVectorFromRadians(float radians)
		{
			return new Vector2()
			{
				x = Mathf.Cos(radians),
				y = Mathf.Sin(radians)
			};
		}

		public static Vector2 GetVectorFromDegrees(float degrees)
		{
			return GetVectorFromRadians(degrees.ToRadians());
		}

		/// <summary>
		/// Calculate the duration to traverse the given distance at the given speed.<br/>
		/// <c>duration = distance / speed</c>
		/// </summary>
		public static float GetDuration(float distance, float speed)
		{
			return distance / speed;
		}

		public static bool IsNearlyEqual(float a, float b, float tolerance = float.Epsilon)
		{
			return Mathf.Abs(a - b) <= tolerance;
		}
	}
}
