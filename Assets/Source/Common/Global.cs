using FMODUnity;
using QFSW.QC;
using System;
using System.IO;
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

#if UNITY_WEBGL
			// Required, or the player won't be able to move.
			SceneManager.LoadScene(0);
#endif
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
