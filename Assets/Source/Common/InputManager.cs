using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quinn
{
	public class InputManager : MonoBehaviour
	{
		public static InputManager Instance { get; private set; }
		public static bool IsInputDisabled => _inputBlockers.Count > 0;

		private readonly HashSet<CursorStateHandle> _cursorStateHandles = new();
		private static readonly HashSet<object> _inputBlockers = new();

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void StaticReset()
		{
			_inputBlockers.Clear();
		}

		private void Awake()
		{
			Instance = this;

			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void Update()
		{
			if (_cursorStateHandles.Any(x => x.ForceShowCursor))
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.Confined;
			}
			else
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Confined;
			}
		}

		private void OnDestroy()
		{
			Instance = null;
		}

		/// <summary>
		/// Create and register an instance of <see cref="CursorStateHandle"/> and return it.
		/// </summary>
		/// <param name="forceShowByDefault">If true, the default state of the created handle will make the cursor visible.</param>
		/// <returns>The created instance.</returns>
		public CursorStateHandle GetCursorStateHandle(bool forceShowByDefault = true)
		{
			var handle = new CursorStateHandle() { ForceShowCursor = forceShowByDefault };
			_cursorStateHandles.Add(handle);

			return handle;
		}

		/// <summary>
		/// Unregister an instance of <see cref="CursorStateHandle"/>.
		/// </summary>
		public void RemoveCursorStateHandle(CursorStateHandle handle)
		{
			_cursorStateHandles.Remove(handle);
		}

		/* BLOCK INPUT */

		/// <summary>
		/// Disable all input routed through <see cref="InputManager"/>.<br/>
		/// Make sure you have a reference to the key you use to that you can re-enable input later via <see cref="UnblockInput(object)"/>.
		/// </summary>
		/// <param name="key">A reference to be used as a key.</param>
		public void BlockInput(object key)
		{
			_inputBlockers.Add(key);
		}

		/// <summary>
		/// Remove a key blocking all input from working.<br/>
		/// This doesn't mean input will actually be enabled again, just that this key won't be used in blocking said input.
		/// </summary>
		/// <param name="key">The reference used to block gameplay input via <see cref="BlockInput(object)"/></param>.

		public void UnblockInput(object key)
		{
			_inputBlockers.Remove(key);
		}
	}
}
