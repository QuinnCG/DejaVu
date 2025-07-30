using System;
using UnityEngine;

namespace Quinn
{
	// TODO: Remake without complexity of callback system.
	public class Timer
	{
		public bool IsCounting { get; private set; }
		public bool IsPaused { get; private set; }
		public bool IsFinished => Remaining <= 0f;

		public float Elapsed => Mathf.Min(Time.time - _startTime, Duration);
		public float Remaining => Mathf.Max(0f, Duration - Elapsed);
		public float Duration { get; private set; }

		public float StartTime => _startTime;

		public event Action OnFinish;

		private bool _listeningToUpdate;

		private float _startTime;
		private float _endTime;

		private float _remainingDurAtPause;

		public Timer()
		{
			throw new System.NotImplementedException();
		}

		public override string ToString()
		{
			return $"{Elapsed}/{Duration}s";
		}

		public void Start(float duration)
		{
			if (!IsCounting)
			{
				IsCounting = true;
				IsPaused = false;

				Duration = duration;
				_startTime = Time.time;
				_endTime = Time.time + duration;

				if (_listeningToUpdate)
				{
					GlobalUpdate.OnUpdate -= OnUpdate;
					_listeningToUpdate = false;
				}

				if (OnFinish != null)
				{
					//GlobalUpdate.OnUpdate += OnUpdate;
					_listeningToUpdate = true;
				}
			}
		}

		public void Play()
		{
			if (IsPaused)
			{
				IsPaused = false;
			}
		}

		public void Pause()
		{
			if (IsCounting)
			{
				IsCounting = false;
				IsPaused = true;

				_remainingDurAtPause = Remaining;
			}
		}

		public void Stop()
		{
			if (IsCounting)
			{
				IsCounting = false;
				IsPaused = false;

				_startTime = 0f;
				_endTime = 0f;

				if (_listeningToUpdate)
				{
					GlobalUpdate.OnUpdate -= OnUpdate;
					_listeningToUpdate = false;
				}
			}
		}

		private void OnUpdate()
		{
			if (IsFinished)
			{
				IsCounting = false;
				IsPaused = false;

				GlobalUpdate.OnUpdate -= OnUpdate;
				_listeningToUpdate = false;

				OnFinish?.Invoke();
			}
		}
	}
}
