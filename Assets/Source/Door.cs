using DG.Tweening;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

namespace Quinn
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class Door : MonoBehaviour
	{
		[SerializeField]
		private bool StartClosed;

		[Space]

		[SerializeField]
		private float OpenDuration = 1f, CloseDuration = 1f;
		[SerializeField]
		private Ease OpenEase = Ease.Linear, CloseEase = Ease.Linear;

		[Space, Required, SerializeField]
		private Transform OpenPosition;

		[SerializeField, FoldoutGroup("FX")]
		private EventReference OpeningSound, ClosedSound;
		[SerializeField, FoldoutGroup("FX")]
		private VisualEffect OpeningVFX, ClosedVFX;

		public bool IsOpen { get; private set; }

		private Rigidbody2D _rb;

		private Vector2 _openPos;
		private Vector2 _closePos;

		private void Awake()
		{
			_rb = GetComponent<Rigidbody2D>();

			_openPos = OpenPosition.position;
			_closePos = _rb.position;

			if (!StartClosed)
			{
				_rb.position = _openPos;
			}

			IsOpen = !StartClosed;
		}

		public void Open()
		{
			if (IsOpen)
				return;

			IsOpen = true;

			_rb.DOKill();
			_rb.DOMove(_openPos, OpenDuration).SetEase(OpenEase);

			if (OpeningVFX != null)
			{
				OpeningVFX.Play();
			}

			Audio.Play(OpeningSound, transform.position);
		}

		public void Close()
		{
			if (!IsOpen)
				return;

			IsOpen = false;

			_rb.DOKill();
			_rb.DOMove(_closePos, CloseDuration)
				.SetEase(CloseEase)
				.OnComplete(() =>
				{
					Audio.Play(ClosedSound, transform.position);

					if (ClosedVFX != null)
					{
						ClosedVFX.Play();
					}
				});
		}
	}
}
