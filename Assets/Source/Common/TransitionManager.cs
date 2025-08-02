using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using QFSW.QC;
using System.Collections;
using UnityEngine;

namespace Quinn
{
	/// <summary>
	/// Manages the screen fading to and from black. Also, manages audio related muting during this time.
	/// </summary>
	public class TransitionManager : MonoBehaviour
	{
		public static TransitionManager Instance { get; private set; }

		[SerializeField]
		private bool StartBlack;
		[SerializeField]
		private CanvasGroup Blackout;

		// This snapshot mutes most sound channels, but leaves music and reverb playing.
		private EventInstance _transitionSnapshot;

		private void Awake()
		{
			Instance = this;
			_transitionSnapshot = RuntimeManager.CreateInstance("snapshot:/Transition");

			if (StartBlack)
			{
				_transitionSnapshot.start();
				Blackout.alpha = 1f;
			}
		}

		private void OnDestroy()
		{
			_transitionSnapshot.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
			_transitionSnapshot.release();
		}

		public void FadeToBlack(float duration)
		{
			StopAllCoroutines();
			StartCoroutine(FadeToBlackSequence(duration));

			_transitionSnapshot.start();
		}

		public void FadeFromBlack(float duration)
		{
			StopAllCoroutines();
			StartCoroutine(FadeFromBlackSequence(duration));

			_transitionSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}

		private IEnumerator FadeToBlackSequence(float duration)
		{
			for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
			{
				Blackout.alpha = Mathf.Lerp(0f, 1f, t);
				yield return null;
			}
		}

		private IEnumerator FadeFromBlackSequence(float duration)
		{
			for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
			{
				Blackout.alpha = Mathf.Lerp(1f, 0f, t);
				yield return null;
			}
		}

		[Command("fade")]
		protected void SetFade(float norm)
		{
			Blackout.DOKill();
			Blackout.alpha = Mathf.Clamp01(norm);
		}
	}
}
