using QFSW.QC;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

namespace Quinn
{
	public class Global : MonoBehaviour
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Bootstrap()
		{
			var instance = Addressables.InstantiateAsync("Globals.prefab")
				.WaitForCompletion();

			instance.MakeTransient();
			instance.name = "Globals";
		}

		private void Update()
		{
			if (Keyboard.current.f4Key.wasPressedThisFrame)
			{
				Debug.Break();
			}
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
