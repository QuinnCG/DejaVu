using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Quinn
{
	/// <summary>
	/// An animator that plays <see cref="AnimationClip"/>s directly.
	/// </summary>
	public class PlayableAnimator : MonoBehaviour
	{
		[SerializeField]
		private AnimationClip DefaultAnim;
		[SerializeField]
		private bool LoopDefaultAnim = true;

		private PlayableGraph _graph;
		private AnimationPlayableOutput _output;

		[ShowInInspector, ReadOnly]
		private AnimationClip LoopingAnimation;

		public bool IsPlayingOneShot => _isOneShotPlaying;

		private float _nextOneShotEndTime;
		private bool _isOneShotPlaying;
		private bool _holdOneShotEndFrame;

		private void Awake()
		{
			if (TryGetComponent(out Animator a))
			{
				DestroyImmediate(a);
			}

			var animator = gameObject.AddComponent<Animator>();

			_graph = PlayableGraph.Create("Playable Animator Graph");
			_output = AnimationPlayableOutput.Create(_graph, "Animation Output", animator);

			if (DefaultAnim != null)
			{
				if (LoopDefaultAnim)
				{
					PlayLooped(DefaultAnim);
				}
				else
				{
					PlayOnce(DefaultAnim, true);
				}
			}
		}

		private void Update()
		{
			if (_isOneShotPlaying && Time.time >= _nextOneShotEndTime && !_holdOneShotEndFrame)
			{
				_isOneShotPlaying = false;
				
				if (LoopingAnimation != null)
				{
					PlayAnimClip(LoopingAnimation);
				}
				else
				{
					Stop();
				}
			}
		}

		private void OnDestroy()
		{
			_graph.Destroy();
		}

		private void OnEnable()
		{
			_graph.Play();
		}

		private void OnDisable()
		{
			_graph.Stop();
		}

		public void PlayLooped(AnimationClip anim, bool overrideOneShot = false)
		{
			if (anim == null)
				Log.Error("Can't play null animation!");

			if (overrideOneShot)
			{
				StopOneShot();
			}

			if (LoopingAnimation != anim && !_isOneShotPlaying)
			{
				LoopingAnimation = anim;
				PlayAnimClip(anim);
			}
		}

		public void PlayOnce(AnimationClip anim, bool holdEndFrame = false)
		{
			if (anim == null)
				Log.Error("Can't play null animation!");

			_nextOneShotEndTime = Time.time + anim.length;
			PlayAnimClip(anim);

			_isOneShotPlaying = true;
			_holdOneShotEndFrame = holdEndFrame;
		}

		public void Stop()
		{
			_graph.Stop();
			LoopingAnimation = null;
			_isOneShotPlaying = false;
			_holdOneShotEndFrame = false;
		}

		public void StopOneShot()
		{
			_nextOneShotEndTime = -1f;
			_holdOneShotEndFrame = false;
		}

		private void PlayAnimClip(AnimationClip anim)
		{
			if (!enabled)
				return;

			// Remove current, if it exists.
			var current = _output.GetSourcePlayable();

			if (current.IsValid())
			{
				current.Destroy();
			}

			// Create new.
			var playableClip = AnimationClipPlayable.Create(_graph, anim);
			_output.SetSourcePlayable(playableClip);

			// Play graph, in case it isn't already playing.
			_graph.Play();
		}
	}
}
