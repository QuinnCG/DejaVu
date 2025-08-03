using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Quinn.UI
{
	public class EndScreenUI : MonoBehaviour
	{
		private bool _buttonPressed;
		CursorStateHandle _cursorHandle;

		private void Awake()
		{
			TransitionManager.Instance.FadeFromBlack(1f);
			_cursorHandle = InputManager.Instance.GetCursorStateHandle(true);
		}

		private void OnDestroy()
		{
			_cursorHandle.ForceShowCursor = false;
		}

		public void Retry()
		{
			if (_buttonPressed)
			{
				return;
			}

			_buttonPressed = true;

			StartCoroutine(RetrySeq());
		}

		public void Quit()
		{
			if (_buttonPressed)
			{
				return;
			}

			_buttonPressed = true;

			StartCoroutine(QuitSeq());
		}

		private IEnumerator RetrySeq()
		{
			Checkpoint.ClearActive();

			TransitionManager.Instance.FadeToBlack(2f);
			yield return new WaitForSeconds(2f);

			SceneManager.LoadScene(0);
		}

		private IEnumerator QuitSeq()
		{
			TransitionManager.Instance.FadeToBlack(1f);
			yield return new WaitForSeconds(1f);

			Application.Quit();
		}
	}
}
