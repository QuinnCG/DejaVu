using UnityEngine;

namespace Quinn
{
	/// <summary>
	/// A static helper class with some useful collision-related properties and methods.
	/// </summary>
	public static class CollisionHelper
	{
		/// <summary>
		/// Name of the obstacle layer.
		/// </summary>
		public const string ObstacleName = "Obstacle";
		/// <summary>
		/// Name of the character layer.
		/// </summary>
		public const string CharacterName = "Character";

		/// <summary>
		/// Integer mask representing the obstacle layer only.
		/// </summary>
		public static int ObstacleMask { get; } = LayerMask.GetMask(ObstacleName);
		/// <summary>
		/// Integer mask representing the character layer only.
		/// </summary>
		public static int CharacterMask { get; } = LayerMask.GetMask(CharacterName);
	}
}
