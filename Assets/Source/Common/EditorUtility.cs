namespace Quinn
{
	/// <summary>
	/// Editor utilities that are safe to to use in builds or are safely stripped from builds.
	/// </summary>
	public static class EditorUtility
	{
		/// <summary>
		/// Is the prefab editing mode of the editor open right now or not?
		/// </summary>
		public static bool InPrefabMode()
		{
#if UNITY_EDITOR
			return UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null;
#else
			return false;
#endif
		}
	}
}
