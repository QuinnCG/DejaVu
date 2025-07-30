using UnityEngine.SceneManagement;

namespace Quinn
{
	public static class SceneHelper
	{
		public static Scene[] GetLoadedScenes()
		{
			var sceneCount = SceneManager.sceneCount;
			var scenes = new Scene[sceneCount];

			for (int i = 0; i < sceneCount; i++)
			{
				scenes[i] = SceneManager.GetSceneAt(i);
			}

			return scenes;
		}
		
		public static void SetActiveScene(int buildIndex)
		{
			SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(buildIndex));
		}
		public static void SetActiveScene(string scenePath)
		{
			SceneManager.SetActiveScene(SceneManager.GetSceneByPath(scenePath));
		}
	}
}
