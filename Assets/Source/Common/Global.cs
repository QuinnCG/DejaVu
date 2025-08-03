using FMODUnity;
using QFSW.QC;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Quinn
{
	public class Global : MonoBehaviour
	{
		[SerializeField]
		private EventReference[] PlayAtGameStart;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Bootstrap()
		{
			var instance = Addressables.InstantiateAsync("Globals.prefab")
				.WaitForCompletion();

			instance.MakeTransient();
			instance.name = "Globals";
		}

		private void Start()
		{
			foreach (var sound in PlayAtGameStart)
			{
				Audio.Play(sound);
			}

			SceneManager.LoadScene(0);
		}

		[Command("quit", "Exit out of the game.")]
		protected void Quit_Cmd()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}
	}
}
