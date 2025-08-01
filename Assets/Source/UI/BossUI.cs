using DG.Tweening;
using Quinn.DamageSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quinn.UI
{
	public class BossUI : MonoBehaviour
	{
		public static BossUI Instance { get; private set; }

		[SerializeField]
		private CanvasGroup Group;
		[SerializeField]
		private Image Bar;
		[SerializeField]
		private TextMeshProUGUI Title;

		[Space, SerializeField]
		private float FadeInDuration = 0.1f, FadeOutDuration = 0.5f;

		private Health _health;

		private void Awake()
		{
			Instance = this;
			Group.alpha = 0f;
		}

		private void LateUpdate()
		{
			if (_health != null)
			{
				Bar.fillAmount = _health.Normalized;
			}
		}

		public void SetBoss(string title, Health health)
		{
			Title.text = title;
			_health = health;
			_health.OnDeath += () =>
			{
				ClearBoss();
			};

			Group.DOKill();
			Group.DOFade(1f, FadeInDuration);
		}

		public void ClearBoss()
		{
			Group.DOKill();
			Group.DOFade(0f, FadeOutDuration);

			_health = null;
		}
	}
}
