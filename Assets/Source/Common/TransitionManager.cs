using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Quinn
{
	/// <summary>
	/// Manages the screen fading to and from black. Also, manages audio related muting during this time.
	/// </summary>
	public class TransitionManager : MonoBehaviour
	{
		public static TransitionManager Instance { get; private set; }

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		protected static void StaticReset()
		{
			Instance = null;
		}

		[SerializeField]
		private CanvasGroup Blackout;

		[Space, SerializeField]
		private Ease FadeToBlackEase = Ease.InCubic;
		[SerializeField]
		private Ease FadeFromBlackEase = Ease.OutCubic;

		// This snapshot mutes most sound channels, but leaves music and reverb playing.
		private EventInstance _transitionSnapshot;

		private void Start()
		{
			Instance = this;
			_transitionSnapshot = RuntimeManager.CreateInstance("snapshot:/Transition");
		}

		private void OnDestroy()
		{
			_transitionSnapshot.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
			_transitionSnapshot.release();
		}

		public async Awaitable FadeToBlackAsync(float duration)
		{
			await Blackout.DOFade(1f, duration)
				.SetEase(FadeToBlackEase)
				.AsyncWaitForCompletion();

			_transitionSnapshot.start();
		}

		public async Awaitable FadeFromBlackAsync(float duration)
		{
			_transitionSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

			await Blackout.DOFade(0f, duration)
				.SetEase(FadeFromBlackEase)
				.AsyncWaitForCompletion();
		}
	}
}
